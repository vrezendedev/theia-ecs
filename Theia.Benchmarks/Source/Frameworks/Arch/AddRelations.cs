using System.Collections.Generic;
using Arch.Core;
using Arch.Relationships;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchAddTagRelationOnT1 : AddTagRelationOnT1
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
            _entities.Add(_world.Create<Component1>());
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
            _world!.AddRelationship<TagRelation1>(owner, _entities![i]);
    }
}

public class ArchAddEvaluatedRelationOnT1 : AddEvaluatedRelationOnT1
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
            _entities.Add(_world.Create<Component1>());
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
            _world!.AddRelationship(owner, _entities![i], new EvaluatedRelation1() { Value = i });
    }
}
