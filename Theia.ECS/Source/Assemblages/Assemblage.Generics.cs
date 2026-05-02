using System;
using System.Collections.Generic;
using Theia.ECS.Archetypes;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

/// <summary>
/// Single-component assemblage. Creates entities bound to an archetype carrying exactly
/// <typeparamref name="ComponentT1"/>, with both immediate and deferred creation paths and an
/// optional "create as the target of a pending relation" hook.
/// </summary>
public class Assemblage<ComponentT1> : Assemblage
    where ComponentT1 : struct
{
    private Queue<EntityCreateDeferred<ComponentT1>> _deferredCreate;

    internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
        : base(world, archetype, componentStorageMapping) =>
        _deferredCreate = new(World.DefaultDeferredCommandsCapacity);

    /// <summary>
    /// Creates an entity with <paramref name="componentT1"/> immediately and returns its handle.
    /// Throws if a query is currently iterating, since immediate creation is a structural change.
    /// </summary>
    public Entity Create(in ComponentT1 componentT1)
    {
        _world.ThrowIfQueriesExecuting();

        return CreateAndSet(in componentT1)._entity;
    }

    internal EntityCreated CreateAndSet(in ComponentT1 componentT1)
    {
        EntityCreated entityCreated = _world.CreateEntity(_archetype);

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        _archetype.Set(mapping[0], in entityCreated._entityMeta, in componentT1);

        InvokeOnEntityCreated(new EntityAssembled(_world, entityCreated._entity, in _archetype));

        return entityCreated;
    }

    /// <summary>
    /// Queues an entity creation with <paramref name="componentT1"/> for materialization at the
    /// next deferred-flush point. Safe to call from inside system or query execution; the actual entity
    /// is created (and <see cref="EntitiesEvents.OnCreated">OnCreated</see> fires) only when the world flushes.
    /// </summary>
    public void DeferredCreate(in ComponentT1 componentT1)
    {
        _world.ThrowIfFlushingDeferred();

        lock (_deferredCreateLock)
        {
            _deferredCreate.Enqueue(
                new EntityCreateDeferred<ComponentT1>() { _componentT1 = componentT1 }
            );
        }
    }

    /// <summary>
    /// Queues an entity creation with <paramref name="componentT1"/> and an additional relation:
    /// <paramref name="deferredRelationOnCreate"/>'s <c>Owner</c> will gain a relation of
    /// type <typeparamref name="TRelation"/> targeting the newly created entity. Both the entity
    /// creation and the relation are materialized atomically at the next deferred-flush point.
    /// </summary>
    /// <typeparam name="TRelation">The relation type to add.</typeparam>
    /// <param name="componentT1">The component payload for the new entity.</param>
    /// <param name="deferredRelationOnCreate">
    /// Carries the existing owner entity and the relation payload. The target is patched in
    /// during flush once the new entity has a handle.
    /// </param>
    /// <remarks>
    /// Useful when an existing entity needs to immediately reference a newly spawned one.
    /// </remarks>
    public void DeferredCreate<TRelation>(
        in ComponentT1 componentT1,
        DeferredRelationOnCreate<TRelation> deferredRelationOnCreate
    )
        where TRelation : struct
    {
        _world.ThrowIfFlushingDeferred();

        lock (_deferredCreateLock)
        {
            _deferredCreate.Enqueue(
                new EntityCreateDeferred<ComponentT1>()
                {
                    _componentT1 = componentT1,
                    _relationDeferred = _world.GetAddRelationDeferred(
                        deferredRelationOnCreate.Owner,
                        deferredRelationOnCreate.Relation
                    ),
                }
            );
        }
    }

    /// <inheritdoc/>
    internal override void DeferredCreate()
    {
        while (_deferredCreate.Count > 0)
        {
            EntityCreateDeferred<ComponentT1> deferredCreate = _deferredCreate.Dequeue();

            EntityCreated entityCreated = CreateAndSet(deferredCreate._componentT1);

            if (
                deferredCreate._relationDeferred._relationId
                == AddRelationDeferred.InvalidRelationId
            )
                continue;

            _world.DeferredAddRelationHandler(
                deferredCreate._relationDeferred with
                {
                    _target = entityCreated._entity,
                }
            );
        }
    }
}
