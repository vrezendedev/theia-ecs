using BenchmarkDotNet.Attributes;
using fennecs;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Fennecs;

public class FennecsCreateEntityT1 : CreateEntityT1
{
    private World? _world;

    public override void Setup()
    {
        _world = new World();
    }

    public override void CleanUp()
    {
        _world = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!.Spawn().Add(new Component1 { Value = i });
    }
}

public class FennecsCreateEntityT3 : CreateEntityT3
{
    private World? _world;

    public override void Setup()
    {
        _world = new World();
    }

    public override void CleanUp()
    {
        _world = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!
                .Spawn()
                .Add(new Component1 { Value = i })
                .Add(new Component2 { Value = i })
                .Add(new Component3 { Value = i });
    }
}

public class FennecsCreateEntityT5 : CreateEntityT5
{
    private World? _world;

    public override void Setup()
    {
        _world = new World();
    }

    public override void CleanUp()
    {
        _world = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!
                .Spawn()
                .Add(new Component1 { Value = i })
                .Add(new Component2 { Value = i })
                .Add(new Component3 { Value = i })
                .Add(new Component4 { Value = i })
                .Add(new Component5 { Value = i });
    }
}
