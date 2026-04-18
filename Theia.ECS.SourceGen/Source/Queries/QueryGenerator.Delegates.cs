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

            sb.AppendLine(IForEachEntityTemplate(generics, constraints, arguments));
            sb.AppendLine(IForEachTemplate(generics, constraints, arguments));
        }

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"Query.Delegates.g.cs", sb.ToString())
        );
    }

    private static string IForEachEntityTemplate(
        string generics,
        string constraints,
        string arguments
    ) =>
        $$"""
public interface IForEachEntity{{generics}}
{{constraints}}
{
    public void Execute(Entity entity, {{arguments}});
}

""";

    private static string IForEachTemplate(string generics, string constraints, string arguments) =>
        $$"""
public interface IForEach{{generics}}
{{constraints}}
{
    public void Execute({{arguments}});
}

""";
}
