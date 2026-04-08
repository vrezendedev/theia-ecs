using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntitiesDbCreateEntityDeferredT1 : CreateEntityDeferredT1
{
    private EntityDatabase? _entityDatabase;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entityDatabase = new EntityDatabase(new(16384, int.MaxValue));
        _commandBuffer = _entityDatabase.CreateCommandBuffer(EntityCount);
    }

    public override void CleanUp()
    {
        _entityDatabase = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _commandBuffer!.Create(new Component1 { Value = i });

        _commandBuffer!.Commit();
    }
}

public class EntitiesDbCreateEntityDeferredT3 : CreateEntityDeferredT3
{
    private EntityDatabase? _entityDatabase;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entityDatabase = new EntityDatabase(new(16384, int.MaxValue));
        _commandBuffer = _entityDatabase.CreateCommandBuffer(EntityCount);
    }

    public override void CleanUp()
    {
        _entityDatabase = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _commandBuffer!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );

        _commandBuffer!.Commit();
    }
}

public class EntitiesDbCreateEntityDeferredT5 : CreateEntityDeferredT5
{
    private EntityDatabase? _entityDatabase;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entityDatabase = new EntityDatabase(new(16384, int.MaxValue));
        _commandBuffer = _entityDatabase.CreateCommandBuffer(EntityCount);
    }

    public override void CleanUp()
    {
        _entityDatabase = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _commandBuffer!.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );

        _commandBuffer!.Commit();
    }
}
