using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.Contracts;

[Generator]
public class ContractsGeneratorEntityDeferred : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        sb.Append(
            @"
using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

"
        );

        for (
            int i = Constants.ComponentSetOverloadInitialIndex;
            i <= Constants.OverloadMaxRange;
            i++
        )
        {
            string generics = Generator.Generics(i, Constants.GenericComponentPrefix);
            string fields = Fields(
                i,
                Constants.GenericComponentPrefix,
                Constants.GenericComponentLocalPrefix
            );
            string constraints = Generator.Constraints(
                i,
                Constants.GenericComponentPrefix,
                "struct",
                "    "
            );

            sb.AppendLine(EntityCreateDeferredTemplate(generics, fields, constraints));
        }

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"EntityDeferred.g.cs", sb.ToString())
        );
    }

    internal static string Fields(int count, string typePrefix, string fieldPrefix) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $$"""    internal readonly required {{typePrefix}}{{i}} _{{fieldPrefix}}{{i}} { get; init; } """
                )
        );

    private static string EntityCreateDeferredTemplate(
        string generics,
        string fields,
        string constraints
    ) =>
        $$"""
internal readonly struct EntityCreateDeferred{{generics}}
{{constraints}}
{
    internal readonly AddRelationDeferred _relationDeferred { get; init; }

{{fields}}

    public EntityCreateDeferred() => _relationDeferred = AddRelationDeferred.Invalid;
}

""";
}
