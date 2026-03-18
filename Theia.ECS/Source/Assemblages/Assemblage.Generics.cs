using System;
using System.Collections.Generic;
using Theia.ECS.Archetypes;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

public class Assemblage<T1> : Assemblage
    where T1 : struct
{
    private Queue<EntityCreateDeferred<T1>> _deferredCreate;

    internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
        : base(world, archetype, componentStorageMapping) =>
        _deferredCreate = new(World.DefaultDeferredCommandsCapacity);

    public Entity TryCreate(in T1 componentT1)
    {
        _world.ThrowIfQueriesExecuting();

        return Create(in componentT1)._entity;
    }

    internal EntityCreated Create(in T1 componentT1)
    {
        EntityCreated entityCreated = _world.CreateEntity(_archetype);

        int[] mapping = _componentStorageMapping;

        _archetype.Set(mapping[0], in entityCreated._entityMeta, in componentT1);

        return entityCreated;
    }

    public void DeferredCreate(in T1 componentT1)
    {
        _world.ThrowIfFlushingDeferred();

        lock (_deferredCreate)
        {
            _deferredCreate.Enqueue(new EntityCreateDeferred<T1>() { _componentT1 = componentT1 });
        }
    }

    internal override void DeferredCreate()
    {
        while (_deferredCreate.Count > 0)
            Create(_deferredCreate.Dequeue()._componentT1);
    }
}
