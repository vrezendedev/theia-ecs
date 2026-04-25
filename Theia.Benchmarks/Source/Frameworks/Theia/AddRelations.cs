using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaAddTagRelationOnT1 : AddTagRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Assemblage<Component1> _assemblage = _world.CreateAssemblage<Component1>();

        Owner = _assemblage.Create(new Component1());

        for (int i = 0; i < Relations; i++)
            _entities.Add(_assemblage.Create(new Component1 { Value = i }));
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
            _world!.TryAddTagRelation<TagRelation1>(owner, _entities![i]);
    }
}

public class TheiaAddEvaluatedRelationOnT1 : AddEvaluatedRelationOnT1
{
    public Entity Owner { get; set; }
    private World? _world;
    private List<Entity>? _entities;

    public override void Setup()
    {
        _entities = new();
        _world = new World();

        Assemblage<Component1> _assemblage = _world.CreateAssemblage<Component1>();

        Owner = _assemblage.Create(new Component1());

        for (int i = 0; i < Relations; i++)
            _entities.Add(_assemblage.Create(new Component1 { Value = i }));
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
            _world!.TryAddEvaluatedRelation(
                owner,
                _entities![i],
                new EvaluatedRelation1() { Value = i }
            );
    }
}
