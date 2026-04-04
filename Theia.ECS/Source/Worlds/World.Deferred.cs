using System;
using System.Collections.Generic;
using System.Threading;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Relations;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    internal const int DefaultDeferredCommandsCapacity = 256;

    private bool _isFlushingDeferred;

    private Queue<Entity> _deferredGhoulify;
    private readonly Lock _deferredGhoulifyLock = new();

    private Queue<EntityComponentDeferred> _deferredAddComponent;
    private readonly Lock _deferredAddComponentLock = new();
    private ComponentDeferredStorage[] _deferredAddComponentStorages;

    private Queue<EntityComponentDeferred> _deferredRemoveComponent;
    private readonly Lock _deferredRemoveComponentLock = new();

    private Queue<AddRelationDeferred> _deferredAddRelation;
    private readonly Lock _deferredAddRelationLock = new();
    private RelationDeferredStorage[] _deferredAddRelationStorages;

    private Queue<RemoveRelationDeferred> _deferredRemoveRelation;
    private readonly Lock _deferredRemoveRelationLock = new();

    public bool IsFlushingDeferred() => Volatile.Read(ref _isFlushingDeferred);

    public void DeferredGhoulify(Entity entity)
    {
        ThrowIfFlushingDeferred();

        lock (_deferredGhoulifyLock)
        {
            _deferredGhoulify.Enqueue(entity);
        }
    }

    private void DeferredGhoulifyHandler(Entity entity) => AttemptGhoulify(entity);

    private ComponentDeferredStorage<TComponent> GetOrCreateDeferredAddComponentStorage<TComponent>(
        int componentId
    )
        where TComponent : struct
    {
        ComponentDeferredStorage deferredStorage;

        if (componentId > _deferredAddComponentStorages.Length - 1)
            Array.Resize(ref _deferredAddComponentStorages, componentId + 1);

        deferredStorage = _deferredAddComponentStorages[componentId];

        if (deferredStorage is null)
        {
            deferredStorage = new ComponentDeferredStorage<TComponent>(
                DefaultDeferredCommandsCapacity
            );
            _deferredAddComponentStorages[componentId] = deferredStorage;
        }

        return (ComponentDeferredStorage<TComponent>)deferredStorage;
    }

    private RelationDeferredStorage<TRelation> GetOrCreateDeferredAddRelationStorage<TRelation>(
        int relationId
    )
        where TRelation : struct
    {
        RelationDeferredStorage deferredStorage;

        if (relationId > _deferredAddRelationStorages.Length - 1)
            Array.Resize(ref _deferredAddRelationStorages, relationId + 1);

        deferredStorage = _deferredAddRelationStorages[relationId];

        if (deferredStorage is null)
        {
            deferredStorage = new RelationDeferredStorage<TRelation>(
                DefaultDeferredCommandsCapacity
            );
            _deferredAddRelationStorages[relationId] = deferredStorage;
        }

        return (RelationDeferredStorage<TRelation>)deferredStorage;
    }

    public void DeferredAddComponent<TComponent>(Entity entity, in TComponent component = default)
        where TComponent : struct
    {
        ThrowIfFlushingDeferred();

        int componentId = ComponentMeta<TComponent>.s_id;

        lock (_deferredAddComponentLock)
        {
            _deferredAddComponent.Enqueue(
                new EntityComponentDeferred() { _entity = entity, _componentId = componentId }
            );

            ComponentDeferredStorage<TComponent> storage =
                GetOrCreateDeferredAddComponentStorage<TComponent>(componentId);

            storage.EnqueueDeferred(component);
        }
    }

    private void DeferredAddComponentHandler(EntityComponentDeferred entityComponentDeferred)
    {
        Entity entity = entityComponentDeferred._entity;
        int componentId = entityComponentDeferred._componentId;

        ComponentDeferredStorage storage = _deferredAddComponentStorages[componentId];

        EntityReferences entityReferences = AttemptAddComponent(entity, componentId);

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

    public void DeferredRemoveComponent<TComponent>(Entity entity)
        where TComponent : struct
    {
        ThrowIfFlushingDeferred();

        lock (_deferredRemoveComponentLock)
        {
            _deferredRemoveComponent.Enqueue(
                new EntityComponentDeferred()
                {
                    _entity = entity,
                    _componentId = ComponentMeta<TComponent>.s_id,
                }
            );
        }
    }

    private void DeferredRemoveComponentHandler(EntityComponentDeferred entityComponentDeferred) =>
        AttemptRemoveComponent(
            entityComponentDeferred._entity,
            entityComponentDeferred._componentId
        );

    internal AddRelationDeferred GetAddRelationDeferred<TRelation>(
        Entity owner,
        TRelation relation = default
    )
        where TRelation : struct
    {
        int relationId = RelationMeta<TRelation>.s_id;

        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        int evaluatedRelationId = TryStorageDeferredEvaluatedRelation(
            relationId,
            relationType,
            relation
        );

        return new AddRelationDeferred()
        {
            _owner = owner,
            _target = default,
            _relationId = relationId,
            _relationStorageIndex = evaluatedRelationId,
            _isTag = relationType._isTag,
        };
    }

    internal AddRelationDeferred GetAddRelationDeferredTargeted<TRelation>(
        int relationId,
        RelationType relationType,
        Entity owner,
        Entity target,
        TRelation relation = default
    )
        where TRelation : struct
    {
        int evaluatedRelationId = TryStorageDeferredEvaluatedRelation(
            relationId,
            relationType,
            relation
        );

        return new AddRelationDeferred()
        {
            _owner = owner,
            _target = target,
            _relationId = relationId,
            _relationStorageIndex = evaluatedRelationId,
            _isTag = relationType._isTag,
        };
    }

    private int TryStorageDeferredEvaluatedRelation<TRelation>(
        int relationId,
        RelationType relationType,
        TRelation relation
    )
        where TRelation : struct
    {
        if (relationType._isTag)
            return AddRelationDeferred.InvalidRelationStorageIndex;

        RelationDeferredStorage<TRelation> storage =
            GetOrCreateDeferredAddRelationStorage<TRelation>(relationId);

        return storage.AccountDeferred(relation);
    }

    public void DeferredAddRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        ThrowIfFlushingDeferred();

        int relationId = RelationMeta<TRelation>.s_id;
        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        ThrowIfExpectedTagRelation(relationType);

        lock (_deferredAddRelationLock)
        {
            _deferredAddRelation.Enqueue(
                GetAddRelationDeferredTargeted<TRelation>(relationId, relationType, owner, target)
            );
        }
    }

    public void DeferredAddEvaluatedRelation<TRelation>(
        Entity owner,
        Entity target,
        TRelation relation
    )
        where TRelation : struct
    {
        ThrowIfFlushingDeferred();

        int relationId = RelationMeta<TRelation>.s_id;
        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        ThrowIfExpectedEvaluatedRelation(relationType);

        lock (_deferredAddRelationLock)
        {
            _deferredAddRelation.Enqueue(
                GetAddRelationDeferredTargeted(relationId, relationType, owner, target, relation)
            );
        }
    }

    internal void DeferredAddRelationHandler(AddRelationDeferred deferredRelation)
    {
        Entity owner = deferredRelation._owner;
        Entity target = deferredRelation._target;

        RelationAccounted relationAccounted = AttemptAccountRelation(
            deferredRelation._relationId,
            owner,
            target
        );

        if (!relationAccounted._accounted)
            return;

        RelationLinked relationLinked = TryRelate(owner, target, relationAccounted);

        if (relationLinked._linked && !deferredRelation._isTag)
        {
            RelationDeferredStorage storage = _deferredAddRelationStorages[
                deferredRelation._relationId
            ];

            storage.SetWith(
                deferredRelation._relationStorageIndex,
                relationLinked._relation,
                relationLinked._compositeKey
            );
        }
    }

    public void DeferredRemoveRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        ThrowIfFlushingDeferred();

        lock (_deferredRemoveRelationLock)
        {
            _deferredRemoveRelation.Enqueue(
                new RemoveRelationDeferred()
                {
                    _owner = owner,
                    _target = target,
                    _relationId = RelationMeta<TRelation>.s_id,
                }
            );
        }
    }

    private void DeferredRemoveRelationHandler(RemoveRelationDeferred deferredRelation)
    {
        Entity owner = deferredRelation._owner;
        Entity target = deferredRelation._target;

        AttemptRemoveRelation(deferredRelation._relationId, owner, target);
    }

    public void FlushDeferred()
    {
        ThrowIfQueriesExecuting();

        Volatile.Write(ref _isFlushingDeferred, true);

        while (_deferredGhoulify.Count > 0)
            DeferredGhoulifyHandler(_deferredGhoulify.Dequeue());

        while (_deferredAddComponent.Count > 0)
            DeferredAddComponentHandler(_deferredAddComponent.Dequeue());

        while (_deferredRemoveComponent.Count > 0)
            DeferredRemoveComponentHandler(_deferredRemoveComponent.Dequeue());

        while (_deferredAddRelation.Count > 0)
            DeferredAddRelationHandler(_deferredAddRelation.Dequeue());

        while (_deferredRemoveRelation.Count > 0)
            DeferredRemoveRelationHandler(_deferredRemoveRelation.Dequeue());

        Assemblage[] assemblages = _assemblages;

        for (int i = 0; i < assemblages.Length; i++)
            assemblages[i].DeferredCreate();

        Volatile.Write(ref _isFlushingDeferred, false);
    }

    internal void ThrowIfFlushingDeferred()
    {
        if (IsFlushingDeferred())
            throw new InvalidOperationException(
                "Cannot enqueue deferred commands during a deferred flush. Use the non-deferred equivalent instead."
            );
    }
}
