using System;
using System.Collections.Generic;

namespace Theia.ECS.Relations;

internal abstract class RelationType
{
    private const int DefaultRelationPoolCapacity = 4;

    internal readonly Type _type;
    internal readonly RelationCardinality _cardinality;
    protected Queue<Relation> _pool;

    internal RelationType(Type type, RelationCardinality cardinality)
    {
        _type = type;
        _cardinality = cardinality;
        _pool = new(DefaultRelationPoolCapacity);
    }

    internal void Pool(Relation relation) => _pool.Enqueue(relation);

    internal abstract Relation CreateRelation();
}
