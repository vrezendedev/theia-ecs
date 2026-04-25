using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaGetTagRelationOnT1 : GetTagRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Assemblage<Component1> _assemblage = _world.CreateAssemblage<Component1>();

        Owner = _assemblage.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_assemblage.Create(new Component1 { Value = i }));
            _world.TryAddTagRelation<TagRelation1>(Owner, _entities[i]);
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
            _ = _world!.IsRelatedTo<TagRelation1>(owner, _entities![i]);
    }
}

public class TheiaGetAllTagRelationsOnT1 : GetAllTagRelationsOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Assemblage<Component1> _assemblage = _world.CreateAssemblage<Component1>();

        Owner = _assemblage.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_assemblage.Create(new Component1 { Value = i }));
            _world.TryAddTagRelation<TagRelation1>(Owner, _entities[i]);
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

        _ = _world!.GetRelations<TagRelation1>(owner);
    }
}

public class TheiaGetEvaluatedRelationOnT1 : GetEvaluatedRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Assemblage<Component1> _assemblage = _world.CreateAssemblage<Component1>();

        Owner = _assemblage.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_assemblage.Create(new Component1 { Value = i }));

            _world.TryAddEvaluatedRelation(
                Owner,
                _entities[i],
                new EvaluatedRelation1() { Value = i }
            );
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
            _ = _world!.GetEvaluatedRelation<EvaluatedRelation1>(owner, _entities![i]);
    }
}

public class TheiaGetAllEvaluatedRelationsOnT1 : GetAllEvaluatedRelationsOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Assemblage<Component1> _assemblage = _world.CreateAssemblage<Component1>();

        Owner = _assemblage.Create(new Component1() { Value = 0 });

        for (int i = 0; i < Relations; i++)
        {
            _entities.Add(_assemblage.Create(new Component1 { Value = i }));

            _world.TryAddEvaluatedRelation(
                Owner,
                _entities[i],
                new EvaluatedRelation1() { Value = i }
            );
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

        _ = _world!.GetEvaluatedRelations<EvaluatedRelation1>(owner);
    }
}
