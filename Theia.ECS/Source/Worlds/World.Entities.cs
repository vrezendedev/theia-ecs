using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Extensions;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private const int DefaultEntitiesMetaCapacity = 16_384;
    private const int DefaultEntitiesMetaGrowthFactor = 2;
    private const int DefaultGhoulsInitialCapacityDivisor = 4;

    private int _entitiesCount;
    private EntityMeta[] _entitiesMeta;

    private Queue<int> _ghouls;

    private EntityCreated CreateEntity()
    {
        Queue<int> ghouls = _ghouls;

        bool dequeue = ghouls.Count > 0;

        int index = dequeue ? ghouls.Dequeue() : _entitiesCount;

        if (!dequeue)
        {
            Array.TryResize(ref _entitiesMeta, index, DefaultEntitiesMetaGrowthFactor);

            _entitiesMeta[index] = new EntityMeta(EntityMeta.DefaultEntityMetaVersion);

            _entitiesCount++;
        }

        ref EntityMeta entityMeta = ref _entitiesMeta[index];

        return new EntityCreated(
            new Entity() { _id = index, _version = entityMeta._version },
            ref entityMeta
        );
    }

    /// <summary>
    /// Allocates a fresh entity slot (recycling a ghoulified slot when available) and places
    /// the entity into <paramref name="archetype"/>. Returns the new entity handle paired with a
    /// reference to its <see cref="EntityMeta"/> for downstream metadata patching.
    /// </summary>
    internal EntityCreated CreateEntity(in Archetype archetype)
    {
        EntityCreated entityCreated = CreateEntity();
        EntityAccounted entityAccounted = archetype.Add(entityCreated._entity);
        UpdateEntityAccounted(ref entityCreated._entityMeta, entityAccounted);

        return entityCreated;
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="entity"/>'s handle still matches its
    /// slot in the world. <b>Stale handles</b> (referring to a slot that has been ghoulified and
    /// possibly recycled) <b>return <see langword="false"/></b> via the version mismatch.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive(Entity entity) =>
        entity._id < _entitiesCount && entity._version == _entitiesMeta[entity._id]._version;

    private void Ghoulify(Entity entity, ref EntityMeta entityMeta, in Archetype archetype)
    {
        ResetRelations(entity);

        UpdateEntitySwappedIfValid(archetype.Remove(entityMeta));

        entityMeta.Reset();

        _ghouls.Enqueue(entity._id);
    }

    private bool AttemptGhoulify(Entity entity)
    {
        if (!IsAlive(entity))
            return false;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];
        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        Ghoulify(entity, ref entityMeta, in archetype);

        InvokeOnGhoulified(new EntityGhoulified(this, entity, in archetype));

        return true;
    }

    /// <summary>
    /// Destroys <paramref name="entity"/> immediately: tears down every relation it participates
    /// in, removes it from its archetype, possibly fires <see cref="Events.RelationEvents.InvokeOnRelationRemoved">InvokeOnRelationRemoved</see>,
    /// calls <see cref="InvokeOnGhoulified"/>, and
    /// queues its slot for recycling. Returns <see langword="false"/> if the entity is already
    /// dead.
    /// </summary>
    /// <remarks>
    /// "Ghoulified" rather than "destroyed" because the slot is held in a recycling queue with
    /// the version bumped, so a stale handle becomes a no-op rather than addressing a different
    /// entity. The slot becomes available for reuse only when a future <c>CreateEntity</c>
    /// dequeues it.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if a query is currently iterating; use <see cref="DeferredGhoulify"/> from inside system execution.</exception>
    public bool TryGhoulify(Entity entity)
    {
        ThrowIfQueriesExecuting();

        return AttemptGhoulify(entity);
    }

    private EntityReferences AttemptAddComponent(Entity entity, int componentId)
    {
        if (!IsAlive(entity))
            return EntityReferences.Invalid;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype currentArchetype = GetArchetype(entityMeta._archetypeIndex);

        if (currentArchetype.Has(componentId))
            return EntityReferences.Invalid;

        Archetype? newArchetype = currentArchetype.TryGetAddEdge(componentId);

        if (newArchetype is null)
        {
            Signature currentSignature = currentArchetype._signature;

            const int MaxStackSize = 256;
            int length = currentSignature._length + 1;
            Span<int> ids = length <= MaxStackSize ? stackalloc int[length] : new int[length];
            currentSignature.GetComponents().CopyTo(ids);
            ids[^1] = componentId;

            newArchetype = FindOrCreateArchetype(ids);

            currentArchetype.SetAddEdge(componentId, newArchetype);
        }

        EntityTransferred transferred = currentArchetype.Transfer(entity, entityMeta, newArchetype);

        UpdateEntityAccounted(ref entityMeta, transferred._entityAccounted);
        UpdateEntitySwappedIfValid(transferred._entitySwapped);

        return new EntityReferences(ref entityMeta, currentArchetype, newArchetype);
    }

    /// <summary>
    /// Adds <typeparamref name="TComponent"/> to <paramref name="entity"/> immediately,
    /// transitioning it to the destination archetype and writing <paramref name="component"/>
    /// into its row. Returns <see langword="false"/> if the entity is dead or already has this
    /// component.
    /// </summary>
    /// <remarks>
    /// On first transition between two given archetypes, the destination is found via
    /// <see cref="FindOrCreateArchetype"/> and cached as an "add edge" on the source archetype;
    /// subsequent additions of the same component to entities in the same source archetype
    /// reuse the edge in O(1) without re-scanning. Calls <see cref="InvokeOnComponentAdded"/>
    /// on success.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if a query is currently iterating; use <see cref="DeferredAddComponent"/> from inside system execution.</exception>
    public bool TryAddComponent<TComponent>(Entity entity, in TComponent component = default)
        where TComponent : struct
    {
        ThrowIfQueriesExecuting();

        int componentId = ComponentMeta<TComponent>.s_id;

        EntityReferences entityReferences = AttemptAddComponent(entity, componentId);

        if (entityReferences._valid)
        {
            entityReferences._currentArchetype!.Set(in entityReferences._entityMeta, in component);

            InvokeOnComponentAdded(
                new EntityModified(
                    this,
                    entity,
                    ComponentsMeta.GetComponentType(componentId)._type,
                    entityReferences._previousArchetype!,
                    entityReferences._currentArchetype!,
                    componentId
                )
            );
        }

        return entityReferences._valid;
    }

    private bool AttemptRemoveComponent(Entity entity, int componentId)
    {
        if (!IsAlive(entity))
            return false;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype currentArchetype = GetArchetype(entityMeta._archetypeIndex);

        if (!currentArchetype.Has(componentId))
            return false;

        Signature currentSignature = currentArchetype._signature;

        if (currentSignature._length == 1)
            return AttemptGhoulify(entity);

        Archetype? newArchetype = currentArchetype.TryGetRemoveEdge(componentId);

        if (newArchetype is null)
        {
            Span<int> ids = stackalloc int[currentSignature._length - 1];
            ReadOnlySpan<int> currentComponents = currentSignature.GetComponents();

            int index = 0;

            for (int i = 0; i < currentComponents.Length; i++)
            {
                int currentComponentId = currentComponents[i];
                if (currentComponentId != componentId)
                {
                    ids[index] = currentComponents[i];
                    index++;
                }
            }

            newArchetype = FindOrCreateArchetype(ids);

            currentArchetype.SetRemoveEdge(componentId, newArchetype);
        }

        EntityTransferred transferred = currentArchetype.Transfer(entity, entityMeta, newArchetype);

        UpdateEntityAccounted(ref entityMeta, transferred._entityAccounted);
        UpdateEntitySwappedIfValid(transferred._entitySwapped);

        InvokeOnComponentRemoved(
            new EntityModified(
                this,
                entity,
                ComponentsMeta.GetComponentType(componentId)._type,
                currentArchetype,
                newArchetype,
                componentId
            )
        );

        return true;
    }

    /// <summary>
    /// Removes <typeparamref name="TComponent"/> from <paramref name="entity"/> immediately.
    /// If <b>the component was the entity's last, the entity is ghoulified instead</b>. Returns
    /// <see langword="false"/> if the entity is dead or does not have this component.
    /// </summary>
    /// <remarks>
    /// On first transition between two given archetypes, the destination is found via
    /// <see cref="FindOrCreateArchetype"/> and cached as a "remove edge" on the source
    /// archetype; subsequent removals of the same component from entities in the same source
    /// archetype reuse the edge in O(1). Calls <see cref="InvokeOnComponentRemoved"/>
    /// on success or, when the removal would leave the entity with no components,
    /// <see cref="InvokeOnGhoulified"/> via the implicit ghoulification.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if a query is currently iterating; use <see cref="DeferredRemoveComponent"/> from inside system execution.</exception>
    public bool TryRemoveComponent<TComponent>(Entity entity)
        where TComponent : struct
    {
        ThrowIfQueriesExecuting();

        return AttemptRemoveComponent(entity, ComponentMeta<TComponent>.s_id);
    }

    /// <summary>
    /// Returns a reference to <paramref name="entity"/>'s <typeparamref name="TComponent"/>
    /// row in storage. <b>Mutations through the reference persist</b>.
    /// </summary>
    /// <remarks>
    /// <b>Not thread-safe:</b> concurrent structural changes that transition the entity to a
    /// different archetype while the reference is in use produce undefined behavior; the caller
    /// is responsible for ensuring the entity stays in its current archetype for the
    /// reference's lifetime.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the entity is dead or does not have <typeparamref name="TComponent"/>.</exception>
    public ref TComponent Get<TComponent>(Entity entity)
        where TComponent : struct
    {
        if (!IsAlive(entity))
            ThrowEntityNotAlive(entity);

        int componentId = ComponentMeta<TComponent>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        if (!archetype.Has(componentId))
            ThrowEntityMissingComponent<TComponent>(entity);

        return ref archetype.Get<TComponent>(in entityMeta);
    }

    /// <summary>
    /// Overwrites <paramref name="entity"/>'s <typeparamref name="TComponent"/> row with
    /// <paramref name="component"/>. Does not fire any events; this is a value update, not a
    /// structural change.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the entity is dead or does not have <typeparamref name="TComponent"/>.</exception>
    public void Set<TComponent>(Entity entity, in TComponent component)
        where TComponent : struct
    {
        if (!IsAlive(entity))
            ThrowEntityNotAlive(entity);

        int componentId = ComponentMeta<TComponent>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        if (!archetype.Has(componentId))
            ThrowEntityMissingComponent<TComponent>(entity);

        archetype.Set(in entityMeta, component);
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="entity"/>'s current archetype is bound
    /// to <paramref name="assemblage"/>: the entity's composition exactly matches that
    /// assemblage's composition with no additions or removals having transitioned it elsewhere.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Is(Entity entity, Assemblage assemblage)
    {
        if (!IsAlive(entity))
            return false;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        return assemblage == archetype.GetMatchedAssemblage();
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="entity"/>'s current archetype's signature satisfies
    /// to <paramref name="assemblage"/>: the entity's composition satisfies that assemblage's composition.
    /// </summary>
    public bool Satisfies(Entity entity, Assemblage assemblage)
    {
        if (!IsAlive(entity))
            return false;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity.ID];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        return assemblage._signature.IsSatisfiedBy(archetype._signature);
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="entity"/> currently has <typeparamref name="TComponent"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has<TComponent>(Entity entity)
        where TComponent : struct
    {
        if (!IsAlive(entity))
            return false;

        int componentId = ComponentMeta<TComponent>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        return archetype.Has(componentId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateEntitySwappedIfValid(EntitySwapped swapped)
    {
        if (swapped._entityID != EntitySwapped.InvalidEntitySwappedIndexes)
        {
            ref EntityMeta swappedMeta = ref _entitiesMeta[swapped._entityID];
            swappedMeta._componentIndex = swapped._dataIndex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateEntityAccounted(ref EntityMeta entityMeta, EntityAccounted entityAccounted)
    {
        entityMeta._archetypeIndex = entityAccounted._archetypeIndex;
        entityMeta._storageIndex = entityAccounted._storageIndex;
        entityMeta._componentIndex = entityAccounted._componentIndex;
    }

    [DoesNotReturn]
    private static void ThrowEntityNotAlive(Entity entity) =>
        throw new InvalidOperationException(
            $"{entity} is not alive. Access requires an entity to be alive."
        );

    [DoesNotReturn]
    private static void ThrowEntityMissingComponent<TComponent>(Entity entity)
        where TComponent : struct =>
        throw new InvalidOperationException(
            $"{entity} does not have component '{typeof(TComponent).Name}'. Add the component before attempting to access it."
        );
}
