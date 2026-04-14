using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Contracts;

namespace Theia.ECS.Relations;

internal sealed class RelationStorage
{
    private const int DefaultRelationsCapacity = 8;
    private const int DefaultRelationsGrowthFactor = 2;

    internal readonly Lock _lock = new();
    internal readonly int _relationId;

    private Relation[] _relations;
    private Queue<int> _free;

    internal int CountStorageSlotsOccupied() => _relations.Length - _free.Count;

    internal RelationStorage(int relationId)
    {
        _relationId = relationId;

        _relations = new Relation[DefaultRelationsCapacity];

        _free = new(DefaultRelationsCapacity);

        for (int i = 0; i < DefaultRelationsCapacity; i++)
            _free.Enqueue(i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Relation GetRelation(int primaryKey)
    {
        Relation[] relations = _relations;
        return relations[primaryKey];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Relation[] GetRelations() => _relations;

    internal RelationKeyed RentRelation()
    {
        int index;

        if (_free.Count > 0)
            index = _free.Dequeue();
        else
        {
            index = _relations.Length;

            int targetLength = index * DefaultRelationsGrowthFactor;

            Array.Resize(ref _relations, targetLength);

            for (int i = index + 1; i < targetLength; i++)
                _free.Enqueue(i);
        }

        _relations[index] = RelationsMeta.GetRelationType(_relationId).CreateRelation();

        return new RelationKeyed(_relations[index], index);
    }

    internal void ReturnRelation(int primaryKey)
    {
        Relation relation = _relations[primaryKey];

        RelationsMeta.GetRelationType(_relationId).PoolRelation(relation);

        _relations[primaryKey] = null!;

        _free.Enqueue(primaryKey);
    }
}
