using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Columns;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntitiesDbWorldConstructor : WorldConstructor
{
    [Benchmark]
    [Warning("Initial Size not Applied")]
    public override void Run()
    {
        EntityDatabase db = new EntityDatabase(new());
    }
}
