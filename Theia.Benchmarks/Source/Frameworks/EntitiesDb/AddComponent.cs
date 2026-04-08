using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntityDbAddComponentOnT1 : AddComponentOnT1
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
            _entityStore!.Add(_entities![i], new Component2());
    }
}

public class EntityDbAddComponentOnT3 : AddComponentOnT3
{
    private EntityDatabase? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityDatabase(new(16384, int.MaxValue));

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _entityStore.Create(
                    new Component1 { Value = i },
                    new Component2 { Value = i },
                    new Component3 { Value = i }
                )
            );
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
            _entityStore!.Add(_entities![i], new Component4());
    }
}

public class EntityDbAddComponentOnT5 : AddComponentOnT5
{
    private EntityDatabase? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityDatabase(new(16384, int.MaxValue));

        for (int i = 0; i < EntityCount; i++)
            _entities.Add(
                _entityStore.Create(
                    new Component1 { Value = i },
                    new Component2 { Value = i },
                    new Component3 { Value = i },
                    new Component4 { Value = i },
                    new Component5 { Value = i }
                )
            );
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
            _entityStore!.Add(_entities![i], new Component6());
    }
}
