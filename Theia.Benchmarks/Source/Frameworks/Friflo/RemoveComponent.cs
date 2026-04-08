using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloRemoveComponentOnT2 : RemoveComponentOnT2
{
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _entityStore.CreateEntity(
                    new FComponent1 { Value = i },
                    new FComponent2 { Value = i }
                )
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entities![i].Remove<FComponent1>();
    }
}

public class FrifloRemoveComponentOnT3 : RemoveComponentOnT3
{
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _entityStore.CreateEntity(
                    new FComponent1 { Value = i },
                    new FComponent2 { Value = i },
                    new FComponent3 { Value = i }
                )
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entities![i].Remove<FComponent1>();
    }
}

public class FrifloRemoveComponentOnT5 : RemoveComponentOnT5
{
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _entityStore.CreateEntity(
                    new FComponent1 { Value = i },
                    new FComponent2 { Value = i },
                    new FComponent3 { Value = i },
                    new FComponent4 { Value = i },
                    new FComponent5 { Value = i }
                )
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entities![i].Remove<FComponent1>();
    }
}
