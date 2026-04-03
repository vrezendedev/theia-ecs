using System;
using System.Collections.Generic;
using Theia.ECS.Archetypes;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

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

        InvokeOnEntityCreated(new EntityAssembled(entityCreated._entity, in _archetype));

        return entityCreated;
    }

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

    internal override void DeferredCreate()
    {
        while (_deferredCreate.Count > 0)
        {
            EntityCreateDeferred<ComponentT1> deferredCreate = _deferredCreate.Dequeue();

            EntityCreated entityCreated = CreateAndSet(deferredCreate._componentT1);

            _world.DeferredAddRelationHandler(
                deferredCreate._relationDeferred with
                {
                    _target = entityCreated._entity,
                }
            );
        }
    }
}
