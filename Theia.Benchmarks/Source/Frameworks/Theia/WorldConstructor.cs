using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaWorldConstructor : WorldConstructor
{
    [Benchmark]
    public override void Run()
    {
        World world = new World(InitialSize);
    }
}
