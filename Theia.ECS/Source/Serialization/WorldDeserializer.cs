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
