using System;
using System.Buffers;
using MessagePack;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Relations;
using Theia.ECS.Worlds;

namespace Theia.ECS.Serialization;

/// <summary>
/// Restores world state from a <see cref="WorldDataTransferObject"/> by walking its sections in
/// turn and rehydrating each into the destination <see cref="World"/>: registers types, recreates
/// archetypes with their entities and component data, restores relations, and restores uniques.
/// One instance per deserialization call; not reusable.
/// </summary>
/// <remarks>
/// <para>
/// The walk is fluent, each step returns <c>this</c>, so <see cref="To"/> reads as a single
/// statement listing the order of operations: register component types, register relation types,
/// account archetypes, account relations, account uniques, dispose. Order is load-bearing:
/// types must be registered before archetypes can resolve them, and entities must exist (via
/// archetype rehydration) before relations can link them.
/// </para>
/// <para>
/// Entities cannot keep their original IDs across deserialization, since the destination world
/// has its own slot allocator and version counters. <see cref="_newEntitiesMapping"/> records
/// the source-id to destination-entity mapping built during archetype rehydration, so the <b>relation
/// pass can resolve old owner/target IDs to their freshly created counterparts</b>.
/// </para>
/// <para>
/// Relations are added through <see cref="World.UnrestrictedAddRelation"/> rather than the public
/// <c>TryAddTagRelation</c>/<c>TryAddEvaluatedRelation</c> paths because deserialization needs
/// to bypass liveness validation, tag/evaluated category enforcement, and event firing — the
/// world is being rehydrated mid-operation and the standard guards would either reject valid
/// links or fire spurious events on a partially-loaded world.
/// </para>
/// </remarks>
internal sealed class WorldDeserializer : IDisposable
{
    private readonly WorldDataTransferObject _dtoWorld;
    private readonly MessagePackSerializerOptions _deserializerOptions;
    private readonly Entity[] _newEntitiesMapping;

    internal WorldDeserializer(
        WorldDataTransferObject worldDataTransferObject,
        MessagePackSerializerOptions options
    )
    {
        _dtoWorld = worldDataTransferObject;
        _deserializerOptions = options;
        _newEntitiesMapping = ArrayPool<Entity>.Shared.Rent(worldDataTransferObject.MaxEntityId);
    }

    /// <summary>
    /// Rehydrates <paramref name="world"/> from <see cref="_dtoWorld"/> in fixed order: register
    /// component types, register relation types, recreate archetypes (and their entities and
    /// component data), restore relations, restore uniques. The pooled entity-mapping array is
    /// returned at the end via <see cref="Dispose"/>.
    /// </summary>
    internal void To(in World world) =>
        AttemptComponentTypesRegistration()
            .AttemptRelationTypesRegistration()
            .AccountArchetypes(in world)
            .AccountRelations(in world)
            .AccountUniques(in world)
            .Dispose();

    private WorldDeserializer AttemptComponentTypesRegistration()
    {
        ReadOnlySpan<string> typesAccounted = _dtoWorld.ComponentsTypesAccounted;

        for (int i = 0; i < typesAccounted.Length; i++)
            ComponentsMeta.AttemptRegisterComponent(typesAccounted[i]);

        return this;
    }

    private WorldDeserializer AttemptRelationTypesRegistration()
    {
        ReadOnlySpan<string> typesAccounted = _dtoWorld.RelationsTypesAccounted;

        for (int i = 0; i < typesAccounted.Length; i++)
            RelationsMeta.AttemptRegisterRelation(typesAccounted[i]);

        return this;
    }

