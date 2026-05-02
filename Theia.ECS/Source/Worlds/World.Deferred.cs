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
    /// <summary>Initial capacity for every deferred-command queue and value storage created on the world.</summary>
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

    /// <summary>
    /// Returns <see langword="true"/> while <see cref="FlushDeferred"/> is draining the
    /// deferred queues. Use this from <b>inside event handlers to detect whether a structural
    /// change is being applied through the deferred path</b>.
    /// </summary>
    public bool IsFlushingDeferred() => Volatile.Read(ref _isFlushingDeferred);

    /// <summary>
    /// Queues <paramref name="entity"/> for ghoulification at the next <see cref="FlushDeferred"/>.
    /// Safe to call from inside query execution; the entity is not actually destroyed until the flush.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if called while a flush is already in progress.</exception>
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

    /// <summary>
    /// Queues "add <typeparamref name="TComponent"/> to <paramref name="entity"/>" for the next
    /// flush.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if called while a flush is already in progress.</exception>
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
                    this,
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

    /// <summary>
    /// Queues "remove <typeparamref name="TComponent"/> from <paramref name="entity"/>" for the
    /// next flush.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if called while a flush is already in progress.</exception>
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

    /// <summary>
    /// Pre-resolves the metadata needed to add <typeparamref name="TRelation"/> to
    /// <paramref name="owner"/> and stages the relation payload in
    /// <see cref="RelationDeferredStorage{TRelation}"/> when applicable. Used by assemblages to
    /// build a fully-resolved <see cref="AddRelationDeferred"/> at queue time, leaving only the
    /// target field to be patched in once the new entity exists.
    /// </summary>
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

    /// <summary>
    /// Variant of <see cref="GetAddRelationDeferred"/> that also captures
    /// <paramref name="target"/> at queue time, for callers that already know both endpoints.
    /// </summary>
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

    /// <summary>
    /// Queues a <b>tag-relation</b> add: links <paramref name="owner"/> to <paramref name="target"/>
    /// under <typeparamref name="TRelation"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a flush is in progress, or if <typeparamref name="TRelation"/> carries data,
    /// use <see cref="DeferredAddEvaluatedRelation"/>.
    /// </exception>
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

    /// <summary>
    /// Queues a <b>data-relation</b> add: links <paramref name="owner"/> to <paramref name="target"/>
    /// under <typeparamref name="TRelation"/> with <paramref name="relation"/> as the per-link
    /// payload.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a flush is in progress, or if <typeparamref name="TRelation"/> is a tag
    /// use <see cref="DeferredAddRelation"/>.
    /// </exception>
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

        RelationType relationType = RelationsMeta.GetRelationType(deferredRelation._relationId);

        RelationsEvents.InvokeOnRelationAdded(
            new RelationModified(
                this,
                owner,
                target,
                relationType._type,
                deferredRelation._relationId
            )
        );
    }

    /// <summary>
    /// Queues "remove the <typeparamref name="TRelation"/> link from <paramref name="owner"/> to
    /// <paramref name="target"/>" for the next flush.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if called while a flush is already in progress.</exception>
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

    /// <summary>
    /// Drains every deferred queue in a <b>fixed order</b>:
    /// <list type="number">
    /// <item><description>Ghoulifications;</description></item>
    /// <item><description>Component Adds;</description></item>
    /// <item><description>Component Removes;</description></item>
    /// <item><description>Relation Adds;</description></item>
    /// <item><description>Relation Removes;</description></item>
    /// <item><description>Per-assemblage entity creations.</description></item>
    /// </list>
    /// While draining, <see cref="IsFlushingDeferred"/> returns <see langword="true"/> and any
    /// further deferred-enqueue call throws.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The drain order is meaningful. Ghoulifications run first so subsequent commands see a
    /// clean slot map and can <b>short-circuit work targeting destroyed entities</b>. Component changes
    /// run before relation changes so structural transitions settle before the relations system
    /// observes them. Assemblage creations run last so the entities they spawn enter a fully
    /// settled world.
    /// </para>
    /// <para>
    /// Commands that target an entity which no longer satisfies the operation's preconditions
    /// (e.g., adding a component to an already-ghoulified entity) are <b>quietly dropped</b>.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if called while a query is currently iterating.</exception>
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
