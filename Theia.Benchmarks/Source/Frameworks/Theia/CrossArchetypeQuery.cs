using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaCrossArchetypeQueryOnT1 : CrossArchetypeQueryOnT1
{
    private World? _world;
    private Assemblage<Component1>? _assemblageA;
    private Assemblage<Component1, Component2>? _assemblageB;
    private NomadQuery<Component1>? _query;

    private ref struct ForEachEntity : IForEachEntity<Component1>
    {
        public void Execute(Entity entity, ref Component1 c1)
        {
            c1.Value += 1;
        }
    }

    public override void Setup()
    {
        _world = new World();
        _assemblageA = _world.CreateAssemblage<Component1>();
        _assemblageB = _world.CreateAssemblage<Component1, Component2>();
        _query = _world.CreateNomadQuery<Component1>();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _assemblageA.Create(new Component1 { Value = i });
            _assemblageB.Create(new Component1 { Value = i }, new Component2() { Value = i });
        }
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblageA = null;
        _assemblageB = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        ForEachEntity forEach = new ForEachEntity();
        _query!.ForEachEntity(ref forEach);
    }
}

public class TheiaCrossArchetypeQueryOnT3 : CrossArchetypeQueryOnT3
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3>? _assemblageA;
    private Assemblage<Component1, Component2, Component3, Component4>? _assemblageB;
    private NomadQuery<Component1, Component2, Component3>? _query;

    private ref struct ForEachEntity : IForEachEntity<Component1, Component2, Component3>
    {
        public void Execute(Entity entity, ref Component1 c1, ref Component2 c2, ref Component3 c3)
        {
            c1.Value += 1;
            c2.Value += 1;
            c3.Value += 1;
        }
    }

    public override void Setup()
    {
        _world = new World();
        _assemblageA = _world.CreateAssemblage<Component1, Component2, Component3>();
        _assemblageB = _world.CreateAssemblage<Component1, Component2, Component3, Component4>();
        _query = _world.CreateNomadQuery<Component1, Component2, Component3>();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _assemblageA.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );
            _assemblageB.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i }
            );
        }
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblageA = null;
        _assemblageB = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        ForEachEntity forEachEntity = new();
        _query!.ForEachEntity(ref forEachEntity);
    }
}

public class TheiaCrossArchetypeQueryOnT5 : CrossArchetypeQueryOnT5
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3, Component4, Component5>? _assemblageA;
    private Assemblage<
        Component1,
        Component2,
        Component3,
        Component4,
        Component5,
        Component6
    >? _assemblageB;
    private NomadQuery<Component1, Component2, Component3, Component4, Component5>? _query;

    private ref struct ForEachEntity
        : IForEachEntity<Component1, Component2, Component3, Component4, Component5>
    {
        public void Execute(
            Entity entity,
            ref Component1 c1,
            ref Component2 c2,
            ref Component3 c3,
            ref Component4 c4,
            ref Component5 c5
        )
        {
            c1.Value += 1;
            c2.Value += 1;
            c3.Value += 1;
            c4.Value += 1;
            c5.Value += 1;
        }
    }

    public override void Setup()
    {
        _world = new World();
        _assemblageA = _world.CreateAssemblage<
            Component1,
            Component2,
            Component3,
            Component4,
            Component5
        >();
        _assemblageB = _world.CreateAssemblage<
            Component1,
            Component2,
            Component3,
            Component4,
            Component5,
            Component6
        >();
        _query = _world.CreateNomadQuery<
            Component1,
            Component2,
            Component3,
            Component4,
            Component5
        >();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _assemblageA.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );
            _assemblageB.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i },
                new Component6 { Value = i }
            );
        }
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblageA = null;
        _assemblageB = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        ForEachEntity forEachEntity = new();
        _query!.ForEachEntity(ref forEachEntity);
    }
}
