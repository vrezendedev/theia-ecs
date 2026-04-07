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
            i <= Constants.ComponentSetOverloadMaxRange;
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
            string variablesStorages = SettlerQueryStorages(
                i,
                Constants.QueryStoragesVariablePrefix
            );
            string variablesStorage = QueriesStorageAccess(
                i,
                Constants.GenericComponentPrefix,
                Constants.QueryStorageVariablePrefix,
                Constants.QueryStoragesVariablePrefix
            );
            string componentsParams = QueriesComponentParams(
                i,
                "ref",
                Constants.QueryStorageVariablePrefix
            );

            sb.AppendLine(
                SettlerQueryTemplate(
                    generics,
                    constraints,
                    variablesStorages,
                    variablesStorage,
                    componentsParams
                )
            );

            //@TO-DO Nomad Query
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

    private static string QueriesStorageAccess(
        int count,
        string genericPrefix,
        string storagePrefix,
        string storagesPrefix
    ) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"            Span<{genericPrefix}{i}> {storagePrefix}{i} = ((Storage<{genericPrefix}{i}>){storagesPrefix}{i}[i]).GetValues(count);"
                )
        );

    private static string QueriesComponentParams(
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
        string componentsParams
    ) =>
        $$"""
public sealed class SettlerQuery{{generics}} : SettlerQuery
{{constraints}}
{

    internal SettlerQuery(in World world, in Assemblage{{generics}} assemblage)
        : base(world, assemblage) { }

    public void ForEachEntity(ForEachEntity{{generics}} forEachEntity)
    {
        _world.IncrementQueriesBeingExecuted();

        Archetype archetype = _archetype;

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Indexer> indexers = archetype.GetIndexers();

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
               forEachEntity(entities[j], {{componentsParams}});
        }

        _world.DecrementQueriesBeingExecuted();
    }

    public void ForEach(ForEach{{generics}} forEach)
    {
        _world.IncrementQueriesBeingExecuted();

        Archetype archetype = _archetype;

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Indexer> indexers = archetype.GetIndexers();

{{storagesVariables}}

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int count = indexer.Count();

            if (count == 0)
                continue;
        
{{storageVariables}}

            for (int j = 0; j < count; j++)
               forEach({{componentsParams}});
        }

        _world.DecrementQueriesBeingExecuted();
    }

}
""";
}
