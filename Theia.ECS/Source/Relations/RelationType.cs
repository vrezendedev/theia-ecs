using System;
using System.Collections.Generic;
using System.Threading;
using Theia.ECS.Reflection;

namespace Theia.ECS.Relations;

/// <summary>
/// Non-generic base for relation type metadata. Holds the runtime <see cref="Type"/>, a
/// cached name, the tag flag, and the pools used to recycle <see cref="Relation"/>
/// and <see cref="RelationLink"/> instances for this relation type.
/// </summary>
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

    /// <summary>
    /// Parameterless constructor used by <see cref="Activator.CreateInstance(Type)"/> on the
    /// <see cref="RelationsMeta.AttemptRegisterRelation(string)"/> reflection path, where
    /// fields are populated immediately afterwards via <see cref="Initialize"/>.
    /// </summary>
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

    /// <summary>
    /// Populates the type and tag fields. Used by <see cref="RelationsMeta.AttemptRegisterRelation(string)"/>.
    /// </summary>
    public void Initialize(Type type, bool isTag)
    {
        _type = type;
        _isTag = isTag;
    }

    /// <summary>
    /// Returns a <see cref="Relation"/> instance to the pool for later reuse. Pairs with the
    /// dequeue performed by <see cref="RelationType{T}"/>'s <see cref="CreateRelation"/> override.
    /// </summary>
    internal void PoolRelation(Relation relation)
    {
        lock (_relationPoolLock)
        {
            _relationPool.Enqueue(relation);
        }
    }

    /// <summary>
    /// Returns a <see cref="RelationLink"/> instance to the pool for later reuse. Pairs with
    /// the dequeue performed by <see cref="CreateRelationLink"/>.
    /// </summary>
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

    /// <summary>
    /// Creates or recycles a <see cref="Relation"/> for this relation type. Implemented by
    /// <see cref="RelationType{T}"/>, which owns the type-parameterized construction.
    /// </summary>
    internal abstract Relation CreateRelation();

    /// <summary>
    /// Returns a fresh or recycled <see cref="RelationLink"/>. If the pool has an available
    /// instance it is reset and returned; otherwise a new one is allocated.
    /// </summary>
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
