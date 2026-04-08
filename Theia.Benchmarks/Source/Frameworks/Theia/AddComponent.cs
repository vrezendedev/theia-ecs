using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaAddComponentOnT1 : AddComponentOnT1
{
    private World? _world;
    private Assemblage<Component1>? _assemblage;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();
        _assemblage = _world.CreateAssemblage<Component1>();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(_assemblage!.Create(new Component1 { Value = i }));
    }

    public override void CleanUp()
    {
        _entities = null;
        _world = null;
        _assemblage = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!.TryAddComponent(_entities![i], new Component2());
    }
}

public class TheiaAddComponentOnT3 : AddComponentOnT3
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3>? _assemblage;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();
        _assemblage = _world.CreateAssemblage<Component1, Component2, Component3>();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _assemblage!.Create(
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
        _assemblage = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!.TryAddComponent(_entities![i], new Component4());
    }
}

public class TheiaAddComponentOnT5 : AddComponentOnT5
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3, Component4, Component5>? _assemblage;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();
        _assemblage = _world.CreateAssemblage<
            Component1,
            Component2,
            Component3,
            Component4,
            Component5
        >();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _assemblage!.Create(
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
        _assemblage = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!.TryAddComponent(_entities![i], new Component6());
    }
}
