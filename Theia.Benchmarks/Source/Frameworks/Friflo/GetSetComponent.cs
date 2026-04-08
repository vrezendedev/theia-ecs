using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloGetComponentOnT1 : GetComponentOnT1
{
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(_entityStore.CreateEntity(new FComponent1 { Value = i }));
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
        {
            ref FComponent1 component1 = ref _entities![i].GetComponent<FComponent1>();
            component1.Value += 1;
        }
    }
}

public class FrifloSetComponentOnT1 : SetComponentOnT1
{
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(_entityStore.CreateEntity(new FComponent1 { Value = i }));
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
            _entities![i].Set(new FComponent1() { Value = i });
    }
}
