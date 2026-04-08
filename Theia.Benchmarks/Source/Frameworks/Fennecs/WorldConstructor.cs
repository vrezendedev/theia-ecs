using BenchmarkDotNet.Attributes;
using fennecs;
using Theia.Benchmarks.Source.Categories;

namespace Theia.Benchmarks.Source.Frameworks.Fennecs;

public class FennecsWorldConstructor : WorldConstructor
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
