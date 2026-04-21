using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.World;

[Generator]
public class WorldGeneratorQueries : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        sb.Append(
            @"
using System;
using System.Threading;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Queries;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
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
                "        "
            );

            sb.AppendLine(CreateSettlerQueryTemplate(generics, constraints));
            sb.AppendLine(
                CreateNomadQueryTemplate(
                    i,
                    generics,
                    constraints,
                    Generator.VariablesDefinitionComponentsIds(
                        i,
                        Constants.GenericComponentLocalPrefix,
                        Constants.GenericComponentPrefix
                    ),
                    Generator.VariablesAccessComponentsIds(i, Constants.GenericComponentLocalPrefix)
                )
            );
        }

        sb.Append(
            @"}
        "
        );

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"World.Queries.g.cs", sb.ToString())
        );
    }

    private static string CreateSettlerQueryTemplate(string generics, string constraints) =>
        $$"""
    public SettlerQuery{{generics}} CreateSettlerQuery{{generics}}(
        Assemblage{{generics}} assemblage
    )
{{constraints}}
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        return new SettlerQuery{{generics}}(this, assemblage);
    }   

""";

    private static string CreateNomadQueryTemplate(
        int count,
        string generics,
        string constraints,
        string variablesDefinition,
        string variablesAccess
    ) =>
        $$"""
    public NomadQuery{{generics}} CreateNomadQuery{{generics}}()
{{constraints}}
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

{{variablesDefinition}}

        ReadOnlySpan<int> componentIds = stackalloc int[{{count}}] { {{variablesAccess}} };

        NomadQuery{{generics}} nomadQuery = new NomadQuery{{generics}}(this, componentIds);

        AddNomadQuery(nomadQuery);

        AddSatisfiedArchetypes(nomadQuery);

        return nomadQuery;
    }

""";
}
