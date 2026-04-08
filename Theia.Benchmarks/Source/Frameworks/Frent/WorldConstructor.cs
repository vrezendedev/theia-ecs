using BenchmarkDotNet.Attributes;
using Frent;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Columns;

namespace Theia.Benchmarks.Source.Frameworks.Frent;

public class FrentWorldConstructor : WorldConstructor
{
    [Benchmark]
    [Warning("Initial Size not Applied")]
    public override void Run()
    {
        World world = new World();
    }
}
