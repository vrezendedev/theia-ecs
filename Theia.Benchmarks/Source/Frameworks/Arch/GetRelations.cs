using System.Collections.Generic;
using Arch.Core;
using Arch.Relationships;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchGetTagRelationOnT1 : GetTagRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();

        Owner = _world.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Create(new Component1 { Value = i }));
            _world.AddRelationship<TagRelation1>(Owner, _entities[i]);
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
            _ = _world!.HasRelationship<TagRelation1>(owner, _entities![i]);
    }
}

public class ArchGetAllTagRelationsOnT1 : GetAllTagRelationsOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();

        Owner = _world.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Create(new Component1 { Value = i }));
            _world.AddRelationship<TagRelation1>(Owner, _entities[i]);
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

        _ = _world!.GetRelationships<TagRelation1>(owner);
    }
}

public class ArchGetEvaluatedRelationOnT1 : GetEvaluatedRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();

        Owner = _world.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Create(new Component1 { Value = i }));
            _world.AddRelationship(Owner, _entities[i], new EvaluatedRelation1() { Value = i });
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
            _ = _world!.GetRelationship<EvaluatedRelation1>(owner, _entities![i]);
    }
}

public class ArchGetAllEvaluatedRelationsOnT1 : GetAllEvaluatedRelationsOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();

        Owner = _world.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Create(new Component1 { Value = i }));
            _world.AddRelationship(Owner, _entities[i], new EvaluatedRelation1() { Value = i });
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

        _ = _world!.GetRelationships<EvaluatedRelation1>(owner);
    }
}
