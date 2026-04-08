using System.Collections.Generic;
using Arch.Core;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchSetComponentOnT1 : SetComponentOnT1
{
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(_world.Create(new Component1 { Value = i }));
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
            _world!.Set(_entities![i], new Component1() { Value = i });
    }
}
