using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

/// <summary>
/// Per-entity bookkeeping for the relations the entity participates in, on either side of the
/// link. Owns two parallel sparse-set structures: one keyed by relation ID for relations the
/// entity owns (<see cref="RelationKey"/>), and one keyed by relation ID for relations whose
/// targets the entity participates in (<see cref="RelationLink"/>).
/// </summary>
/// <remarks>
/// <para>
/// This is the per-entity hub that ties the bilateral relation representation together: the
/// owner-side <see cref="Relation"/> is reachable through <see cref="GetRelationKey"/> +
/// <see cref="RelationStorage"/>, and the target-side <see cref="RelationLink"/> is reachable
/// through <see cref="GetRelationLink"/>. Adding a link updates both sides; removing a link
/// unwinds both sides.
/// </para>
/// </remarks>
internal sealed class RelationsIndexer
{
    private const int DefaultAddedCapacity = 4;
    private const int DefaultAddedGrowthFactor = 2;

    internal readonly Lock _lock = new();

    private int _addedRelationsIdCount;
    private int[] _addedRelationsId;
    private RelationKey[] _keys;

    private int _addedLinksCount;
    private int[] _addedLinks;
    private RelationLink[] _links;

    internal RelationsIndexer()
    {
        _addedRelationsId = new int[DefaultAddedCapacity];
        _keys = Array.Empty<RelationKey>();
        _addedLinks = new int[DefaultAddedCapacity];
        _links = Array.Empty<RelationLink>();
    }

    /// <summary>Returns <see langword="true"/> if this entity owns at least one relation under <paramref name="relationId"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasKey(int relationId) =>
        relationId < _keys.Length && _keys[relationId].HasRelation();

    /// <summary>Returns <see langword="true"/> if this entity is a target of at least one relation under <paramref name="relationId"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasLink(int relationId) =>
        relationId < _links.Length && _links[relationId] is not null;

    /// <summary>Returns the number of distinct relation types this entity is a target of.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetAddedLinksCount() => _addedLinksCount;

    /// <summary>Returns the relation ID at position <paramref name="index"/> in the dense added-links list.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetAddedLinksAt(int index) => _addedLinks[index];

    /// <summary>Returns the <see cref="RelationLink"/> for <paramref name="relationId"/>; the caller must have already verified presence via <see cref="HasLink"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal RelationLink GetRelationLink(int relationId) => _links[relationId];

    /// <summary>
    /// Returns the existing <see cref="RelationLink"/> for <paramref name="relationId"/> if present,
    /// otherwise rents a fresh one from the relation type's pool and registers it in the dense
    /// added-links list. Grows the sparse <c>_links</c> array if needed.
    /// </summary>
    internal RelationLink GetOrRentLink(int relationId)
    {
        if (HasLink(relationId))
            return _links[relationId];

        int addedIndex = _addedLinksCount;

        Array.TryResize(ref _addedLinks, addedIndex, DefaultAddedGrowthFactor);

        _addedLinksCount++;
        _addedLinks[addedIndex] = relationId;

        RelationLink relationLink = RelationsMeta.GetRelationType(relationId).CreateRelationLink();

        if (relationId >= _links.Length)
            Array.Resize(ref _links, relationId + 1);

        relationLink.SetIndexerAddedLinkIndex(addedIndex);

        _links[relationId] = relationLink;

        return relationLink;
    }

    /// <summary>
    /// Returns the <see cref="RelationLink"/> for <paramref name="relationId"/> to the relation
    /// type's pool and removes it from the dense added-links list in O(1) via swap-with-last.
    /// </summary>
    internal void ReturnLink(int relationId)
    {
        RelationLink relationLink = _links[relationId];

        int addedLinkIndex = relationLink.GetIndexerAddedLinkIndex();

        int last = _addedLinksCount - 1;

        _addedLinksCount--;

        if (addedLinkIndex < last)
        {
            int lastAddedLinkId = _addedLinks[last];
            _addedLinks[addedLinkIndex] = lastAddedLinkId;
            _links[lastAddedLinkId].SetIndexerAddedLinkIndex(addedLinkIndex);
        }

        _links[relationId] = null!;

        RelationsMeta.GetRelationType(relationId).PoolRelationLink(relationLink);
    }

    /// <summary>Returns the number of distinct relation types this entity owns.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetAddedRelationsCount() => _addedRelationsIdCount;

    /// <summary>Returns the relation ID at position <paramref name="index"/> in the dense added-relations list.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetAddedRelationsAt(int index) => _addedRelationsId[index];

    /// <summary>
    /// Returns a reference to the <see cref="RelationKey"/> stored at <paramref name="relationId"/>.
    /// Callers reading the primary key should pair this with <see cref="HasKey"/> first.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref RelationKey GetRelationKey(int relationId) => ref _keys[relationId];

    /// <summary>
    /// Records that this entity owns a relation under <paramref name="relationId"/> at the
    /// <see cref="RelationStorage"/> slot identified by <paramref name="primaryKey"/>. Adds the
    /// relation ID to the dense added-relations list.
    /// </summary>
    internal void AddKey(int relationId, int primaryKey)
    {
        int addedIndex = _addedRelationsIdCount;

        Array.TryResize(ref _addedRelationsId, addedIndex, DefaultAddedGrowthFactor);

        _addedRelationsIdCount++;

        _addedRelationsId[addedIndex] = relationId;

        if (relationId >= _keys.Length)
        {
            int length = _keys.Length;
            Array.Resize(ref _keys, relationId + 1);
            _keys.AsSpan(length).Fill(new RelationKey());
        }

        RelationKey relationKey = new();
        relationKey.SetPrimaryKey(primaryKey);
        relationKey.SetIndexerAddedRelationIdIndex(addedIndex);

        _keys[relationId] = relationKey;
    }

    /// <summary>
    /// Removes this entity's record of owning a relation under <paramref name="relationId"/>
    /// from the dense added-relations list in O(1) via swap-with-last, and resets the keyed
    /// entry to its empty state.
    /// </summary>
    internal void RemoveRelationKey(int relationId)
    {
        ref RelationKey relationKey = ref _keys[relationId];

        int addedRelationIdIndex = relationKey.GetIndexerAddedRelationIdIndex();

        int last = _addedRelationsIdCount - 1;

        _addedRelationsIdCount--;

        if (addedRelationIdIndex < last)
        {
            int lastAddedRelationId = _addedRelationsId[last];
            _addedRelationsId[addedRelationIdIndex] = lastAddedRelationId;
            _keys[lastAddedRelationId].SetIndexerAddedRelationIdIndex(addedRelationIdIndex);
        }

        relationKey.Reset();
    }
}