    private WorldDeserializer AccountArchetypes(in World world)
    {
        int[] componentsIds = ArrayPool<int>.Shared.Rent(_dtoWorld.ComponentsTypesAccounted.Length);

        ReadOnlySpan<ArchetypeDataTransferObject> archetypeDataTransferObjects =
            _dtoWorld.ArchetypesAccounted;

        for (int i = 0; i < archetypeDataTransferObjects.Length; i++)
        {
            ArchetypeDataTransferObject archetypeDto = archetypeDataTransferObjects[i];

            Archetype archetype = AccountArchetype(
                componentsIds,
                archetypeDto.ComponentsTypeSet,
                in world
            );

            if (archetypeDto.Entities.Length > 0)
                AccountEntities(
                    archetypeDto.Entities,
                    componentsIds.AsSpan(0, archetypeDto.ComponentsTypeSet.Length),
                    archetypeDto.ComponentData,
                    in archetype,
                    in world
                );
        }

        ArrayPool<int>.Shared.Return(componentsIds);

        return this;
    }

    private WorldDeserializer AccountRelations(in World world)
    {
        ReadOnlySpan<RelationDataTransferObject> relationDataTransferObjects =
            _dtoWorld.RelationsAccounted;

        for (int i = 0; i < relationDataTransferObjects.Length; i++)
        {
            RelationDataTransferObject relationDto = relationDataTransferObjects[i];

            ReadOnlySpan<EntityRelationDataTransferObject> entityRelationDataTransferObjects =
                relationDto.EntityRelations;

            if (entityRelationDataTransferObjects.Length == 0)
                continue;

            int relationId = RelationsMeta.GetRelationId(relationDto.RelationType);

            for (int j = 0; j < entityRelationDataTransferObjects.Length; j++)
            {
                EntityRelationDataTransferObject entityRelationDto =
                    entityRelationDataTransferObjects[j];

                ReadOnlySpan<Entity> relatedTo = entityRelationDto.Related;

                if (relatedTo.Length == 0)
                    continue;

                Entity newOwnerEntity = _newEntitiesMapping[entityRelationDto.Owner._id];

                Relation relation = null!;

                for (int k = 0; k < relatedTo.Length; k++)
                    relation = world.UnrestrictedAddRelation(
                        relationId,
                        newOwnerEntity,
                        _newEntitiesMapping[relatedTo[k]._id]
                    );

                if (relation is not null && entityRelationDto.RelationData.Length > 0)
                    relation.CopyAllData(entityRelationDto.RelationData, _deserializerOptions);
            }
        }

        return this;
    }

    private WorldDeserializer AccountUniques(in World world)
    {
        ReadOnlySpan<UniqueDataTransferObject> uniqueDataTransferObjects =
            _dtoWorld.UniquesAccounted;

        for (int i = 0; i < uniqueDataTransferObjects.Length; i++)
        {
            UniqueDataTransferObject uniqueDto = uniqueDataTransferObjects[i];

            if (uniqueDto.ComponentData.Length == 0)
                continue;

            Unique unique = world.GetOrCreateUnique(
                ComponentsMeta.GetComponentId(uniqueDto.ComponentType)
            );

            unique.CopyData(uniqueDto.ComponentData, _deserializerOptions);
        }

        return this;
    }

    private Archetype AccountArchetype(
        int[] componentsIds,
        ReadOnlySpan<string> components,
        in World world
    )
    {
        for (int i = 0; i < components.Length; i++)
            componentsIds[i] = ComponentsMeta.GetComponentId(components[i]);

        return world.FindOrCreateArchetype(componentsIds.AsSpan(0, components.Length));
    }

    private void AccountEntities(
        ReadOnlySpan<Entity> entities,
        ReadOnlySpan<int> componentsIds,
        ReadOnlySpan<byte[]> components,
        in Archetype archetype,
        in World world
    )
    {
        for (int i = 0; i < entities.Length; i++)
        {
            EntityCreated entityCreated = world.CreateEntity(in archetype);
            _newEntitiesMapping[entities[i]._id] = entityCreated._entity;
        }

        for (int i = 0; i < componentsIds.Length; i++)
        {
            ReadOnlySpan<Storage> storages = archetype.GetStorages(
                archetype.GetStorageIndex(componentsIds[i])
            );

            storages[0]
                .CopyAllData(storages, components[i], archetype._capacity, _deserializerOptions);
        }
    }

    public void Dispose() => ArrayPool<Entity>.Shared.Return(_newEntitiesMapping);
}
