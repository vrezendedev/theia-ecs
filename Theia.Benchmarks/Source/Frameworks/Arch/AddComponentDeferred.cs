using System.Collections.Generic;
using Arch.Buffer;
using Arch.Core;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchAddComponentDeferredOnT3 : AddComponentDeferredOnT3
{
    private World? _world;
    private List<Entity>? _entities;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();
        _commandBuffer = new CommandBuffer();

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
            _commandBuffer!.Add(_entities![i], new Component4() { Value = i });

        _commandBuffer!.Playback(_world!);
    }
}
