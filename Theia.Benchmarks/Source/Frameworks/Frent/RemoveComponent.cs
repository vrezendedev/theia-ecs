using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Frent;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Frent;

public class FrentRemoveComponentOnT2 : RemoveComponentOnT2
{
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _world.Create(new Component1 { Value = i }, new Component2 { Value = i })
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _world = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entities![i].Remove<Component1>();
    }
}

public class FrentRemoveComponentOnT3 : RemoveComponentOnT3
{
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _world.Create(
                    new Component1 { Value = i },
                    new Component2 { Value = i },
                    new Component3 { Value = i }
                )
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _world = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entities![i].Remove<Component1>();
    }
}

public class FrentRemoveComponentOnT5 : RemoveComponentOnT5
{
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _world.Create(
                    new Component1 { Value = i },
                    new Component2 { Value = i },
                    new Component3 { Value = i },
                    new Component4 { Value = i },
                    new Component5 { Value = i }
                )
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _world = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entities![i].Remove<Component1>();
    }
}
