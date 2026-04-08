using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaCreateEntityT1 : CreateEntityT1
{
    private World? _world;
    private Assemblage<Component1>? _assemblage;

    public override void Setup()
    {
        _world = new World();
        _assemblage = _world.CreateAssemblage<Component1>();
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblage = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _assemblage!.Create(new Component1 { Value = i });
    }
}

public class TheiaCreateEntityT3 : CreateEntityT3
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3>? _assemblage;

    public override void Setup()
    {
        _world = new World();
        _assemblage = _world.CreateAssemblage<Component1, Component2, Component3>();
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblage = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _assemblage!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );
    }
}

public class TheiaCreateEntityT5 : CreateEntityT5
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3, Component4, Component5>? _assemblage;

    public override void Setup()
    {
        _world = new World();
        _assemblage = _world.CreateAssemblage<
            Component1,
            Component2,
            Component3,
            Component4,
            Component5
        >();
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblage = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _assemblage!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );
    }
}
