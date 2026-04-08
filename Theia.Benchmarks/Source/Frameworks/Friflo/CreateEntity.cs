using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloCreateEntityT1 : CreateEntityT1
{
    private EntityStore? _entityStore;

    public override void Setup()
    {
        _entityStore = new EntityStore();
    }

    public override void CleanUp()
    {
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entityStore.CreateEntity(new FComponent1 { Value = i });
    }
}

public class FrifloCreateEntityT3 : CreateEntityT3
{
    private EntityStore? _entityStore;

    public override void Setup()
    {
        _entityStore = new EntityStore();
    }

    public override void CleanUp()
    {
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entityStore.CreateEntity(
                new FComponent1 { Value = i },
                new FComponent2 { Value = i },
                new FComponent3 { Value = i }
            );
    }
}

public class FrifloCreateEntityT5 : CreateEntityT5
{
    private EntityStore? _entityStore;

    public override void Setup()
    {
        _entityStore = new EntityStore();
    }

    public override void CleanUp()
    {
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entityStore.CreateEntity(
                new FComponent1 { Value = i },
                new FComponent2 { Value = i },
                new FComponent3 { Value = i },
                new FComponent4 { Value = i },
                new FComponent5 { Value = i }
            );
    }
}
