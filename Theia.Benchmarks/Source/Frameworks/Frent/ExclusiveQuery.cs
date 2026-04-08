using BenchmarkDotNet.Attributes;
using Frent;
using Frent.Systems;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Frent;

public class FrentExclusiveQueryOnT1 : ExclusiveQueryOnT1
{
    private World? _world;
    private Query? _query;

    public override void Setup()
    {
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _world.Create(new Component1 { Value = i });

        _query = _world.Query<Component1>();
    }

    public override void CleanUp()
    {
        _world = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        foreach (EntityRefTuple<Component1> e in _query!.EnumerateWithEntities<Component1>())
            e.Item1.Value.Value += 1;
    }
}

public class FrentExclusiveQueryOnT3 : ExclusiveQueryOnT3
{
    private World? _world;
    private Query? _query;

    public override void Setup()
    {
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _world.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );

        _query = _world.Query<Component1, Component2, Component3>();
    }

    public override void CleanUp()
    {
        _world = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        foreach (
            EntityRefTuple<Component1, Component2, Component3> e in _query!.EnumerateWithEntities<
                Component1,
                Component2,
                Component3
            >()
        )
        {
            e.Item1.Value.Value += 1;
            e.Item2.Value.Value += 1;
            e.Item3.Value.Value += 1;
        }
    }
}

public class FrentExclusiveQueryOnT5 : ExclusiveQueryOnT5
{
    private World? _world;
    private Query? _query;

    public override void Setup()
    {
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _world.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );

        _query = _world.Query<Component1, Component2, Component3, Component4, Component5>();
    }

    public override void CleanUp()
    {
        _world = null;
        _query = null;
    }

    [Benchmark]
    public override void Run()
    {
        foreach (
            EntityRefTuple<
                Component1,
                Component2,
                Component3,
                Component4,
                Component5
            > e in _query!.EnumerateWithEntities<
                Component1,
                Component2,
                Component3,
                Component4,
                Component5
            >()
        )
        {
            e.Item1.Value.Value += 1;
            e.Item2.Value.Value += 1;
            e.Item3.Value.Value += 1;
            e.Item4.Value.Value += 1;
            e.Item5.Value.Value += 1;
        }
    }
}
