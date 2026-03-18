using System;
using System.Collections.Generic;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private bool _isFlushingDeferred;
    private Queue<Entity> _deferredGhoulify;
    private Queue<EntityComponentDeferred> _deferredAdd;
    private Dictionary<int, DeferredStorage> _deferredAddStorages;
    private Queue<EntityComponentDeferred> _deferredRemove;

    public void DeferredGhoulify(Entity entity)
    {
        ThrowIfFlushingDeferred();

        lock (_deferredGhoulify)
        {
            _deferredGhoulify.Enqueue(entity);
        }
    }

    private void DeferredGhoulifyHandler(Entity entity) => AttemptGhoulify(entity);

    public void DeferredAdd<T>(Entity entity, in T component = default)
        where T : struct
    {
        ThrowIfFlushingDeferred();

        int componentId = ComponentMeta<T>.s_id;

        lock (_deferredAdd)
        {
            _deferredAdd.Enqueue(
                new EntityComponentDeferred() { _entity = entity, _componentId = componentId }
            );

            if (!_deferredAddStorages.TryGetValue(componentId, out DeferredStorage? storage))
            {
                storage = new DeferredStorage<T>(DefaultDeferredCommandsCapacity);
                _deferredAddStorages.Add(componentId, storage);
            }

            ((DeferredStorage<T>)storage).EnqueueDeferred(component);
        }
    }

    private void DeferredAddHandler(EntityComponentDeferred entityComponentDeferred)
    {
        DeferredStorage storage = _deferredAddStorages[entityComponentDeferred._componentId];

        EntityReferences entityReferences = AttemptAdd(
            entityComponentDeferred._entity,
            entityComponentDeferred._componentId
        );

        if (entityReferences._valid)
            storage.SetWithNext(in entityReferences._entityMeta, entityReferences._archetype!);
        else
            storage.DiscardNext();
    }

    public void DeferredRemove<T>(Entity entity)
        where T : struct
    {
        ThrowIfFlushingDeferred();

        lock (_deferredRemove)
        {
            _deferredRemove.Enqueue(
                new EntityComponentDeferred()
                {
                    _entity = entity,
                    _componentId = ComponentMeta<T>.s_id,
                }
            );
        }
    }

    private void DeferredRemoveHandler(EntityComponentDeferred entityComponentDeferred) =>
        AttemptRemove(entityComponentDeferred._entity, entityComponentDeferred._componentId);

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
        if (_isFlushingDeferred)
            throw new InvalidOperationException(
                "Deferred operations cannot be enqueued during a flush. Use synchronous operations instead."
            );
    }
}
