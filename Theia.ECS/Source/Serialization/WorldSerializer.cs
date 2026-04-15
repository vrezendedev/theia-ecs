using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MessagePack;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Relations;
using Theia.ECS.Worlds;

namespace Theia.ECS.Serialization;

internal sealed class WorldSerializer
{
    private readonly WorldDataTransferObject _dtoWorld;
    private readonly MessagePackSerializerOptions _serializerOptions;

    private readonly StringBuilder _stringBuilder;
    private readonly List<Entity> _tempEntities;
    private readonly List<int> _tempLengths;
    private readonly ArrayBufferWriter<byte> _bufferWriter;

    private string[] _componentsTypeName;
    private string[] _relationsTypeName;

    internal WorldSerializer(MessagePackSerializerOptions options)
    {
        _dtoWorld = new() { Version = 1 };
        _serializerOptions = options;

        _stringBuilder = new();
        _tempEntities = new();
        _tempLengths = new();
        _bufferWriter = new();

        _componentsTypeName = Array.Empty<string>();
        _relationsTypeName = Array.Empty<string>();
    }

    internal WorldDataTransferObject Create(World world) =>
        AccountMaxEntityId(world)
            .AccountComponentsTypes()
            .AccountRelationsTypes()
            .AccountArchetypes(world.GetArchetypes())
            .AccountRelations(world.GetRelationStorages())
            .AccountUniques(world.GetUniques())
            ._dtoWorld;

    private WorldSerializer AccountMaxEntityId(World world)
    {
        _dtoWorld.MaxEntityId = world.CountTotalEntities();

        return this;
    }

    private WorldSerializer AccountComponentsTypes()
    {
        int componentsCount = ComponentsMeta.Count();

        _componentsTypeName =
            componentsCount > 0 ? new string[componentsCount] : Array.Empty<string>();

        for (int i = 0; i < componentsCount; i++)
            _componentsTypeName[i] = GetTypeName(ComponentsMeta.GetComponentType(i)._type);

        _dtoWorld.ComponentsTypesAccounted = _componentsTypeName;

        return this;
    }

    private WorldSerializer AccountRelationsTypes()
    {
        int relationsCount = RelationsMeta.Count();

        _relationsTypeName =
            relationsCount > 0 ? new string[RelationsMeta.Count()] : Array.Empty<string>();

        for (int i = 0; i < relationsCount; i++)
            _relationsTypeName[i] = GetTypeName(RelationsMeta.GetRelationType(i)._type);

        _dtoWorld.RelationsTypesAccounted = _relationsTypeName;

        return this;
    }

    private WorldSerializer AccountArchetypes(ReadOnlySpan<Archetype> archetypes)
    {
        _dtoWorld.ArchetypesAccounted =
            archetypes.Length > 0
                ? new ArchetypeDataTransferObject[archetypes.Length]
                : Array.Empty<ArchetypeDataTransferObject>();

        for (int i = 0; i < archetypes.Length; i++)
        {
            ArchetypeDataTransferObject dtoArchetype = new();

            Archetype archetype = archetypes[i];

            AccountArchetypeComponents(in archetype, dtoArchetype);
            AccountArchetypeEntities(in archetype, dtoArchetype);

            _dtoWorld.ArchetypesAccounted[i] = dtoArchetype;
        }

        return this;
    }

    private WorldSerializer AccountRelations(ReadOnlySpan<RelationStorage> relationStorages)
    {
        _dtoWorld.RelationsAccounted =
            relationStorages.Length > 0
                ? new RelationDataTransferObject[relationStorages.Length]
                : Array.Empty<RelationDataTransferObject>();

        for (int i = 0; i < relationStorages.Length; i++)
        {
            RelationStorage relationStorage = relationStorages[i];

            RelationDataTransferObject dtoRelation = new() { RelationType = _relationsTypeName[i] };

            int slotsOccupied = relationStorage?.CountStorageSlotsOccupied() ?? 0;

            if (relationStorage is null || slotsOccupied == 0)
                dtoRelation.EntityRelations = Array.Empty<EntityRelationDataTransferObject>();
            else
                AccountEntityRelations(relationStorage, slotsOccupied, dtoRelation);

            _dtoWorld.RelationsAccounted[i] = dtoRelation;
        }

        return this;
    }

