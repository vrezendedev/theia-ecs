using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using fennecs;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Fennecs;

public class FennecsAddComponentOnT1 : AddComponentOnT1
{
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(_world.Spawn().Add(new Component1 { Value = i }));
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
            _entities![i].Add(new Component2());
    }
}

public class FennecsAddComponentOnT3 : AddComponentOnT3
{
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _world
                    .Spawn()
                    .Add(new Component1 { Value = i })
                    .Add(new Component2 { Value = i })
                    .Add(new Component3 { Value = i })
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
            _entities![i].Add(new Component4());
    }
}

public class FennecsAddComponentOnT5 : AddComponentOnT5
{
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _world
                    .Spawn()
                    .Add(new Component1 { Value = i })
                    .Add(new Component2 { Value = i })
                    .Add(new Component3 { Value = i })
                    .Add(new Component4 { Value = i })
                    .Add(new Component5 { Value = i })
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
            _entities![i].Add(new Component6());
    }
}
