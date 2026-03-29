using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

internal sealed class RelationsIndexer
{
    private const int DefaultRelationAddedCapacity = 4;
    private const int DefaultRelationAddedGrowthFactor = 2;

    private int _relationsAddedCount;
    private int[] _relationsAdded;

    private RelationKey[] _keys;
    private readonly Lock _lock = new();

    internal RelationsIndexer()
    {
        _relationsAdded = new int[DefaultRelationAddedCapacity];
        _keys = Array.Empty<RelationKey>();
    }

    internal RelationKey GetOrAddKey(int relationId)
    {
        RelationKey[] keys = _keys;

        if (HasKey(relationId))
            return keys[relationId];

        lock (_lock)
        {
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
    }

    internal void DeleteKey(int relationId)
    {
        RelationKey key;

        lock (_lock)
        {
            RelationKey[] keys = _keys;
            int[] relationsAdded = _relationsAdded;

            key = keys[relationId];

            int relationAddedIndex = key._indexerAddedIndex;

            int last = _relationsAddedCount - 1;

            _relationsAddedCount--;

            if (relationAddedIndex < last)
            {
                int lastRelationAddedId = relationsAdded[last];
                relationsAdded[relationAddedIndex] = lastRelationAddedId;
                keys[lastRelationAddedId]._indexerAddedIndex = relationAddedIndex;
            }

            keys[relationId] = null!;
        }

        RelationsMeta.GetRelationType(relationId).PoolRelationKey(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasKey(int relationId)
    {
        RelationKey[] keys = _keys;
        return relationId < keys.Length && keys[relationId] is not null;
    }

    internal void Reset()
    {
        while (_relationsAddedCount > 0)
            DeleteKey(_relationsAdded[_relationsAddedCount - 1]);
    }
}
