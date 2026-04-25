using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using fennecs;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Fennecs;

public class FennecsAddTagRelationOnT1 : AddTagRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Owner = _world.Spawn().Add<Component1>();

        for (int i = 0; i < Relations; i++)
            _entities.Add(_world.Spawn().Add<Component1>());
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
            owner.Add<TagRelation1>(default, _entities![i]);
    }
}

public class FennecsAddEvaluatedRelationOnT1 : AddEvaluatedRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Owner = _world.Spawn().Add<Component1>();

        for (int i = 0; i < Relations; i++)
            _entities.Add(_world.Spawn().Add<Component1>());
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
            owner.Add(new EvaluatedRelation1() { Value = i }, _entities![i]);
    }
}
