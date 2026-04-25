using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloAddTagRelationOnT1 : AddTagRelationOnT1
{
    private record struct LinkRelation(Entity Target) : ILinkRelation
    {
        public Entity GetRelationKey() => Target;
    }

    public Entity Owner { get; set; }
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        Owner = _entityStore.CreateEntity(new FComponent1());

        for (int i = 0; i < Relations; i++)
            _entities.Add(_entityStore.CreateEntity(new FComponent1()));
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        Entity owner = Owner;

        for (int i = 0; i < Relations; i++)
            owner.AddRelation(new LinkRelation(_entities![i]));
    }
}

public class FrifloAddEvaluatedRelationOnT1 : AddEvaluatedRelationOnT1
{
    private record struct EvaluatedLinkRelation(int Value, Entity Target) : ILinkRelation
    {
        public Entity GetRelationKey() => Target;
    }

    public Entity Owner { get; set; }
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        Owner = _entityStore.CreateEntity(new FComponent1());

        for (int i = 0; i < Relations; i++)
            _entities.Add(_entityStore.CreateEntity(new FComponent1()));
    }

    public override void CleanUp()
    {
        _entities = null;
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        Entity owner = Owner;

        for (int i = 0; i < Relations; i++)
            owner.AddRelation(new EvaluatedLinkRelation(i, _entities![i]));
    }
}
