using System;
using System.Collections.Generic;
using System.Threading;
using Theia.ECS.Reflection;

namespace Theia.ECS.Relations;

internal abstract class RelationType : TypeMeta
{
    private const int DefaultRelationPoolCapacity = 4;

    internal readonly RelationCardinality _cardinality;
    internal readonly RelationSubtype _subtype;

    protected Queue<Relation> _pool;
    protected Lock _poolLock = new();

    internal RelationType(Type type, RelationCardinality cardinality, RelationSubtype subtype)
        : base(type)
    {
        _cardinality = cardinality;
        _subtype = subtype;
        _pool = new(DefaultRelationPoolCapacity);
    }

    internal void Pool(Relation relation)
    {
        lock (_poolLock)
        {
            _pool.Enqueue(relation);
        }
    }

    internal abstract Relation CreateRelation();
}
