using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaAddComponentDeferredOnT3 : AddComponentDeferredOnT3
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
            _world!.DeferredAddComponent(_entities![i], new Component4());

        _world!.FlushDeferred();
    }
}
