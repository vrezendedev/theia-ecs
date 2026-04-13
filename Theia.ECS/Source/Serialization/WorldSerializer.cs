using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Relations;
using Theia.ECS.Worlds;

namespace Theia.ECS.Serialization;

internal sealed class WorldSerializer
{
    private readonly WorldDataTransferObject _dtoWorld;

    private StringBuilder _stringBuilder = new();
    private List<Entity> _tempEntities = new();
    private int maxEntityId;

    private string[] _componentsTypeName;
    private ArrayBufferWriter<byte>[] _componentWriters;
    private string[] _relationsTypeName;

#pragma warning disable CS8618
    internal WorldSerializer() => _dtoWorld = new() { Version = 1 };

#pragma warning restore

    internal WorldDataTransferObject Create(World world) =>
        AccountMaxEntityId(world)
            .AccountComponentsTypes()
            .AccountRelationsTypes()
            .AccountArchetypes(world.GetArchetypes())
            .AccountRelations(world.GetRelationStorages())
            ._dtoWorld;

    private WorldSerializer AccountMaxEntityId(World world)
    {
        _dtoWorld.MaxEntityId = world.CountEntities();

        return this;
    }

    private WorldSerializer AccountComponentsTypes()
    {
        int componentsCount = ComponentsMeta.Count();

        _componentsTypeName = new string[componentsCount];
        _componentWriters = new ArrayBufferWriter<byte>[componentsCount];

        for (int i = 0; i < _componentsTypeName.Length; i++)
        {
            _componentsTypeName[i] = GetTypeName(ComponentsMeta.GetComponentType(i)._type);
            _componentWriters[i] = new ArrayBufferWriter<byte>();
        }

        _dtoWorld.ComponentsTypesAccounted = _componentsTypeName;

        return this;
    }

    private WorldSerializer AccountRelationsTypes()
    {
        _relationsTypeName = new string[RelationsMeta.Count()];

        for (int i = 0; i < _relationsTypeName.Length; i++)
            _relationsTypeName[i] = GetTypeName(RelationsMeta.GetRelationType(i)._type);

        _dtoWorld.RelationsTypesAccounted = _relationsTypeName;

        return this;
    }

    private WorldSerializer AccountArchetypes(ReadOnlySpan<Archetype> archetypes)
    {
        _dtoWorld.ArchetypesAccounted = new ArchetypeDataTransferObject[archetypes.Length];

        for (int i = 0; i < archetypes.Length; i++)
        {
            ArchetypeDataTransferObject dtoArchetype = new();

            Archetype archetype = archetypes[i];

            AccountArchetypeComponents(archetype, dtoArchetype);
            AccountArchetypeEntities(archetype, dtoArchetype);

            _dtoWorld.ArchetypesAccounted[i] = dtoArchetype;
        }

        return this;
    }

    private WorldSerializer AccountRelations(ReadOnlySpan<RelationStorage> relationStorages)
    {
        _dtoWorld.RelationsAccounted = new RelationDataTransferObject[relationStorages.Length];

        //@TO-DO

        return this;
    }

    private void AccountArchetypeComponents(
        Archetype archetype,
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

        archetypeDto.ComponentSet = components;
        archetypeDto.ComponentData = new byte[componentsLength][];
    }

    private void AccountArchetypeEntities(
        Archetype archetype,
        ArchetypeDataTransferObject archetypeDto
    )
    {
        ReadOnlySpan<int> componentsIds = archetype._signature.GetComponents();

        for (int i = 0; i < componentsIds.Length; i++)
            _componentWriters[componentsIds[i]].ResetWrittenCount();

        ReadOnlySpan<Indexer> indexers = archetype.GetIndexers();

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int length = indexer.Count();

            if (length == 0)
                continue;

            _tempEntities.AddRange(indexer.GetValues());

            for (int j = 0; j < componentsIds.Length; j++)
            {
                Storage storage = archetype.GetStorage(
                    archetype.GetStorageIndex(componentsIds[j]),
                    i
                );

                storage.Write(_componentWriters[componentsIds[j]], length);
            }
        }

        archetypeDto.StaleEntities = _tempEntities.ToArray();

        for (int i = 0; i < componentsIds.Length; i++)
            archetypeDto.ComponentData![i] = _componentWriters[componentsIds[i]]
                .WrittenSpan.ToArray();

        _tempEntities.Clear();
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
