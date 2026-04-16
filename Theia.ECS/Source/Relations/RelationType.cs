using System;
using System.Collections.Generic;
using System.Threading;
using Theia.ECS.Reflection;

namespace Theia.ECS.Relations;

internal abstract class RelationType : ITypeMeta
{
    private const int DefaultRelationPoolCapacity = 4;
    private const int DefaultRelationLinkPoolCapacity = 16;

    internal Type _type;
    internal string? _name;
    internal bool _isTag;

    protected readonly Lock _relationPoolLock = new();
    protected Queue<Relation> _relationPool;
    protected readonly Lock _relationLinkPoolLock = new();
    protected Queue<RelationLink> _relationLinkPool;

#pragma warning disable CS8618
    public RelationType()
    {
        _relationPool = new(DefaultRelationPoolCapacity);
        _relationLinkPool = new(DefaultRelationLinkPoolCapacity);
    }
#pragma warning restore CS8618

    internal RelationType(Type type, bool isTag)
    {
        _type = type;
        _isTag = isTag;
        _relationPool = new(DefaultRelationPoolCapacity);
        _relationLinkPool = new(DefaultRelationLinkPoolCapacity);
    }

    public void Initialize(Type type, bool isTag)
    {
        _type = type;
        _isTag = isTag;
    }

    internal void PoolRelation(Relation relation)
    {
        lock (_relationPoolLock)
        {
            _relationPool.Enqueue(relation);
        }
    }

    internal void PoolRelationLink(RelationLink relationKey)
    {
        lock (_relationLinkPoolLock)
        {
            _relationLinkPool.Enqueue(relationKey);
        }
    }

    public Type Get() => _type;

    public void SetTypeName(string name) => _name = name;

    public string GetTypeName() => _name!;

    public static int Count() => RelationsMeta.Count();

    public static int GetId(Type type) => RelationsMeta.GetRelationId(type);

    internal abstract Relation CreateRelation();

    internal RelationLink CreateRelationLink()
    {
        RelationLink? relationLink;

        lock (_relationLinkPoolLock)
        {
            _relationLinkPool.TryDequeue(out relationLink);
        }

        if (relationLink is null)
            relationLink = new();
        else
            relationLink.Reset();

        return relationLink;
    }
}
