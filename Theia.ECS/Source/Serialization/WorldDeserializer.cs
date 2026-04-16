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
    private readonly int[] _newEntityIdsMapping;

    internal WorldDeserializer(
        WorldDataTransferObject worldDataTransferObject,
        MessagePackSerializerOptions options
    )
    {
        _dtoWorld = worldDataTransferObject;
        _deserializerOptions = options;
        _newEntityIdsMapping = ArrayPool<int>.Shared.Rent(worldDataTransferObject.MaxEntityId);
    }

    internal void To(in World world) =>
        AttemptComponentTypesRegistration()
            .AttemptRelationTypesRegistration()
            .AccountArchetypes(in world)
            .AccountRelations(in world)
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
            _newEntityIdsMapping[entities[i]._id] = entityCreated._entity._id;
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

    public void Dispose()
    {
        ArrayPool<int>.Shared.Return(_newEntityIdsMapping);
    }
}
