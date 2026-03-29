using System;
using System.Collections.Generic;
using System.Threading;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    internal const int DefaultDeferredCommandsCapacity = 256;

    private bool _isFlushingDeferred;

    private Queue<Entity> _deferredGhoulify;
    private Lock _deferredGhoulifyLock = new();

    private Queue<EntityComponentDeferred> _deferredAdd;
    private Lock _deferredAddLock = new();
    private DeferredStorage[] _deferredAddStorages;

    private Queue<EntityComponentDeferred> _deferredRemove;
    private Lock _deferredRemoveLock = new();

    internal bool IsFlushingDeferred() => Volatile.Read(ref _isFlushingDeferred);

    public void DeferredGhoulify(Entity entity)
    {
        ThrowIfFlushingDeferred();

        lock (_deferredGhoulifyLock)
        {
            _deferredGhoulify.Enqueue(entity);
        }
    }

    private void DeferredGhoulifyHandler(Entity entity) => AttemptGhoulify(entity);

    private DeferredStorage<TComponent> GetOrAddDeferredStorage<TComponent>(int componentId)
        where TComponent : struct
    {
        DeferredStorage deferredStorage;

        if (componentId > _deferredAddStorages.Length - 1)
            Array.Resize(ref _deferredAddStorages, componentId + 1);

        deferredStorage = _deferredAddStorages[componentId];

        if (deferredStorage is null)
        {
            deferredStorage = new DeferredStorage<TComponent>(DefaultDeferredCommandsCapacity);
            _deferredAddStorages[componentId] = deferredStorage;
        }

        return (DeferredStorage<TComponent>)deferredStorage;
    }

    public void DeferredAdd<TComponent>(Entity entity, in TComponent component = default)
        where TComponent : struct
    {
        ThrowIfFlushingDeferred();

        int componentId = ComponentMeta<TComponent>.s_id;

        lock (_deferredAddLock)
        {
            _deferredAdd.Enqueue(
                new EntityComponentDeferred() { _entity = entity, _componentId = componentId }
            );

            DeferredStorage<TComponent> storage = GetOrAddDeferredStorage<TComponent>(componentId);

            storage.EnqueueDeferred(component);
        }
    }

    private void DeferredAddHandler(EntityComponentDeferred entityComponentDeferred)
    {
        Entity entity = entityComponentDeferred._entity;
        int componentId = entityComponentDeferred._componentId;

        DeferredStorage storage = _deferredAddStorages[componentId];

        EntityReferences entityReferences = AttemptAdd(entity, componentId);

        if (entityReferences._valid)
        {
            storage.SetWithNext(
                in entityReferences._entityMeta,
                entityReferences._currentArchetype!
            );

            InvokeOnComponentAdded(
                new EntityModified(
                    entity,
                    in ComponentsMeta.GetComponentType(componentId)._type,
                    in entityReferences._previousArchetype!,
                    in entityReferences._currentArchetype!,
                    componentId
                )
            );
        }
        else
            storage.DiscardNext();
    }

    public void DeferredRemove<TComponent>(Entity entity)
        where TComponent : struct
    {
        ThrowIfFlushingDeferred();

        lock (_deferredRemoveLock)
        {
            _deferredRemove.Enqueue(
                new EntityComponentDeferred()
                {
                    _entity = entity,
                    _componentId = ComponentMeta<TComponent>.s_id,
                }
            );
        }
    }

    private void DeferredRemoveHandler(EntityComponentDeferred entityComponentDeferred) =>
        AttemptRemove(entityComponentDeferred._entity, entityComponentDeferred._componentId);

    public void DeferredAddRelation<TRelation>(Entity relationOwner, Entity target)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public void DeferredAddEvaluatedRelation<TRelation>(
        Entity relationOwner,
        Entity target,
        TRelation data = default
    )
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public void DeferredRemoveRelation<TRelation>(Entity relationOwner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public void DeferredRemoveRelation<TRelation>(Entity relationOwner, Entity target)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public void FlushDeferred()
    {
        ThrowIfQueriesExecuting();

        _isFlushingDeferred = true;

        while (_deferredGhoulify.Count > 0)
            DeferredGhoulifyHandler(_deferredGhoulify.Dequeue());

        while (_deferredAdd.Count > 0)
            DeferredAddHandler(_deferredAdd.Dequeue());

        while (_deferredRemove.Count > 0)
            DeferredRemoveHandler(_deferredRemove.Dequeue());

        Assemblage[] assemblages = _assemblages;

        for (int i = 0; i < assemblages.Length; i++)
            assemblages[i].DeferredCreate();

        _isFlushingDeferred = false;
    }

    internal void ThrowIfFlushingDeferred()
    {
        if (IsFlushingDeferred())
            throw new InvalidOperationException(
                "Cannot perform structural changes or entity modifications during a deferred flush. Enqueue the operation as a deferred command or wait until the flush completes."
            );
    }
}
