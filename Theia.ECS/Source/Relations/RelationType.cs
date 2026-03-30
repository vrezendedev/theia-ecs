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

    protected readonly Lock _relationPoolLock = new();
    protected Queue<Relation> _relationPool;
    protected readonly Lock _relationKeyPoolLock = new();
    protected Queue<RelationKey> _relationKeyPool;

    internal RelationType(Type type, RelationCardinality cardinality, RelationSubtype subtype)
    {
        _type = type;
        _cardinality = cardinality;
        _subtype = subtype;
        _relationPool = new(DefaultRelationPoolCapacity);
        _relationKeyPool = new(DefaultRelationPoolCapacity);
    }

    internal void PoolRelation(Relation relation)
    {
        lock (_relationPoolLock)
        {
            _relationPool.Enqueue(relation);
        }
    }

    internal void PoolRelationKey(RelationKey relationKey)
    {
        lock (_relationKeyPoolLock)
        {
            _relationKeyPool.Enqueue(relationKey);
        }
    }

    public Type Get() => _type;

    public static int Count() => RelationsMeta.Count();

    public static int GetId(Type type) => RelationsMeta.GetRelationId(type);

    internal abstract Relation CreateRelation();
    internal abstract RelationKey CreateKey();
}
