using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.World;

[Generator]
public class WorldGeneratorAssemblages : IIncrementalGenerator
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

namespace Theia.ECS.Worlds;

public sealed partial class World
{
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
                "        "
            );
            string variablesDefinition = Generator.VariablesDefinitionComponentsIds(
                i,
                Constants.GenericComponentLocalPrefix,
                Constants.GenericComponentPrefix
            );
            string variablesAccess = Generator.VariablesAccessComponentsIds(
                i,
                Constants.GenericComponentLocalPrefix
            );
            string componentStorageMapping = ComponentStorageMapping(
                i,
                Constants.GenericComponentLocalPrefix
            );

            sb.AppendLine(
                CreateAssemblageTemplate(
                    i,
                    generics,
                    constraints,
                    variablesDefinition,
                    variablesAccess,
                    componentStorageMapping
                )
            );
        }

        sb.Append(
            @"}
        "
        );

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"World.Assemblages.g.cs", sb.ToString())
        );
    }

    private static string ComponentStorageMapping(int count, string variablePrefix) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"        componentStorageMapping[{i - 1}] = archetype.GetStorageIndex({variablePrefix}{i}{Constants.ComponentIdVariableSuffix});"
                )
        );

    private static string CreateAssemblageTemplate(
        int count,
        string generics,
        string constraints,
        string variablesDefinitions,
        string variablesAccess,
        string componentStorageMapping
    ) =>
        $$"""
    public Assemblage{{generics}} CreateAssemblage{{generics}}()
{{constraints}}
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

{{variablesDefinitions}}

        Archetype archetype = FindOrCreateArchetype(stackalloc int[{{count}}] { {{variablesAccess}} });

        Span<int> componentStorageMapping = stackalloc int[{{count}}];

{{componentStorageMapping}}

        Assemblage{{generics}} assemblage = new Assemblage{{generics}}(
            this,
            in archetype,
            componentStorageMapping
        );

        AddAssemblage(assemblage);

        if (!archetype.TrySetMatchedAssemblage(assemblage))
            ThrowDuplicatedAssemblage();

        return assemblage;
    }

""";
}
