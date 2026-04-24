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
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Entities;
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
                    generics,
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
                    generics,
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

    private static string SettlerQueryTemplate(
        string generics,
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
}

""";

    private static string NomadQueryTemplate(
        string generics,
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
}

""";
}
