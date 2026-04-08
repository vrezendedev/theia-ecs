using Arch.Buffer;
using Arch.Core;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchCreateEntityDeferredT1 : CreateEntityDeferredT1
{
    private World? _world;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _world = World.Create();
        _commandBuffer = new CommandBuffer();
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
            Entity entity = _commandBuffer!.Create(new Signature(typeof(Component1)));
            _commandBuffer.Set(entity, new Component1() { Value = i });
        }

        _commandBuffer!.Playback(_world!);
    }
}

public class ArchCreateEntityDeferredT3 : CreateEntityDeferredT3
{
    private World? _world;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _world = World.Create();
        _commandBuffer = new CommandBuffer();
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
            Entity entity = _commandBuffer!.Create(
                new Signature(typeof(Component1), typeof(Component2), typeof(Component3))
            );
            _commandBuffer.Set(entity, new Component1() { Value = i });
            _commandBuffer.Set(entity, new Component2() { Value = i });
            _commandBuffer.Set(entity, new Component3() { Value = i });
        }

        _commandBuffer!.Playback(_world!);
    }
}

public class ArchCreateEntityDeferredT5 : CreateEntityDeferredT5
{
    private World? _world;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _world = World.Create();
        _commandBuffer = new CommandBuffer();
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
            Entity entity = _commandBuffer!.Create(
                new Signature(
                    typeof(Component1),
                    typeof(Component2),
                    typeof(Component3),
                    typeof(Component4),
                    typeof(Component5)
                )
            );
            _commandBuffer.Set(entity, new Component1() { Value = i });
            _commandBuffer.Set(entity, new Component2() { Value = i });
            _commandBuffer.Set(entity, new Component3() { Value = i });
            _commandBuffer.Set(entity, new Component4() { Value = i });
            _commandBuffer.Set(entity, new Component5() { Value = i });
        }

        _commandBuffer!.Playback(_world!);
    }
}
