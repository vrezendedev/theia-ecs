using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntitiesDbCreateEntityT1 : CreateEntityT1
{
    private EntityDatabase? _entityDatabase;

    public override void Setup()
    {
        _entityDatabase = new EntityDatabase(new(16384, int.MaxValue));
    }

    public override void CleanUp()
    {
        _entityDatabase = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entityDatabase!.Create(new Component1 { Value = i });
    }
}

public class EntitiesDbCreateEntityT3 : CreateEntityT3
{
    private EntityDatabase? _entityDatabase;

    public override void Setup()
    {
        _entityDatabase = new EntityDatabase(new(16384, int.MaxValue));
    }

    public override void CleanUp()
    {
        _entityDatabase = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entityDatabase!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );
    }
}

public class EntitiesDbCreateEntityT5 : CreateEntityT5
{
    private EntityDatabase? _entityDatabase;

    public override void Setup()
    {
        _entityDatabase = new EntityDatabase(new(16384, int.MaxValue));
    }

    public override void CleanUp()
    {
        _entityDatabase = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entityDatabase!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );
    }
}
