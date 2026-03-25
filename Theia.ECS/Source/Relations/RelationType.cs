using System;
using System.Collections.Generic;
using System.Threading;
using Theia.ECS.Reflection;

namespace Theia.ECS.Relations;

internal abstract class RelationType : ITypeMeta
{
    private const int DefaultRelationPoolCapacity = 4;

    internal readonly Type _type;
    internal readonly RelationCardinality _cardinality;
    internal readonly RelationSubtype _subtype;

    protected Queue<Relation> _pool;
    protected Lock _poolLock = new();

    internal RelationType(Type type, RelationCardinality cardinality, RelationSubtype subtype)
    {
        _type = type;
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

    public Type Get() => _type;

    public static int Count() => RelationsMeta.Count();

    public static int GetId(Type type) => RelationsMeta.GetRelationId(type);

    internal abstract Relation CreateRelation();
}
