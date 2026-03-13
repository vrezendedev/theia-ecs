using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
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

    internal Assemblage CreateAssemblage(ReadOnlySpan<int> componentIds)
    {
        Archetype archetype = FindOrCreateArchetype(componentIds);

        Span<int> componentStorageMapping = stackalloc int[componentIds.Length];

        for (int i = 0; i < componentStorageMapping.Length; i++)
            componentStorageMapping[i] = archetype.GetStorageIndex(componentIds[i]);

        return new Assemblage(this, in archetype, componentStorageMapping);
    }

    internal EntityCreated CreateEntity()
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive(Entity entity) =>
        entity._id < _entitiesCount && entity._version == _entitiesMeta[entity._id]._version;

    public bool TryGhoulify(Entity entity)
    {
        if (!IsAlive(entity))
            return false;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = _archetypes[entityMeta._archetypeIndex];

        TryUpdateEntitySwapped(archetype.Remove(entityMeta));

        entityMeta._version++;
        entityMeta._archetypeIndex = EntityMeta.DefaultInvalidEntityMetaIndexes;
        entityMeta._storageIndex = EntityMeta.DefaultInvalidEntityMetaIndexes;
        entityMeta._componentIndex = EntityMeta.DefaultInvalidEntityMetaIndexes;

        _ghoulsCount++;
        _ghouls.Enqueue(entity._id);

        return true;
    }

    public bool TryAdd<T>(Entity entity, in T component)
        where T : struct
    {
        if (!IsAlive(entity))
            return false;

        int componentId = ComponentMeta<T>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype currentArchetype = _archetypes[entityMeta._archetypeIndex];

        if (currentArchetype.Has(componentId))
            return false;

        Signature archetypeSignature = currentArchetype._signature;

        Span<int> ids = stackalloc int[archetypeSignature._length + 1];
        archetypeSignature.GetComponents().CopyTo(ids);
        ids[^1] = componentId;

        Archetype newArchetype = FindOrCreateArchetype(ids);

        EntityTransferred transferred = currentArchetype.Transfer(entity, entityMeta, newArchetype);

        UpdateEntityAccounted(ref entityMeta, transferred._entityAccounted);
        TryUpdateEntitySwapped(transferred._entitySwapped);

        newArchetype.Set(in entityMeta, in component);

        return true;
    }

    public bool TryRemove<T>(Entity entity)
        where T : struct
    {
        if (!IsAlive(entity))
            return false;

        int componentId = ComponentMeta<T>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype currentArchetype = _archetypes[entityMeta._archetypeIndex];

        if (!currentArchetype.Has(componentId))
            return false;

        //@TO-DO

        return true;
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
}
