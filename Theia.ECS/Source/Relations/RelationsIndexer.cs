using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

internal sealed class RelationsIndexer
{
    private const int DefaultRelationAddedCapacity = 4;
    private const int DefaultRelationAddedGrowthFactor = 2;

    internal readonly Lock _lock = new();

    internal int _relationsAddedCount { get; private set; }
    private int[] _relationsAdded;

    private RelationKey[] _keys;

    internal RelationsIndexer()
    {
        _relationsAdded = new int[DefaultRelationAddedCapacity];
        _keys = Array.Empty<RelationKey>();
    }

    internal RelationKey GetOrAddKey(int relationId)
    {
        if (HasKey(relationId))
            return _keys[relationId];

        int addedIndex = _relationsAddedCount;

        Array.AttemptResize(ref _relationsAdded, addedIndex, DefaultRelationAddedGrowthFactor);

        _relationsAddedCount++;

        _relationsAdded[addedIndex] = relationId;

        RelationKey relationKey = RelationsMeta.GetRelationType(relationId).CreateKey();

        relationKey._indexerAddedIndex = addedIndex;

        if (relationId >= _keys.Length)
            Array.Resize(ref _keys, relationId + 1);

        _keys[relationId] = relationKey;

        return relationKey;
    }

    internal RelationKey DeleteKey(int relationId)
    {
        RelationKey key = _keys[relationId];

        int relationAddedIndex = key._indexerAddedIndex;

        int last = _relationsAddedCount - 1;

        _relationsAddedCount--;

        if (relationAddedIndex < last)
        {
            int lastRelationAddedId = _relationsAdded[last];
            _relationsAdded[relationAddedIndex] = lastRelationAddedId;
            _keys[lastRelationAddedId]._indexerAddedIndex = relationAddedIndex;
        }

        _keys[relationId] = null!;

        return key;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasKey(int relationId)
    {
        RelationKey[] keys = _keys;
        return relationId < keys.Length && keys[relationId] is not null;
    }
}
