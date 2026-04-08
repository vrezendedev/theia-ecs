using BenchmarkDotNet.Attributes;
using EntitiesDb;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.EntitiesDb;

public class EntityDbCrossArchetypeQueryOnT1 : CrossArchetypeQueryOnT1
{
    private EntityDatabase? _entityStore;
    private Query? _query;

    public override void Setup()
    {
        _entityStore = new EntityDatabase(new(16384, int.MaxValue));

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _entityStore.Create(new Component1 { Value = i });
            _entityStore.Create(new Component1 { Value = i }, new Component2 { Value = i });
        }

        _query = _entityStore.QueryBuilder.WithAll<Component1>().Build();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        _query.ForEach(
            (Entity e, ref Component1 c1) =>
            {
                c1.Value += 1;
            }
        );
    }
}

public class EntityDbCrossArchetypeQueryOnT3 : CrossArchetypeQueryOnT3
{
    private EntityDatabase? _entityStore;
    private Query? _query;

    public override void Setup()
    {
        _entityStore = new EntityDatabase(new(16384, int.MaxValue));

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _entityStore.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );
            _entityStore.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i }
            );
        }

        _query = _entityStore.QueryBuilder.WithAll<Component1, Component2, Component3>().Build();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        _query.ForEach(
            (Entity e, ref Component1 c1, ref Component2 c2, ref Component3 c3) =>
            {
                c1.Value += 1;
                c2.Value += 1;
                c3.Value += 1;
            }
        );
    }
}

public class EntityDbCrossArchetypeQueryOnT5 : CrossArchetypeQueryOnT5
{
    private EntityDatabase? _entityStore;
    private Query? _query;

    public override void Setup()
    {
        _entityStore = new EntityDatabase(new(16384, int.MaxValue));

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _entityStore.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );

            _entityStore.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i },
                new Component6 { Value = i }
            );
        }

        _query = _entityStore
            .QueryBuilder.WithAll<Component1, Component2, Component3, Component4, Component5>()
            .Build();
    }

    public override void CleanUp()
    {
        _entityStore = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        _query.ForEach(
            (
                Entity e,
                ref Component1 c1,
                ref Component2 c2,
                ref Component3 c3,
                ref Component4 c4,
                ref Component5 c5
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
