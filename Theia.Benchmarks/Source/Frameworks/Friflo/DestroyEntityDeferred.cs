using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloDestroyEntityDeferredT1 : DestroyEntityDeferredT1
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
            _entities.Add(_entityStore.CreateEntity(new FComponent1 { Value = i }));
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
            _commandBuffer!.DeleteEntity(_entities![i].Id);

        _commandBuffer!.Playback();
    }
}

public class FrifloDestroyEntityDeferredT3 : DestroyEntityDeferredT3
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
            _commandBuffer!.DeleteEntity(_entities![i].Id);

        _commandBuffer!.Playback();
    }
}

public class FrifloDestroyEntityDeferredT5 : DestroyEntityDeferredT5
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
                    new FComponent3 { Value = i },
                    new FComponent4 { Value = i },
                    new FComponent5 { Value = i }
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
            _commandBuffer!.DeleteEntity(_entities![i].Id);

        _commandBuffer!.Playback();
    }
}
