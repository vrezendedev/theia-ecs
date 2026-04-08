using BenchmarkDotNet.Attributes;
using Frent;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Columns;

namespace Theia.Benchmarks.Source.Frameworks.Frent;

public class FrentWorldConstructor : WorldConstructor
{
    private World? _world;

    public override void CleanUp()
    {
        _world = null;
    }

    [Benchmark]
    [Warning("Initial Size not Applied")]
    public override void Run()
    {
        _world = new World();
    }
}
