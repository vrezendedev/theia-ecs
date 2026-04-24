using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.Queries;

[Generator]
public class QueryGeneratorGenerics : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        sb.Append(
            @"
using System;
using System.Buffers;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Jobs;
using Theia.ECS.Worlds;

namespace Theia.ECS.Queries;

"
        );

        for (
            int i = Constants.ComponentSetOverloadInitialIndex;
            i <= Constants.OverloadMaxRange;
            i++
        )
        {
            string generics = Generator.Generics(i, Constants.GenericComponentPrefix);
            string jobGenerics = generics.Replace("<", string.Empty).Replace(">", string.Empty);
            string constraints = Generator.Constraints(
                i,
                Constants.GenericComponentPrefix,
                "struct",
                "    "
            );
            string componentsArgs = QueriesComponentArguments(
                i,
                "ref",
                Constants.QueryStorageVariablePrefix
            );

            sb.AppendLine(
                SettlerQueryTemplate(
                    i,
                    generics,
                    jobGenerics,
                    constraints,
                    SettlerQueryStorages(i, Constants.QueryStoragesVariablePrefix),
                    QueriesStorageAccess(
                        i,
                        Constants.GenericComponentPrefix,
                        Constants.QueryStorageVariablePrefix,
                        Constants.QueryStoragesVariablePrefix,
                        "            "
                    ),
                    componentsArgs
                )
            );

            sb.AppendLine(
                NomadQueryTemplate(
                    i,
                    generics,
                    jobGenerics,
                    constraints,
                    Generator.VariablesDefinitionComponentsIds(
                        i,
                        Constants.GenericComponentLocalPrefix,
                        Constants.GenericComponentPrefix
                    ),
                    NomadQueryStorages(
                        i,
                        Constants.QueryStoragesVariablePrefix,
                        Constants.QueryStorageIndexVariableSufix,
                        Constants.GenericComponentLocalPrefix
                    ),
                    QueriesStorageAccess(
                        i,
                        Constants.GenericComponentPrefix,
                        Constants.QueryStorageVariablePrefix,
                        Constants.QueryStoragesVariablePrefix,
                        "                "
                    ),
                    componentsArgs
                )
            );
        }

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"Query.Generics.g.cs", sb.ToString())
        );
    }

    private static string SettlerQueryStorages(int count, string variablePrefix) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"        Span<Storage> {variablePrefix}{i} = archetype.GetStorages(mapping[{i - 1}]);"
                )
        );

    private static string NomadQueryStorages(
        int count,
        string storagesPrefix,
        string storageIndexSufix,
        string componentIdVariablePrefix
    ) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $$"""
            int {{storagesPrefix}}{{i}}{{storageIndexSufix}} = archetype.GetStorageIndex({{componentIdVariablePrefix}}{{i}}{{Constants.ComponentIdVariableSuffix}});
            Span<Storage> {{storagesPrefix}}{{i}} = archetype.GetStorages({{storagesPrefix}}{{i}}{{storageIndexSufix}});
"""
                )
        );

    private static string QueriesStorageAccess(
        int count,
        string genericPrefix,
        string storagePrefix,
        string storagesPrefix,
        string tabs
    ) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"{tabs}Span<{genericPrefix}{i}> {storagePrefix}{i} = ((Storage<{genericPrefix}{i}>){storagesPrefix}{i}[i]).GetValues(count);"
                )
        );

    private static string QueriesComponentArguments(
        int count,
        string paramScope,
        string storagePrefix
    ) =>
        string.Join(
            ", ",
            Enumerable.Range(1, count).Select(i => $"{paramScope} {storagePrefix}{i}[j]")
        );

    private static string JobStoragesInit(int count, string tabs) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"{tabs}job.StoragesComponentT{i} = (Storage<ComponentT{i}>)storagesT{i}[i];"
                )
        );

    private static string JobStoragesNullify(int count) =>
        string.Join(
            "\n",
            Enumerable.Range(1, count).Select(i => $"            job.StoragesComponentT{i} = null;")
        );

    private static string SettlerQueryTemplate(
        int count,
        string generics,
        string jobGenerics,
        string constraints,
        string storagesVariables,
        string storageVariables,
        string componentsArgs
    ) =>
        $$"""
public sealed class SettlerQuery{{generics}} : SettlerQuery
{{constraints}}
{

    internal SettlerQuery(in World world, in Assemblage{{generics}} assemblage)
        : base(world, assemblage) { }

    public void ForEachEntity<T>(ref T forEachEntity)
        where T : struct, IForEachEntity{{generics}}, allows ref struct
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        if (indexers.Length == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

{{storagesVariables}}

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int count = indexer.Count();

            if (count == 0)
                continue;

            Span<Entity> entities = indexer.GetValues();
        
{{storageVariables}}

            for (int j = 0; j < count; j++)
               forEachEntity.Execute(entities[j], {{componentsArgs}});
        }

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEachEntityParallel<T>(T forEachEntity)
        where T : struct, IForEachEntity{{generics}}
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        int indexersCount = indexers.Length;

        if (indexersCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

{{storagesVariables}}

        Job[] jobs = ArrayPool<Job>.Shared.Rent(indexersCount);

        int jobCount = 0;

        for (int i = 0; i < indexersCount; i++)
        {
            int count = indexers[i].Count();

            if (count == 0)
                continue;

            ForEachEntityJob<T, {{jobGenerics}}> job = JobPool<ForEachEntityJob<T, {{jobGenerics}}>>.Rent();
          
            job.ForEach = forEachEntity;
            job.Indexer = indexers[i];
{{JobStoragesInit(count, "            ")}}
            job.Count = count;

            jobs[jobCount] = job;
            jobCount++;
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachEntityJob<T, {{jobGenerics}}> job = (ForEachEntityJob<T, {{jobGenerics}}>)jobs[i];
           
            job.Indexer = null;
{{JobStoragesNullify(count)}}

            JobPool<ForEachEntityJob<T, {{jobGenerics}}>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEach<T>(ref T forEach)
        where T : struct, IForEach{{generics}}, allows ref struct
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        if (indexers.Length == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

{{storagesVariables}}

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int count = indexer.Count();

            if (count == 0)
                continue;
        
{{storageVariables}}

            for (int j = 0; j < count; j++)
               forEach.Execute({{componentsArgs}});
        }

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEachParallel<T>(T forEach)
        where T : struct, IForEach{{generics}}
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        int indexersCount = indexers.Length;

        if (indexersCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

{{storagesVariables}}

        Job[] jobs = ArrayPool<Job>.Shared.Rent(indexersCount);

        int jobCount = 0;

        for (int i = 0; i < indexersCount; i++)
        {
            int count = indexers[i].Count();

            if (count == 0)
                continue;

            ForEachJob<T, {{jobGenerics}}> job = JobPool<ForEachJob<T, {{jobGenerics}}>>.Rent();
            
            job.ForEach = forEach;
{{JobStoragesInit(count, "            ")}}
            job.Count = count;

            jobs[jobCount] = job;
            jobCount++;
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachJob<T, {{jobGenerics}}> job = (ForEachJob<T, {{jobGenerics}}>)jobs[i];

{{JobStoragesNullify(count)}}

            JobPool<ForEachJob<T, {{jobGenerics}}>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }
}

""";

    private static string NomadQueryTemplate(
        int count,
        string generics,
        string jobGenerics,
        string constraints,
        string componentIds,
        string storages,
        string storage,
        string componentsArgs
    ) =>
        $$"""
public sealed class NomadQuery{{generics}} : NomadQuery
{{constraints}}
{
    internal NomadQuery(in World world, ReadOnlySpan<int> componentIds)
        : base(world, componentIds) { }

    public void ForEachEntity<T>(ref T forEachEntity)
        where T : struct, IForEachEntity{{generics}}, allows ref struct
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<Archetype> matchedArchetypesIds = GetMatchedArchetypes();

{{componentIds}}

        foreach (Archetype archetype in matchedArchetypesIds)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

{{storages}}

            for (int i = 0; i < indexers.Length; i++)
            {
                Indexer indexer = indexers[i];

                int count = indexer.Count();

                if (count == 0)
                    continue;

                Span<Entity> entities = indexer.GetValues();

{{storage}}

                for (int j = 0; j < count; j++)
                    forEachEntity.Execute(entities[j], {{componentsArgs}});
            }
        }

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEachEntityParallel<T>(T forEachEntity)
        where T : struct, IForEachEntity{{generics}}
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        ReadOnlySpan<Archetype> matchedArchetypes = GetMatchedArchetypes();

        int totalIndexers = 0;

        for (int i = 0; i < matchedArchetypes.Length; i++)
        {
            Archetype archetype = matchedArchetypes[i];
            totalIndexers += archetype.GetInitializedCount();
        }

        if (totalIndexers == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

{{componentIds}}

        Job[] jobs = ArrayPool<Job>.Shared.Rent(totalIndexers);

        int jobCount = 0;

        foreach (Archetype archetype in matchedArchetypes)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

{{storages}}

            for (int i = 0; i < indexers.Length; i++)
            {
                int count = indexers[i].Count();

                if (count == 0)
                    continue;

                ForEachEntityJob<T, {{jobGenerics}}> job = JobPool<
                    ForEachEntityJob<T, {{jobGenerics}}>
                >.Rent();

                job.ForEach = forEachEntity;
                job.Indexer = indexers[i];
{{JobStoragesInit(count, "                ")}}
                job.Count = count;

                jobs[jobCount] = job;
                jobCount++;
            }
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachEntityJob<T, {{jobGenerics}}> job = (ForEachEntityJob<T, {{jobGenerics}}>)jobs[i];

            job.Indexer = null;
{{JobStoragesNullify(count)}}

            JobPool<ForEachEntityJob<T, {{jobGenerics}}>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEach<T>(ref T forEach)
        where T : struct, IForEach{{generics}}, allows ref struct
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<Archetype> matchedArchetypesIds = GetMatchedArchetypes();

{{componentIds}}

        foreach (Archetype archetype in matchedArchetypesIds)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

{{storages}}

            for (int i = 0; i < indexers.Length; i++)
            {
                Indexer indexer = indexers[i];

                int count = indexer.Count();

                if (count == 0)
                    continue;

{{storage}}

                for (int j = 0; j < count; j++)
                    forEach.Execute({{componentsArgs}});
            }
        }

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEachParallel<T>(T forEach)
        where T : struct, IForEach{{generics}}
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        ReadOnlySpan<Archetype> matchedArchetypes = GetMatchedArchetypes();

        int totalIndexers = 0;

        for (int i = 0; i < matchedArchetypes.Length; i++)
        {
            Archetype archetype = matchedArchetypes[i];
            totalIndexers += archetype.GetInitializedCount();
        }

        if (totalIndexers == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

{{componentIds}}

        Job[] jobs = ArrayPool<Job>.Shared.Rent(totalIndexers);

        int jobCount = 0;

        foreach (Archetype archetype in matchedArchetypes)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

{{storages}}

            for (int i = 0; i < indexers.Length; i++)
            {
                int count = indexers[i].Count();

                if (count == 0)
                    continue;

                ForEachJob<T, {{jobGenerics}}> job = JobPool<ForEachJob<T, {{jobGenerics}}>>.Rent();
                
                job.ForEach = forEach;
{{JobStoragesInit(count, "                ")}}
                job.Count = count;

                jobs[jobCount] = job;
                jobCount++;
            }
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachJob<T, {{jobGenerics}}> job = (ForEachJob<T, {{jobGenerics}}>)jobs[i];

{{JobStoragesNullify(count)}}

            JobPool<ForEachJob<T, {{jobGenerics}}>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }
}

""";
}
