using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Columns;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntitiesDbWorldConstructor : WorldConstructor
{
    private EntityDatabase? _db;

    public override void CleanUp()
    {
        _db = null;
    }

    [Benchmark]
    [Warning("Initial Size not Applied")]
    public override void Run()
    {
        _db = new EntityDatabase(new(16384, int.MaxValue));
    }
}
