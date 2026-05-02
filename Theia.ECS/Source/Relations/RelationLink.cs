using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Entities;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

/// <summary>
/// Single edge in a <see cref="RelationLink"/>: the entity that holds the <b>inverse view of a
/// relation</b>, paired with an internal foreign key used by the framework to locate the link's
/// metadata. Implicitly converts to <see cref="Entities.Entity">Entity</see> for ergonomic iteration.
/// </summary>
public record struct ExternalLink
{
    /// <summary>The entity that points at the link's owner under the relation type this link belongs to.</summary>
    public required Entity Entity { get; init; }

    /// <summary>Internal foreign key used by the relations indexing system to locate this link's entry.</summary>
    internal int ForeignKey { get; init; }

    /// <summary>Implicitly unwraps the link to its underlying <see cref="Entities.Entity">Entity</see>.</summary>
    public static implicit operator Entity(ExternalLink e) => e.Entity;
}

/// <summary>
/// Sparse-side entry pairing the dense index of an <see cref="ExternalLink"/> with the composite
/// identifier used by the relations indexing system to locate the link inside the owner's
/// <see cref="Relation"/>.
/// </summary>
internal record struct KeyIndexer
{
    /// <summary>Index into <see cref="RelationLink"/>'s dense external-links array, or <see cref="RelationLink.InvalidIndex"/> when the slot is unoccupied.</summary>
    internal required int ExternalLinkIndex { get; init; }

    /// <summary>The composite identifier locating the link inside the owner's <see cref="Relation"/>.</summary>
    internal required int CompositeKey { get; init; }

    private static KeyIndexer _invalid = new KeyIndexer()
    {
        ExternalLinkIndex = RelationLink.InvalidIndex,
        CompositeKey = RelationLink.InvalidIndex,
    };

    /// <summary>The sentinel <see cref="KeyIndexer"/> used to mark an empty slot.</summary>
    internal static KeyIndexer Invalid => _invalid;
}

/// <summary>
/// Per-target view of one relation type: the inverse side of <see cref="Relation"/>. Where a
/// <see cref="Relation"/> holds the owner's list of targets, <see cref="RelationLink"/> holds a
/// target's list of owners pointing at it, indexed for O(1) "am I a target?" lookups.
/// </summary>
/// <remarks>
/// <para>
/// The structure is the classic sparse-set layout: <see cref="_externalLinks"/> is dense and
/// packed (used for iteration); <see cref="_keysIndexer"/> is indexed by foreign key and stores
/// each link's position in the dense array. Removal is O(1) via swap-with-last with the swapped
/// link's <see cref="KeyIndexer"/> patched to its new dense index.
/// </para>
/// <para>
/// <b>Together with <see cref="Relation"/>, this is the bilateral half of Theia's relation
/// representation: every link is recorded on both sides, so both "who am I related to?" and
/// "who is related to me?" run in O(1)</b>.
/// </para>
/// <para>
/// Instances are pooled by <see cref="RelationType"/> and reused across owners; <see cref="Reset"/>
/// returns the structure to its empty state without releasing the underlying arrays.
/// </para>
/// </remarks>
internal sealed class RelationLink
{
    /// <summary>Sentinel value used by index fields when no link is present.</summary>
    internal const int InvalidIndex = -1;
    private const int DefaultRelationsIndexerCapacity = 4;
    private const int DefaultRelationsIndexerGrowthFactor = 2;

    internal readonly Lock _lock = new();

    private int _addedLinkIndex;
    private int _externalLinksCount;
    private ExternalLink[] _externalLinks;
    private KeyIndexer[] _keysIndexer;

    internal RelationLink()
    {
        _addedLinkIndex = InvalidIndex;
        _keysIndexer = Array.Empty<KeyIndexer>();
        _externalLinks = new ExternalLink[DefaultRelationsIndexerCapacity];
    }

    /// <summary>Returns the number of inverse links currently held.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetExternalLinksCount() => _externalLinksCount;

