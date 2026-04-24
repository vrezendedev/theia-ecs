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
            string parameters = Generator.Parameters(
                i,
                "ref",
                Constants.GenericComponentPrefix,
                Constants.GenericComponentLocalPrefix
            );

            sb.AppendLine(IForEachEntityTemplate(generics, constraints, parameters));
            sb.AppendLine(IForEachTemplate(generics, constraints, parameters));
        }

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"Query.Delegates.g.cs", sb.ToString())
        );
    }

    private static string IForEachEntityTemplate(
        string generics,
        string constraints,
        string parameters
    ) =>
        $$"""
public interface IForEachEntity{{generics}}
{{constraints}}
{
    public void Execute(Entity entity, {{parameters}});
}

""";

    private static string IForEachTemplate(
        string generics,
        string constraints,
        string parameters
    ) =>
        $$"""
public interface IForEach{{generics}}
{{constraints}}
{
    public void Execute({{parameters}});
}

""";
}
