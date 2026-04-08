using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloCreateEntityDeferredT1 : CreateEntityDeferredT1
{
    private EntityStore? _entityStore;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entityStore = new EntityStore();
        _commandBuffer = _entityStore.GetCommandBuffer();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            int id = _commandBuffer!.CreateEntity();
            _commandBuffer.AddComponent(id, new FComponent1() { Value = i });
        }

        _commandBuffer!.Playback();
    }
}

public class FrifloCreateEntityDeferredT3 : CreateEntityDeferredT3
{
    private EntityStore? _entityStore;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entityStore = new EntityStore();
        _commandBuffer = _entityStore.GetCommandBuffer();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            int id = _commandBuffer!.CreateEntity();
            _commandBuffer.AddComponent(id, new FComponent1() { Value = i });
            _commandBuffer.AddComponent(id, new FComponent2() { Value = i });
            _commandBuffer.AddComponent(id, new FComponent3() { Value = i });
        }

        _commandBuffer!.Playback();
    }
}

public class FrifloCreateEntityDeferredT5 : CreateEntityDeferredT5
{
    private EntityStore? _entityStore;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entityStore = new EntityStore();
        _commandBuffer = _entityStore.GetCommandBuffer();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            int id = _commandBuffer!.CreateEntity();
            _commandBuffer.AddComponent(id, new FComponent1() { Value = i });
            _commandBuffer.AddComponent(id, new FComponent2() { Value = i });
            _commandBuffer.AddComponent(id, new FComponent3() { Value = i });
            _commandBuffer.AddComponent(id, new FComponent4() { Value = i });
            _commandBuffer.AddComponent(id, new FComponent5() { Value = i });
        }

        _commandBuffer!.Playback();
    }
}
