using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

file record struct LinkTagRelation(Entity Target) : ILinkRelation
{
    public Entity GetRelationKey() => Target;
}

file record struct LinkEvaluatedRelation(int Value, Entity Target) : ILinkRelation
{
    public Entity GetRelationKey() => Target;
}

public class FrifloGetTagRelationOnT1 : GetTagRelationOnT1
{
    public Entity Owner { get; set; }
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        Owner = _entityStore.CreateEntity(new FComponent1());

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_entityStore.CreateEntity(new FComponent1()));
            Owner.AddRelation(new LinkTagRelation(_entities[i]));
        }
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
            _ = owner.GetRelation<LinkTagRelation, Entity>(_entities![i]);
    }
}

public class FrifloGetTagRelationsOnT1 : GetTagRelationsOnT1
{
    public Entity Owner { get; set; }
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        Owner = _entityStore.CreateEntity(new FComponent1());

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_entityStore.CreateEntity(new FComponent1()));
            Owner.AddRelation(new LinkTagRelation(_entities[i]));
        }
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

        _ = owner.GetRelations<LinkTagRelation>();
    }
}

public class FrifloGetEvaluatedRelationOnT1 : GetEvaluatedRelationOnT1
{
    public Entity Owner { get; set; }
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        Owner = _entityStore.CreateEntity(new FComponent1());

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_entityStore.CreateEntity(new FComponent1()));
            Owner.AddRelation(new LinkEvaluatedRelation(i, _entities[i]));
        }
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
            _ = owner.GetRelation<LinkEvaluatedRelation, Entity>(_entities![i]);
    }
}

public class FrifloGetEvaluatedRelationsOnT1 : GetEvaluatedRelationsOnT1
{
    public Entity Owner { get; set; }
    private EntityStore? _entityStore;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _entityStore = new EntityStore();

        Owner = _entityStore.CreateEntity(new FComponent1());

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_entityStore.CreateEntity(new FComponent1()));
            Owner.AddRelation(new LinkEvaluatedRelation(i, _entities[i]));
        }
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

        _ = owner.GetRelations<LinkEvaluatedRelation>();
    }
}
