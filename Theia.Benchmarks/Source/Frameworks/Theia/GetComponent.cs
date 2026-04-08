using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaGetComponentOnT1 : GetComponentOnT1
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
        {
            ref Component1 component1 = ref _world!.Get<Component1>(_entities![i]);
            component1.Value += 1;
        }
    }
}
