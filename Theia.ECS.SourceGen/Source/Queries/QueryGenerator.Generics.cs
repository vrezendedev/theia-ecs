using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.Queries;

[Generator]
public class QueryGeneratorGenerics : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"Query.Generics.g.cs", sb.ToString())
        );
    }
}
