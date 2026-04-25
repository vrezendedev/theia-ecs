using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using fennecs;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Fennecs;

public class FennecsGetTagRelationOnT1 : GetTagRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Owner = _world.Spawn().Add(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Spawn().Add(new Component1 { Value = i }));
            Owner.Add<TagRelation1>(_entities[i]);
        }
    }

    public override void CleanUp()
    {
        _entities = null;
        _world = null;
    }

    [Benchmark]
    public override void Run()
    {
        Entity owner = Owner;

        for (int i = 0; i < Relations; i++)
            _ = owner.Has<TagRelation1>(_entities![i]);
    }
}

public class FennecsGetEvaluatedRelationOnT1 : GetEvaluatedRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Owner = _world.Spawn().Add(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Spawn().Add(new Component1 { Value = i }));
            Owner.Add(new EvaluatedRelation1() { Value = i }, _entities[i]);
        }
    }

    public override void CleanUp()
    {
        _entities = null;
        _world = null;
    }

    [Benchmark]
    public override void Run()
    {
        Entity owner = Owner;

        for (int i = 0; i < Relations; i++)
            _ = owner.Get<EvaluatedRelation1>(_entities![i]);
    }
}
