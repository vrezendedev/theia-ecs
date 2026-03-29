using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Entities;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

internal abstract class RelationKey
{
    protected const int InvalidKey = -1;

    internal int _indexerAddedIndex;
    internal int _primaryKey;

    internal abstract void Reset();
    internal abstract bool IsAvailable();
}

internal abstract class MultiLinkRelation : RelationKey
{
    private const int DefaultRelationsIndexerCapacity = 4;
    private const int DefaultRelationsIndexerGrowthFactor = 2;

    internal int _externalLinksWithCount;
    internal Entity[] _externalLinksWith = new Entity[DefaultRelationsIndexerCapacity];

    protected int AccountExternalLink(Entity entity)
    {
        int index = _externalLinksWithCount;

        Array.AttemptResize(ref _externalLinksWith, index, DefaultRelationsIndexerGrowthFactor);

        _externalLinksWith[index] = entity;

        _externalLinksWithCount++;

        return index;
    }

    internal override void Reset() => _externalLinksWithCount = 0;
}

internal record struct ExclusiveKeyIndexer
{
    internal required int ExternalLinkIndex;
}

internal sealed class ExclusiveKey : MultiLinkRelation
{
    internal ExclusiveKeyIndexer[] _keysIndexer = Array.Empty<ExclusiveKeyIndexer>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddKeyIndexer(Entity entity, int foreignKey)
    {
        int externalLinkIndex = AccountExternalLink(entity);

        if (foreignKey >= _keysIndexer.Length)
            Array.Resize(ref _keysIndexer, foreignKey + 1);

        _keysIndexer[foreignKey].ExternalLinkIndex = externalLinkIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override bool IsAvailable() => _primaryKey == InvalidKey;

    internal override void Reset()
    {
        base.Reset();

        _primaryKey = InvalidKey;
    }
}

internal sealed class TreeKey : RelationKey
{
    internal int _foreignKey;
    internal int _compositeKey;

    internal override bool IsAvailable()
    {
        throw new NotImplementedException();
    }

    internal override void Reset()
    {
        _primaryKey = InvalidKey;
        _foreignKey = InvalidKey;
        _compositeKey = InvalidKey;
    }
}

internal record struct MultipleKeyIndexer(int ExternalLinkIndex, int CompositeKey);

internal sealed class MultipleKey : MultiLinkRelation
{
    internal MultipleKeyIndexer[] _keysIndexer = Array.Empty<MultipleKeyIndexer>();

    internal override bool IsAvailable()
    {
        throw new NotImplementedException();
    }

    internal override void Reset()
    {
        base.Reset();

        _primaryKey = InvalidKey;
    }
}
