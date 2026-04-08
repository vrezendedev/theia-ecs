using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaDestroyEntityDeferredT1 : DestroyEntityDeferredT1
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
            _entities.Add(_assemblage.Create(new Component1 { Value = i }));
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
            _world!.DeferredGhoulify(_entities![i]);

        _world!.FlushDeferred();
    }
}

public class TheiaDestroyEntityDeferredT3 : DestroyEntityDeferredT3
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
                _assemblage.Create(
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
            _world!.DeferredGhoulify(_entities![i]);

        _world!.FlushDeferred();
    }
}

public class TheiaDestroyEntityDeferredT5 : DestroyEntityDeferredT5
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
                _assemblage.Create(
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
            _world!.DeferredGhoulify(_entities![i]);

        _world!.FlushDeferred();
    }
}
