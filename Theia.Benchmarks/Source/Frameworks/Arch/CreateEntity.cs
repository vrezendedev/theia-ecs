using Arch.Core;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchCreateEntityT1 : CreateEntityT1
{
    private World? _world;

    public override void Setup()
    {
        _world = World.Create();
    }

    public override void CleanUp()
    {
        _world = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!.Create(new Component1 { Value = i });
    }
}

public class ArchCreateEntityT3 : CreateEntityT3
{
    private World? _world;

    public override void Setup()
    {
        _world = World.Create();
    }

    public override void CleanUp()
    {
        _world = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );
    }
}

public class ArchCreateEntityT5 : CreateEntityT5
{
    private World? _world;

    public override void Setup()
    {
        _world = World.Create();
    }

    public override void CleanUp()
    {
        _world = null!;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _world!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );
    }
}