    /// <summary>Returns a span over the populated portion of the external links, suitable for iteration without exposing trailing capacity.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<ExternalLink> GetExternalLinks() =>
        _externalLinks.AsSpan(0, _externalLinksCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ExternalLink GetExternalLinkAt(int index) => _externalLinks[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetIndexerAddedLinkIndex() => _addedLinkIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetIndexerAddedLinkIndex(int index) => _addedLinkIndex = index;

    /// <summary>
    /// Returns <see langword="true"/> if a link is present for <paramref name="foreignKey"/>.
    /// Bounds-checks the sparse array first so unseen foreign keys return <see langword="false"/>
    /// without indexing past the end.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasExternalLink(int foreignKey) =>
        foreignKey < _keysIndexer.Length
        && _keysIndexer[foreignKey].ExternalLinkIndex != InvalidIndex;

    /// <summary>Returns the composite identifier locating the link at <paramref name="foreignKey"/> inside the owner's <see cref="Relation"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetCompositeKey(int foreignKey) => _keysIndexer[foreignKey].CompositeKey;

    /// <summary>Replaces the composite key for the link at <paramref name="foreignKey"/>, leaving its dense index unchanged. Used when the owner-side <see cref="Relation"/> swaps a target's slot.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void UpdateCompositeKey(int foreignKey, int compositeKey)
    {
        KeyIndexer keyIndexer = _keysIndexer[foreignKey];
        _keysIndexer[foreignKey] = keyIndexer with { CompositeKey = compositeKey };
    }

    /// <summary>
    /// Adds an inverse link from <paramref name="entity"/> at the given <paramref name="foreignKey"/>,
    /// recording <paramref name="compositeKey"/> so the framework can locate the link in the
    /// owner-side <see cref="Relation"/>. Grows both the dense and sparse arrays as needed.
    /// </summary>
    /// <remarks>
    /// The dense <see cref="_externalLinks"/> array grows by a fixed factor; the sparse
    /// <see cref="_keysIndexer"/> array grows just enough to address the new foreign key, with
    /// the newly opened range filled with <see cref="KeyIndexer.Invalid"/>.
    /// </remarks>
    internal void AddExternalLink(Entity entity, int foreignKey, int compositeKey)
    {
        int externalLinkIndex = _externalLinksCount;

        Array.TryResize(ref _externalLinks, externalLinkIndex, DefaultRelationsIndexerGrowthFactor);

        _externalLinks[externalLinkIndex] = new() { Entity = entity, ForeignKey = foreignKey };

        _externalLinksCount++;

        if (foreignKey >= _keysIndexer.Length)
        {
            int oldLength = _keysIndexer.Length;
            Array.Resize(ref _keysIndexer, foreignKey + 1);

            _keysIndexer.AsSpan(oldLength).Fill(KeyIndexer.Invalid);
        }

        _keysIndexer[foreignKey] = new()
        {
            ExternalLinkIndex = externalLinkIndex,
            CompositeKey = compositeKey,
        };
    }

    /// <summary>
    /// Removes the link identified by <paramref name="foreignKey"/> in O(1) by swapping the
    /// last dense entry into the freed slot and patching the swapped entry's
    /// <see cref="KeyIndexer"/> to point at its new position.
    /// </summary>
    /// <remarks>
    /// Iteration order over <see cref="GetExternalLinks"/> is therefore <b>not</b> stable across
    /// removals; consumers that depend on a particular order should sort or copy first.
    /// </remarks>
    internal void RemoveExternalLink(int foreignKey)
    {
        int externalLinkIndex = _keysIndexer[foreignKey].ExternalLinkIndex;

        int lastExternalLink = _externalLinksCount - 1;

        _externalLinksCount--;

        if (externalLinkIndex < lastExternalLink)
        {
            ExternalLink swappedExternalLink = _externalLinks[lastExternalLink];

            KeyIndexer swappedKeyIndexer = _keysIndexer[swappedExternalLink.ForeignKey];

            _keysIndexer[swappedExternalLink.ForeignKey] = swappedKeyIndexer with
            {
                ExternalLinkIndex = externalLinkIndex,
            };

            _externalLinks[externalLinkIndex] = swappedExternalLink;
        }

        _keysIndexer[foreignKey] = KeyIndexer.Invalid;
    }

    /// <summary>Returns the structure to its empty state, ready for reuse from the pool. Underlying arrays are retained.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _addedLinkIndex = InvalidIndex;
        _externalLinksCount = 0;
        _keysIndexer.AsSpan().Fill(KeyIndexer.Invalid);
    }
}
