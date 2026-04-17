using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    private readonly List<Entity> _tempEntities;
    private readonly List<int> _tempLengths;
    private readonly ArrayBufferWriter<byte> _bufferWriter;

    private string[] _componentsTypeName;
    private string[] _relationsTypeName;

    internal WorldSerializer(MessagePackSerializerOptions options)
    {
        _dtoWorld = new()
        {
            Version = 1,
            ComponentsTypesAccounted = Array.Empty<string>(),
            RelationsTypesAccounted = Array.Empty<string>(),
            ArchetypesAccounted = Array.Empty<ArchetypeDataTransferObject>(),
            RelationsAccounted = Array.Empty<RelationDataTransferObject>(),
            UniquesAccounted = Array.Empty<UniqueDataTransferObject>(),
        };

        _serializerOptions = options;

        _tempEntities = new(Archetype.MinimumEntitiesPerChunk);
        _tempLengths = new(Archetype.MinimumEntitiesPerChunk);
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
            _componentsTypeName[i] = ComponentsMeta.GetComponentType(i)._name!;

        _dtoWorld.ComponentsTypesAccounted = _componentsTypeName;

        return this;
    }

    private WorldSerializer AccountRelationsTypes()
    {
        int relationsCount = RelationsMeta.Count();

        _relationsTypeName =
            relationsCount > 0 ? new string[RelationsMeta.Count()] : Array.Empty<string>();

        for (int i = 0; i < relationsCount; i++)
            _relationsTypeName[i] = RelationsMeta.GetRelationType(i)._name!;

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
            ArchetypeDataTransferObject dtoArchetype = new()
            {
                ComponentsTypeSet = Array.Empty<string>(),
                Entities = Array.Empty<Entity>(),
                ComponentData = Array.Empty<byte[]>(),
            };

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

            RelationDataTransferObject dtoRelation = new()
            {
                RelationType = _relationsTypeName[i],
                EntityRelations = Array.Empty<EntityRelationDataTransferObject>(),
            };

            int slotsOccupied = relationStorage?.CountStorageSlotsOccupied() ?? 0;

            if (relationStorage is not null && slotsOccupied > 0)
                AccountEntityRelations(relationStorage, slotsOccupied, dtoRelation);

            _dtoWorld.RelationsAccounted[i] = dtoRelation;
        }

        return this;
    }

    /// <summary>
    /// Iterates in reverse order so that <see cref="UniqueDataTransferObject"/> entries are written
    /// from the highest component ID down to zero.
    /// <para>
    /// During deserialization, the forward pass then encounters the highest component ID first, allowing
    /// <see cref="World.GetOrCreateUnique"/> to, presumably, resize <c>_uniques</c> to its final required length
    /// in a single allocation.
    /// </para>
    /// All subsequent calls for lower IDs may be guaranteed to fit within the already-allocated array, avoiding
    /// incremental resizes that would otherwise occur for each unique crossing a capacity boundary.
    /// </summary>
    private WorldSerializer AccountUniques(ReadOnlySpan<Unique> uniques)
    {
        _dtoWorld.UniquesAccounted =
            uniques.Length > 0
                ? new UniqueDataTransferObject[uniques.Length]
                : Array.Empty<UniqueDataTransferObject>();

        for (int i = uniques.Length - 1; i >= 0; i--)
        {
            Unique unique = uniques[i];

            UniqueDataTransferObject uniqueDto = new()
            {
                ComponentType = _componentsTypeName[i],
                ComponentData = Array.Empty<byte>(),
            };

            if (unique is not null)
            {
                _bufferWriter.ResetWrittenCount();
                unique.WriteData(_bufferWriter, _serializerOptions);
                uniqueDto.ComponentData = _bufferWriter.WrittenSpan.ToArray();
            }

            _dtoWorld.UniquesAccounted[i] = uniqueDto;
        }

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
        _tempEntities.Clear();
        _tempLengths.Clear();

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
            for (int i = 0; i < componentsIds.Length; i++)
                archetypeDto.ComponentData[i] = Array.Empty<byte>();

            return;
        }

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

            archetypeDto.ComponentData[i] = _bufferWriter.WrittenSpan.ToArray();
        }
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
}
