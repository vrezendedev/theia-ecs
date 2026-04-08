using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntitiesDbAddComponentDeferredOnT3 : AddComponentDeferredOnT3
{
    private EntityDatabase? _entityDatabase;
    private List<Entity>? _entities;
    private CommandBuffer? _commandBuffer;

    public override void Setup()
    {
        _entities = new();
        _entityDatabase = new EntityDatabase(new(16384, int.MaxValue));
        _commandBuffer = _entityDatabase.CreateCommandBuffer(EntityCount);

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _entityDatabase.Create(
                    new Component1 { Value = i },
                    new Component2 { Value = i },
                    new Component3 { Value = i }
                )
            );
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityDatabase = null;
        _commandBuffer = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _commandBuffer!.Add(_entities![i], new Component4() { Value = i });

        _commandBuffer!.Commit();
    }
}
