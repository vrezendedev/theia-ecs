using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.Queries;

[Generator]
public class QueryGeneratorDelegates : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        sb.Append(
            @"
using Theia.ECS.Entities;

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
            string arguments = Generator.Arguments(
                i,
                "ref",
                Constants.GenericComponentPrefix,
                Constants.GenericComponentLocalPrefix
            );
            string constraints = Generator.Constraints(
                i,
                Constants.GenericComponentPrefix,
                "struct",
                "    "
            );

            sb.AppendLine(ForEachEntityDelegateTemplate(generics, arguments, constraints));
            sb.AppendLine(ForEachDelegateTemplate(generics, arguments, constraints));
        }

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"Query.Delegates.g.cs", sb.ToString())
        );
    }

    private static string ForEachEntityDelegateTemplate(
        string generics,
        string arguments,
        string constraints
    ) =>
        $$"""
public delegate void ForEachEntity{{generics}}(
    Entity entity,
    {{arguments}}
)
{{constraints}};

""";

    private static string ForEachDelegateTemplate(
        string generics,
        string arguments,
        string constraints
    ) =>
        $$"""
public delegate void ForEach{{generics}}(
    {{arguments}}
)
{{constraints}};

""";
}
