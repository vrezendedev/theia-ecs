using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloCrossArchetypeQueryOnT1 : CrossArchetypeQueryOnT1
{
    private EntityStore? _entityStore;
    private ArchetypeQuery<FComponent1>? _archetypeQuery;

    public override void Setup()
    {
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _entityStore.CreateEntity(new FComponent1 { Value = i });
            _entityStore.CreateEntity(new FComponent1 { Value = i }, new FComponent2 { Value = i });
        }

        _archetypeQuery = _entityStore.Query<FComponent1>();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _archetypeQuery = null;
    }

    [Benchmark]
    public override void Run()
    {
        _archetypeQuery!.ForEachEntity(
            (ref FComponent1 c1, Entity e) =>
            {
                c1.Value += 1;
            }
        );
    }
}

public class FrifloCrossArchetypeQueryOnT3 : CrossArchetypeQueryOnT3
{
    private EntityStore? _entityStore;
    private ArchetypeQuery<FComponent1, FComponent2, FComponent3>? _archetypeQuery;

    public override void Setup()
    {
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _entityStore.CreateEntity(
                new FComponent1 { Value = i },
                new FComponent2 { Value = i },
                new FComponent3 { Value = i }
            );

            _entityStore.CreateEntity(
                new FComponent1 { Value = i },
                new FComponent2 { Value = i },
                new FComponent3 { Value = i },
                new FComponent4 { Value = i }
            );
        }

        _archetypeQuery = _entityStore.Query<FComponent1, FComponent2, FComponent3>();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _archetypeQuery = null;
    }

    [Benchmark]
    public override void Run()
    {
        _archetypeQuery!.ForEachEntity(
            (ref FComponent1 c1, ref FComponent2 c2, ref FComponent3 c3, Entity e) =>
            {
                c1.Value += 1;
                c2.Value += 1;
                c3.Value += 1;
            }
        );
    }
}

public class FrifloCrossArchetypeQueryOnT5 : CrossArchetypeQueryOnT5
{
    private EntityStore? _entityStore;
    private ArchetypeQuery<
        FComponent1,
        FComponent2,
        FComponent3,
        FComponent4,
        FComponent5
    >? _archetypeQuery;

    public override void Setup()
    {
        _entityStore = new EntityStore();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _entityStore.CreateEntity(
                new FComponent1 { Value = i },
                new FComponent2 { Value = i },
                new FComponent3 { Value = i },
                new FComponent4 { Value = i },
                new FComponent5 { Value = i }
            );

            _entityStore.CreateEntity(
                new FComponent1 { Value = i },
                new FComponent2 { Value = i },
                new FComponent3 { Value = i },
                new FComponent4 { Value = i },
                new FComponent5 { Value = i },
                new FComponent6 { Value = i }
            );
        }

        _archetypeQuery = _entityStore.Query<
            FComponent1,
            FComponent2,
            FComponent3,
            FComponent4,
            FComponent5
        >();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _archetypeQuery = null;
    }

    [Benchmark]
    public override void Run()
    {
        _archetypeQuery!.ForEachEntity(
            (
                ref FComponent1 c1,
                ref FComponent2 c2,
                ref FComponent3 c3,
                ref FComponent4 c4,
                ref FComponent5 c5,
                Entity e
            ) =>
            {
                c1.Value += 1;
                c2.Value += 1;
                c3.Value += 1;
                c4.Value += 1;
                c5.Value += 1;
            }
        );
    }
}
