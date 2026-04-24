using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.Queries;

[Generator]
public class QueryGeneratorJobs : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        sb.Append(
            @"
#nullable enable

using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Jobs;

namespace Theia.ECS.Queries;

"
        );

        for (
            int i = Constants.ComponentSetOverloadInitialIndex;
            i <= Constants.OverloadMaxRange;
            i++
        )
        {
            string generics = Generator
                .Generics(i, Constants.GenericComponentPrefix)
                .Replace("<", string.Empty)
                .Replace(">", string.Empty);
            ;
            string constraints = Generator.Constraints(
                i,
                Constants.GenericComponentPrefix,
                "struct",
                "    "
            );

            sb.AppendLine(ForEachEntityJob(i, generics, constraints));
            sb.AppendLine(ForEachJob(i, generics, constraints));
        }

        sb.Append(
            @"
#nullable disable
    "
        );

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"Query.Jobs.g.cs", sb.ToString())
        );
    }

    private static string ForEachJobComponentStoragesFields(int count) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"    internal Storage<{Constants.GenericComponentPrefix}{i}>? {Constants.GenericJobStorageComponentFieldPrefix}{i};"
                )
        );

    private static string ForEachJobComponentStoragesSpan(int count) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"        Span<{Constants.GenericComponentPrefix}{i}> storageT{i} = {Constants.GenericJobStorageComponentFieldPrefix}{i}!.GetValues(count);"
                )
        );

    private static string ForEachJobQueriesComponentParams(int count) =>
        string.Join(", ", Enumerable.Range(1, count).Select(i => $"ref storageT{i}[i]"));

    private static string ForEachEntityJob(int count, string generics, string constraints) =>
        $$"""
internal sealed class ForEachEntityJob<TForEach, {{generics}}> : Job
    where TForEach : struct, IForEachEntity<{{generics}}>
{{constraints}}
{
    internal TForEach ForEach;
    internal Indexer? Indexer;
{{ForEachJobComponentStoragesFields(count)}}
    internal int Count;

    public override void Execute()
    {
        int count = Count;
        TForEach forEach = ForEach;

        Span<Entity> entities = Indexer!.GetValues();
{{ForEachJobComponentStoragesSpan(count)}}

        for (int i = 0; i < count; i++)
            forEach.Execute(entities[i], {{ForEachJobQueriesComponentParams(count)}});
    }
}
""";

    private static string ForEachJob(int count, string generics, string constraints) =>
        $$"""
internal sealed class ForEachJob<TForEach, {{generics}}> : Job
    where TForEach : struct, IForEach<{{generics}}>
{{constraints}}
{
    internal TForEach ForEach;
{{ForEachJobComponentStoragesFields(count)}}
    internal int Count;

    public override void Execute()
    {
        int count = Count;
        TForEach forEach = ForEach;

{{ForEachJobComponentStoragesSpan(count)}}

        for (int i = 0; i < count; i++)
            forEach.Execute({{ForEachJobQueriesComponentParams(count)}});
    }
}
""";
}
