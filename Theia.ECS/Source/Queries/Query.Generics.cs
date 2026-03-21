using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.ECS.Queries;

public sealed class SettlerQuery<ComponentT1> : SettlerQuery
    where ComponentT1 : struct
{
    internal SettlerQuery(in World world, in Assemblage<ComponentT1> assemblage)
        : base(world, assemblage) { }

    public void ForEachEntity(ForEachEntity<ComponentT1> forEachEntity)
    {
        _world.IncrementQueriesBeingExecuted();

        Archetype archetype = _archetype;

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Indexer> indexers = archetype.GetIndexers();

        Span<Storage> storagesT1 = archetype.GetStorages(mapping[0]);

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int count = indexer.Count();

            if (count == 0)
                continue;

            Span<Entity> entities = indexer.GetValues();

            Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(count);

            for (int j = 0; j < count; j++)
                forEachEntity(entities[j], ref storageT1[j]);
        }

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEach(ForEach<ComponentT1> forEach)
    {
        _world.IncrementQueriesBeingExecuted();

        Archetype archetype = _archetype;

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Indexer> indexers = archetype.GetIndexers();

        Span<Storage> storagesT1 = archetype.GetStorages(mapping[0]);

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int count = indexer.Count();

            if (count == 0)
                continue;

            Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(count);

            for (int j = 0; j < count; j++)
                forEach(ref storageT1[j]);
        }

        _world.DecrementQueriesBeingExecuted();
    }
}

public sealed class NomadQuery<ComponentT1> : NomadQuery
    where ComponentT1 : struct
{
    internal NomadQuery(in World world, ReadOnlySpan<int> componentIds)
        : base(world, componentIds) { }

    public void ForEachEntity(ForEachEntity<ComponentT1> forEachEntity)
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<Archetype> matchedArchetypesIds = GetMatchedArchetypes();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        foreach (Archetype archetype in matchedArchetypesIds)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            int storageT1Index = archetype.GetStorageIndex(componentT1Id);
            Span<Storage> storagesT1 = archetype.GetStorages(storageT1Index);

            for (int i = 0; i < indexers.Length; i++)
            {
                Indexer indexer = indexers[i];

                int count = indexer.Count();

                if (count == 0)
                    continue;

                Span<Entity> entities = indexer.GetValues();

                Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(
                    count
                );

                for (int j = 0; j < count; j++)
                    forEachEntity(entities[j], ref storageT1[j]);
            }
        }

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEach(ForEach<ComponentT1> forEach)
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<Archetype> matchedArchetypesIds = GetMatchedArchetypes();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        foreach (Archetype archetype in matchedArchetypesIds)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            int storageT1Index = archetype.GetStorageIndex(componentT1Id);
            Span<Storage> storagesT1 = archetype.GetStorages(storageT1Index);

            for (int i = 0; i < indexers.Length; i++)
            {
                Indexer indexer = indexers[i];

                int count = indexer.Count();

                if (count == 0)
                    continue;

                Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(
                    count
                );

                for (int j = 0; j < count; j++)
                    forEach(ref storageT1[j]);
            }
        }

        _world.DecrementQueriesBeingExecuted();
    }
}
