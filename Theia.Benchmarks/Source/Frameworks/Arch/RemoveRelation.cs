using System.Collections.Generic;
using Arch.Core;
using Arch.Relationships;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchRemoveTagRelationOnT1 : RemoveTagRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();

        Owner = _world.Create<Component1>();

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Create<Component1>());
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
            _world!.RemoveRelationship<TagRelation1>(owner, _entities![i]);
    }
}

public class ArchRemoveEvaluatedRelationOnT1 : RemoveEvaluatedRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = World.Create();

        Owner = _world.Create<Component1>();

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_world.Create<Component1>());
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
            _world!.RemoveRelationship<EvaluatedRelation1>(owner, _entities![i]);
    }
}
