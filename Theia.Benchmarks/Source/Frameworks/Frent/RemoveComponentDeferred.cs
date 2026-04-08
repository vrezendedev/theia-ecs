using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Frent;
using Frent.Core;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Frent;

public class FrentRemoveComponentDeferredOnT3 : RemoveComponentDeferredOnT3
{
    private World? _world;
    private List<Entity>? _entities;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entities = new();
        _world = new World();
        _commandBuffer = new CommandBuffer(_world);

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
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _commandBuffer!.RemoveComponent<Component3>(_entities![i]);

        _commandBuffer!.Playback();
    }
}
