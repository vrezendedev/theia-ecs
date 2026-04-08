using BenchmarkDotNet.Attributes;
using fennecs;
using Theia.Benchmarks.Source.Categories;

namespace Theia.Benchmarks.Source.Frameworks.Fennecs;

public class FennecsWorldConstructor : WorldConstructor
{
    [Benchmark]
    public override void Run()
    {
        World world = new World(InitialSize);
    }
}
