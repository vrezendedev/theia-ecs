using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    private const int DefaultEntitiesMetaGrowthFactor = 2;
    private const int DefaultEntityMetaVersion = 1;
    private const int DefaultInvalidEntityMetaIndexes = -1;

    private int _entitiesCount;
    private EntityMeta[] _entitiesMeta;

    private int _ghoulsCount;
    private Queue<int> _ghouls;

    private Assemblage[] _assemblages;

    internal Assemblage CreateAssemblage(ReadOnlySpan<int> componentIds)
    {
        int newAssemblageComponentLength = componentIds.Length;
        SignatureMeta newAssemblageSignatureMeta = Signature.GetSignatureMeta(componentIds);
        Span<ulong> newAssemblageMask = stackalloc ulong[newAssemblageSignatureMeta._maskLength];
        Signature.GetSignatureMask(componentIds, newAssemblageMask);

        Assemblage[] assemblages = _assemblages;

        for (int i = 0; i < assemblages.Length; i++)
        {
            Signature registeredAssemblageSignature = assemblages[i]._signature;
            ReadOnlySpan<ulong> registeredAssemblageMask = registeredAssemblageSignature.GetMask();

            if (
                Signature.IsEqual(
                    newAssemblageComponentLength,
                    registeredAssemblageSignature._length,
                    newAssemblageMask,
                    registeredAssemblageMask
                )
            )
                ThrowInvalidOperationDuplicatedAssemblage(componentIds);
        }

        Archetype targetArchetype = null!;

        Archetype[] archetypes = _archetypes;

        for (int i = 0; i < archetypes.Length; i++)
        {
            Signature registeredArchetypeSignature = archetypes[i]._signature;
            ReadOnlySpan<ulong> registeredArchetypeMask = registeredArchetypeSignature.GetMask();

            if (
                Signature.IsEqual(
                    newAssemblageComponentLength,
                    registeredArchetypeSignature._length,
                    newAssemblageMask,
                    registeredArchetypeMask
                )
            )
            {
                targetArchetype = archetypes[i];
                break;
            }
        }

        if (targetArchetype is null)
        {
            Signature signature = new Signature(
                componentIds,
                newAssemblageSignatureMeta,
                newAssemblageMask
            );

            targetArchetype = CreateArchetype(in signature);
        }

        Span<int> componentStorageMapping = stackalloc int[componentIds.Length];

        for (int i = 0; i < componentStorageMapping.Length; i++)
            componentStorageMapping[i] = targetArchetype.GetStorageIndex(componentIds[i]);

        Assemblage assemblage = new Assemblage(this, in targetArchetype, componentStorageMapping);

        int index = _assemblages.Length;

        Array.Resize(ref _assemblages, index + 1);

        _assemblages[index] = assemblage;

        return assemblage;
    }

    internal EntityCreated CreateEntity()
    {
        bool dequeue = _ghoulsCount > 0;
        int currentLength = _entitiesMeta.Length;

        int index = dequeue ? _ghouls.Dequeue() : _entitiesCount;

        if (!dequeue)
        {
            if (index == currentLength)
                Array.Resize(ref _entitiesMeta, currentLength * DefaultEntitiesMetaGrowthFactor);

            _entitiesMeta[index] = new EntityMeta(
                DefaultEntityMetaVersion,
                DefaultInvalidEntityMetaIndexes,
                DefaultInvalidEntityMetaIndexes,
                DefaultInvalidEntityMetaIndexes
            );

            _entitiesCount++;
        }

        ref EntityMeta entityMeta = ref _entitiesMeta[index];

        return new EntityCreated(
            new Entity() { _id = index, _version = entityMeta._version },
            ref entityMeta
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive(Entity entity) =>
        entity._id < _entitiesCount && entity._version == _entitiesMeta[entity._id]._version;

    public void Ghoulify(Entity entity)
    {
        if (!IsAlive(entity))
            return;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        Archetype archetype = _archetypes[entityMeta._archetypeIndex];

        EntitySwapped swapped = archetype.Remove(entityMeta);

        if (swapped._entityID != EntitySwapped.None._entityID)
        {
            ref EntityMeta swappedMeta = ref _entitiesMeta[swapped._entityID];
            swappedMeta._componentIndex = swapped._componentIndex;
        }

        entityMeta._version++;
        entityMeta._archetypeIndex = DefaultInvalidEntityMetaIndexes;
        entityMeta._storageIndex = DefaultInvalidEntityMetaIndexes;
        entityMeta._componentIndex = DefaultInvalidEntityMetaIndexes;

        _ghoulsCount++;
        _ghouls.Enqueue(entity._id);
    }

    [DoesNotReturn]
    private static void ThrowInvalidOperationDuplicatedAssemblage(ReadOnlySpan<int> componentIds) =>
        throw new InvalidOperationException(
            $"An Assemblage with the same Component Set ({string.Join(", ", componentIds.ToArray().Select(id => ComponentsMeta.GetComponentType(id)._type.Name))}) is already registered."
        );
}
