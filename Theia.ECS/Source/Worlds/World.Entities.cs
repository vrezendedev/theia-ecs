using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private const int DefaultEntitiesMetaGrowthFactor = 2;

    private int _entitiesCount;
    private EntityMeta[] _entitiesMeta;

    private int _ghoulsCount;
    private Queue<int> _ghouls;

    private EntityCreated CreateEntity()
    {
        bool dequeue = _ghoulsCount > 0;

        int index = dequeue ? _ghouls.Dequeue() : _entitiesCount;

        if (!dequeue)
        {
            int currentLength = _entitiesMeta.Length;

            if (index == currentLength)
                Array.Resize(ref _entitiesMeta, currentLength * DefaultEntitiesMetaGrowthFactor);

            _entitiesMeta[index] = new EntityMeta(
                EntityMeta.DefaultEntityMetaVersion,
                EntityMeta.DefaultInvalidEntityMetaIndexes,
                EntityMeta.DefaultInvalidEntityMetaIndexes,
                EntityMeta.DefaultInvalidEntityMetaIndexes
            );

            _entitiesCount++;
        }
        else
            _ghoulsCount--;

        ref EntityMeta entityMeta = ref _entitiesMeta[index];

        return new EntityCreated(
            new Entity() { _id = index, _version = entityMeta._version },
            ref entityMeta
        );
    }

    internal EntityCreated CreateEntity(in Archetype archetype)
    {
        EntityCreated entityCreated = CreateEntity();
        EntityAccounted entityAccounted = archetype.Add(entityCreated._entity);
        UpdateEntityAccounted(ref entityCreated._entityMeta, entityAccounted);

        return entityCreated;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive(Entity entity) =>
        entity._id < _entitiesCount && entity._version == _entitiesMeta[entity._id]._version;

    private void Ghoulify(Entity entity, ref EntityMeta entityMeta, in Archetype archetype)
    {
        UpdateEntitySwappedIfValid(archetype.Remove(entityMeta));

        entityMeta._version++;
        entityMeta._archetypeIndex = EntityMeta.DefaultInvalidEntityMetaIndexes;
        entityMeta._storageIndex = EntityMeta.DefaultInvalidEntityMetaIndexes;
        entityMeta._componentIndex = EntityMeta.DefaultInvalidEntityMetaIndexes;

        _ghoulsCount++;
        _ghouls.Enqueue(entity._id);
    }

    private bool AttemptGhoulify(Entity entity)
    {
        if (!IsAlive(entity))
            return false;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];
        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        Ghoulify(entity, ref entityMeta, in archetype);

        InvokeOnEntityGhoulified(new EntityGhoulified(entity, in archetype));

        return true;
    }

    public bool TryGhoulify(Entity entity)
    {
        ThrowIfQueriesExecuting();

        return AttemptGhoulify(entity);
    }

    private EntityReferences AttemptAdd(Entity entity, int componentId)
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

            Span<int> ids = stackalloc int[currentSignature._length + 1];
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

    public bool TryAdd<TComponent>(Entity entity, in TComponent component = default)
        where TComponent : struct
    {
        ThrowIfQueriesExecuting();

        int componentId = ComponentMeta<TComponent>.s_id;

        EntityReferences entityReferences = AttemptAdd(entity, componentId);

        if (entityReferences._valid)
        {
            entityReferences._currentArchetype!.Set(in entityReferences._entityMeta, in component);

            InvokeOnComponentAdded(
                new EntityModified(
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

    private bool AttemptRemove(Entity entity, int componentId)
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
                entity,
                ComponentsMeta.GetComponentType(componentId)._type,
                currentArchetype,
                newArchetype,
                componentId
            )
        );

        return true;
    }

    public bool TryRemove<TComponent>(Entity entity)
        where TComponent : struct
    {
        ThrowIfQueriesExecuting();

        return AttemptRemove(entity, ComponentMeta<TComponent>.s_id);
    }

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
    internal static void ThrowEntityNotAlive(Entity entity) =>
        throw new InvalidOperationException(
            $"{entity} is not alive. Component access requires a live entity."
        );

    [DoesNotReturn]
    internal static void ThrowEntityMissingComponent<TComponent>(Entity entity)
        where TComponent : struct =>
        throw new InvalidOperationException(
            $"{entity} does not have component '{typeof(TComponent).Name}'. Add the component before attempting to access it."
        );
}
