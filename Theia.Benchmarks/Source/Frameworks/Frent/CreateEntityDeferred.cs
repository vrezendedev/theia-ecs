using BenchmarkDotNet.Attributes;
using Frent;
using Frent.Core;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Frent;

public class FrentCreateEntityDeferredT1 : CreateEntityDeferredT1
{
    private World? _world;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _world = new World();
        _commandBuffer = new CommandBuffer(_world);
    }

    public override void CleanUp()
    {
        _world = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            Entity entity = _commandBuffer!.Entity().End();
            _commandBuffer.AddComponent(entity, new Component1() { Value = i });
        }

        _commandBuffer!.Playback();
    }
}

public class FrentCreateEntityDeferredT3 : CreateEntityDeferredT3
{
    private World? _world;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _world = new World();
        _commandBuffer = new CommandBuffer(_world);
    }

    public override void CleanUp()
    {
        _world = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            Entity entity = _commandBuffer!.Entity().End();
            _commandBuffer.AddComponent(entity, new Component1() { Value = i });
            _commandBuffer.AddComponent(entity, new Component2() { Value = i });
            _commandBuffer.AddComponent(entity, new Component3() { Value = i });
        }

        _commandBuffer!.Playback();
    }
}

public class FrentCreateEntityDeferredT5 : CreateEntityDeferredT5
{
    private World? _world;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _world = new World();
        _commandBuffer = new CommandBuffer(_world);
    }

    public override void CleanUp()
    {
        _world = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            Entity entity = _commandBuffer!.Entity().End();
            _commandBuffer.AddComponent(entity, new Component1() { Value = i });
            _commandBuffer.AddComponent(entity, new Component2() { Value = i });
            _commandBuffer.AddComponent(entity, new Component3() { Value = i });
            _commandBuffer.AddComponent(entity, new Component4() { Value = i });
            _commandBuffer.AddComponent(entity, new Component5() { Value = i });
        }

        _commandBuffer!.Playback();
    }
}
