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
        TryUpdateEntitySwapped(archetype.Remove(entityMeta));

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

        Archetype? newArchetype = currentArchetype.GetAddEdge(componentId);

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
        TryUpdateEntitySwapped(transferred._entitySwapped);

        return new EntityReferences(ref entityMeta, newArchetype);
    }

    public bool TryAdd<T>(Entity entity, in T component = default)
        where T : struct
    {
        ThrowIfQueriesExecuting();

        EntityReferences entityReferences = AttemptAdd(entity, ComponentMeta<T>.s_id);

        if (entityReferences._valid)
            entityReferences._archetype!.Set(in entityReferences._entityMeta, in component);

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

        Archetype? newArchetype = currentArchetype.GetRemoveEdge(componentId);

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
        TryUpdateEntitySwapped(transferred._entitySwapped);

        return true;
    }

    public bool TryRemove<T>(Entity entity)
        where T : struct
    {
        ThrowIfQueriesExecuting();

        return AttemptRemove(entity, ComponentMeta<T>.s_id);
    }

    public ref T Get<T>(Entity entity)
        where T : struct
    {
        if (!IsAlive(entity))
            ThrowIfEntityNotAlive(entity);

        int componentId = ComponentMeta<T>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        if (!archetype.Has(componentId))
            ThrowIfEntityMissingComponent<T>(entity);

        return ref archetype.Get<T>(in entityMeta);
    }

    public void Set<T>(Entity entity, in T component)
        where T : struct
    {
        if (!IsAlive(entity))
            ThrowIfEntityNotAlive(entity);

        int componentId = ComponentMeta<T>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        if (!archetype.Has(componentId))
            ThrowIfEntityMissingComponent<T>(entity);

        archetype.Set(in entityMeta, component);
    }

    public bool Has<T>(Entity entity)
        where T : struct
    {
        if (!IsAlive(entity))
            return false;

        int componentId = ComponentMeta<T>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = GetArchetype(entityMeta._archetypeIndex);

        return archetype.Has(componentId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TryUpdateEntitySwapped(EntitySwapped swapped)
    {
        if (swapped._entityID != EntitySwapped.InvalidEntitySwappedIndexes)
        {
            ref EntityMeta swappedMeta = ref _entitiesMeta[swapped._entityID];
            swappedMeta._componentIndex = swapped._componentIndex;
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
    internal static void ThrowIfEntityNotAlive(Entity entity)
    {
        throw new InvalidOperationException(
            $"{entity} is not alive. Component access requires a live entity."
        );
    }

    [DoesNotReturn]
    internal static void ThrowIfEntityMissingComponent<T>(Entity entity)
        where T : struct
    {
        throw new InvalidOperationException(
            $"{entity} does not have component '{typeof(T).Name}'. Add the component before attempting to access it."
        );
    }
}
