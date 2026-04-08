using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntityDbSetComponentOnT1 : SetComponentOnT1
{
    private EntityDatabase? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityDatabase(new(16384, int.MaxValue));

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(_entityStore.Create(new Component1 { Value = i }));
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        for (int i = 0; i < EntityCount; i++)
            _entityStore!.Write<Component1>(_entities![i]) = new Component1() { Value = i };
    }
}
