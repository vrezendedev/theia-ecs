using System;
using System.Collections.Generic;

namespace Theia.ECS.Relations;

internal sealed class RelationStorage
{
    private const int DefaultRelationsCapacity = 8;
    private const int DefaultRelationsGrowthFactor = 2;

    internal readonly int _relationId;

    private int _count;
    private Relation[] _relations;
    private Queue<int> _free;

    internal RelationStorage(int relationId)
    {
        _relationId = relationId;

        _relations = new Relation[DefaultRelationsCapacity];

        _free = new(DefaultRelationsCapacity);

        for (int i = 0; i < DefaultRelationsCapacity; i++)
            _free.Enqueue(i);
    }

    internal int AccountRelation()
    {
        throw new NotImplementedException();
    }

    internal Relation GetRelation(int primaryKey)
    {
        throw new NotImplementedException();
    }
}