    private WorldSerializer AccountUniques(ReadOnlySpan<Unique> uniques)
    {
        UniqueDataTransferObject[] uniqueDataTransferObjects =
            uniques.Length > 0
                ? new UniqueDataTransferObject[uniques.Length]
                : Array.Empty<UniqueDataTransferObject>();

        for (int i = 0; i < uniques.Length; i++)
        {
            Unique unique = uniques[i];

            UniqueDataTransferObject uniqueDto = new() { ComponentType = _componentsTypeName[i] };

            if (unique is not null)
            {
                _bufferWriter.ResetWrittenCount();
                unique.WriteData(_bufferWriter, _serializerOptions);
                uniqueDto.ComponentData = _bufferWriter.WrittenSpan.ToArray();
            }

            uniqueDataTransferObjects[i] = uniqueDto;
        }

        _dtoWorld.UniquesAccounted = uniqueDataTransferObjects;

        return this;
    }

    private void AccountArchetypeComponents(
        in Archetype archetype,
        ArchetypeDataTransferObject archetypeDto
    )
    {
        Signature signature = archetype._signature;
        ReadOnlySpan<int> componentsIds = signature.GetComponents();
        int componentsLength = signature._length;

        string[] components = new string[componentsLength];

        for (int j = 0; j < componentsLength; j++)
        {
            int componentId = componentsIds[j];
            components[j] = _componentsTypeName[componentId];
        }

        archetypeDto.ComponentsTypeSet = components;
        archetypeDto.ComponentData = new byte[componentsLength][];
    }

    private void AccountArchetypeEntities(
        in Archetype archetype,
        ArchetypeDataTransferObject archetypeDto
    )
    {
        ReadOnlySpan<int> componentsIds = archetype._signature.GetComponents();

        ReadOnlySpan<Indexer> indexers = archetype.GetIndexers();

        int accLength = 0;

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int length = indexer.Count();

            _tempLengths.Add(length);
            accLength += length;

            if (length == 0)
                continue;

            _tempEntities.AddRange(indexer.GetValues());
        }

        if (accLength == 0)
        {
            archetypeDto.Entities = Array.Empty<Entity>();

            for (int i = 0; i < componentsIds.Length; i++)
                archetypeDto.ComponentData![i] = Array.Empty<byte>();
        }
        else
        {
            archetypeDto.Entities = _tempEntities.ToArray();

            for (int i = 0; i < componentsIds.Length; i++)
            {
                ReadOnlySpan<Storage> storages = archetype.GetStorages(
                    archetype.GetStorageIndex(componentsIds[i])
                );

                _bufferWriter.ResetWrittenCount();

                storages[0]
                    .WriteAllData(
                        storages,
                        accLength,
                        CollectionsMarshal.AsSpan(_tempLengths),
                        _bufferWriter,
                        _serializerOptions
                    );

                archetypeDto.ComponentData![i] = _bufferWriter.WrittenSpan.ToArray();
            }
        }

        _tempEntities.Clear();
        _tempLengths.Clear();
    }

    private void AccountEntityRelations(
        in RelationStorage relationStorage,
        int slotsOccupied,
        RelationDataTransferObject relationDataTransferObject
    )
    {
        EntityRelationDataTransferObject[] dtoEntityRelations =
            new EntityRelationDataTransferObject[slotsOccupied];

        ReadOnlySpan<Relation> relations = relationStorage.GetRelations();

        int entityRelationsIndex = 0;

        for (int i = 0; i < relations.Length; i++)
        {
            Relation relation = relations[i];

            if (relation is null)
                continue;

            _bufferWriter.ResetWrittenCount();

            relation.WriteData(_bufferWriter, _serializerOptions);

            EntityRelationDataTransferObject dtoEntityRelation =
                new EntityRelationDataTransferObject
                {
                    Owner = relation.GetOwner(),
                    Related = relation.To().ToArray(),
                    RelationData = _bufferWriter.WrittenSpan.ToArray(),
                };

            dtoEntityRelations[entityRelationsIndex] = dtoEntityRelation;

            entityRelationsIndex++;
        }

        relationDataTransferObject.EntityRelations = dtoEntityRelations;
    }

    private string GetTypeName(Type type)
    {
        _stringBuilder.Append(type.FullName);
        _stringBuilder.Append(", ");
        _stringBuilder.Append(type.Assembly.GetName().Name);

        string typeName = _stringBuilder.ToString();

        _stringBuilder.Clear();

        return typeName;
    }
}
