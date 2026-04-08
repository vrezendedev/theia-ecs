using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloAddComponentDeferredOnT3 : AddComponentDeferredOnT3
{
    private EntityStore? _entityStore;
    private List<Entity>? _entities;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();
        _commandBuffer = _entityStore.GetCommandBuffer();

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _entityStore.CreateEntity(
                    new FComponent1 { Value = i },
                    new FComponent2 { Value = i },
                    new FComponent3 { Value = i }
                )
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityStore = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _commandBuffer!.AddComponent(_entities![i].Id, new FComponent4() { Value = i });

        _commandBuffer!.Playback();
    }
}
