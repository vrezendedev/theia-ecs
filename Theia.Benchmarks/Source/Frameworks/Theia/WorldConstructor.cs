using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaWorldConstructor : WorldConstructor
{
    private World? _world;

    public override void CleanUp()
    {
        _world = null;
    }

    [Benchmark]
    public override void Run()
    {
        _world = new World(InitialSize);
    }
}
