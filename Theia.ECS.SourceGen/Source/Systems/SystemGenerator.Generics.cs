using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.Systems;

[Generator]
public class SystemGeneratorGenerics : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        sb.Append(
            @"
using System.Runtime.CompilerServices;
using Theia.ECS.Queries;
        
namespace Theia.ECS.Systems;

"
        );

        for (int i = Constants.QuerySetOverloadInitialIndex; i <= Constants.OverloadMaxRange; i++)
        {
            string generics = Generator
                .Generics(i, Constants.GenericQueryPrefix)
                .Replace("<", string.Empty)
                .Replace(">", string.Empty);
            string constructorParams = Generator.Parameters(
                i,
                "in",
                Constants.GenericQueryPrefix,
                Constants.GenericQueryLocalPrefix
            );
            string constraints = Generator.Constraints(
                i,
                Constants.GenericQueryPrefix,
                "Query",
                "    "
            );
            string fields = Generator.Fields(
                i,
                "private readonly",
                Constants.GenericQueryPrefix,
                Constants.GenericQueryFieldPrefix,
                Constants.GenericQueryLocalPrefix,
                "    "
            );

            sb.AppendLine(SystemTemplate(i, generics, constructorParams, constraints, fields));
        }

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"System.Generics.g.cs", sb.ToString())
        );
    }

    private static string ExecuteQueryMethodCall(int count) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"        Execute{Constants.GenericQueryPrefix}{i}(ref sharedData, in {Constants.GenericQueryFieldPrefix}{i});"
                )
        );

    private static string ExecuteQueryMethodTemplate(int count) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $$"""
                    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Execute{{Constants.GenericQueryPrefix}}{{i}}(ref TSharedData sharedData, in {{Constants.GenericQueryPrefix}}{{i}} query);
"""
                )
        );

    private static string SystemTemplate(
        int count,
        string generics,
        string constructorParams,
        string constraints,
        string fields
    ) =>
        $$"""
public abstract class System<TSharedData, {{generics}}>({{constructorParams}})
    : BaseSystem
    where TSharedData : struct, allows ref struct
{{constraints}}
{
{{fields}}

    internal override void Run()
    {
        TSharedData sharedData = CreateData();

{{ExecuteQueryMethodCall(count)}}
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual TSharedData CreateData() => default;
{{ExecuteQueryMethodTemplate(count)}}
}

""";
}
