using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Frameworks.Theia;

public class TheiaExclusiveQueryOnT1 : ExclusiveQueryOnT1
{
    private ref struct ForEachEntity : IForEachEntity<Component1>
    {
        public void Execute(Entity entity, ref Component1 c1)
        {
            c1.Value += 1;
        }
    }

    private World? _world;
    private Assemblage<Component1>? _assemblage;
    private SettlerQuery<Component1>? _query;

    public override void Setup()
    {
        _world = new World();
        _assemblage = _world.CreateAssemblage<Component1>();
        _query = _world.CreateSettlerQuery(_assemblage);

        for (int i = 0; i < EntityCount; i++)
            _assemblage.Create(new Component1 { Value = i });
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblage = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        ForEachEntity forEach = new ForEachEntity();
        _query!.ForEachEntity(ref forEach);
    }
}

public class TheiaExclusiveQueryOnT3 : ExclusiveQueryOnT3
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3>? _assemblage;
    private SettlerQuery<Component1, Component2, Component3>? _query;

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
        _assemblage = _world.CreateAssemblage<Component1, Component2, Component3>();
        _query = _world.CreateSettlerQuery(_assemblage);

        for (int i = 0; i < EntityCount; i++)
            _assemblage.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblage = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        ForEachEntity forEachEntity = new();
        _query!.ForEachEntity(ref forEachEntity);
    }
}

public class TheiaExclusiveQueryOnT5 : ExclusiveQueryOnT5
{
    private World? _world;
    private Assemblage<Component1, Component2, Component3, Component4, Component5>? _assemblage;
    private SettlerQuery<Component1, Component2, Component3, Component4, Component5>? _query;

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
        _assemblage = _world.CreateAssemblage<
            Component1,
            Component2,
            Component3,
            Component4,
            Component5
        >();
        _query = _world.CreateSettlerQuery(_assemblage);

        for (int i = 0; i < EntityCount; i++)
            _assemblage.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );
    }

    public override void CleanUp()
    {
        _world = null;
        _assemblage = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        ForEachEntity forEachEntity = new();
        _query!.ForEachEntity(ref forEachEntity);
    }
}
