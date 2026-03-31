using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Entities;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

public record struct ExternalLink
{
    public required Entity Entity { get; init; }
    internal int ForeignKey { get; init; }

    public static implicit operator Entity(ExternalLink e) => e.Entity;
}

internal record struct KeyIndexer
{
    internal required int ExternalLinkIndex { get; init; }
    internal required int CompositeKey { get; init; }

    private static KeyIndexer _invalid = new KeyIndexer()
    {
        ExternalLinkIndex = RelationLink.InvalidIndex,
        CompositeKey = RelationLink.InvalidIndex,
    };

    internal static KeyIndexer Invalid => _invalid;
}

internal sealed class RelationLink
{
    internal const int InvalidIndex = -1;
    private const int DefaultRelationsIndexerCapacity = 4;
    private const int DefaultRelationsIndexerGrowthFactor = 2;

    internal readonly Lock _lock = new();

    private int _externalLinksCount;
    private ExternalLink[] _externalLinks;
    private KeyIndexer[] _keysIndexer;

    internal RelationLink()
    {
        _keysIndexer = Array.Empty<KeyIndexer>();
        _externalLinks = new ExternalLink[DefaultRelationsIndexerCapacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<ExternalLink> GetExternalLinks() =>
        _externalLinks.AsSpan(0, _externalLinksCount);

    internal void AddExternalLink(Entity entity, int foreignKey, int compositeKey)
    {
        int externalLinkIndex = _externalLinksCount;

        Array.AttemptResize(
            ref _externalLinks,
            externalLinkIndex,
            DefaultRelationsIndexerGrowthFactor
        );

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

    internal void RemovalExternalLink(int foreignKey)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _externalLinksCount = 0;
        _keysIndexer.AsSpan().Fill(KeyIndexer.Invalid);
    }
}
