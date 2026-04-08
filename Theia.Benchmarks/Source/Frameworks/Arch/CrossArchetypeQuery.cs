using Arch.Core;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchCrossArchetypeQueryOnT1 : CrossArchetypeQueryOnT1
{
    private World? _world;
    private QueryDescription _queryDescription;

    public override void Setup()
    {
        _world = World.Create();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _world.Create(new Component1 { Value = i });
            _world.Create(new Component1 { Value = i }, new Component2 { Value = i });
        }

        _queryDescription = new QueryDescription().WithAll<Component1>();
    }

    public override void CleanUp()
    {
        _world = null;
        _queryDescription = default;
    }

    [Benchmark]
    public override void Run()
    {
        _world!.Query(
            in _queryDescription,
            (Entity entity, ref Component1 c1) =>
            {
                c1.Value += 1;
            }
        );
    }
}

public class ArchCrossArchetypeQueryOnT3 : CrossArchetypeQueryOnT3
{
    private World? _world;
    private QueryDescription _queryDescription;

    public override void Setup()
    {
        _world = World.Create();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _world.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i }
            );

            _world.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i }
            );
        }

        _queryDescription = new QueryDescription().WithAll<Component1, Component2, Component3>();
    }

    public override void CleanUp()
    {
        _world = null;
        _queryDescription = default;
    }

    [Benchmark]
    public override void Run()
    {
        _world!.Query(
            in _queryDescription,
            (Entity entity, ref Component1 c1, ref Component2 c2, ref Component3 c3) =>
            {
                c1.Value += 1;
                c2.Value += 1;
                c3.Value += 1;
            }
        );
    }
}

public class ArchCrossArchetypeQueryOnT5 : CrossArchetypeQueryOnT5
{
    private World? _world;
    private QueryDescription _queryDescription;

    public override void Setup()
    {
        _world = World.Create();

        for (int i = 0; i < EntityCount / 2; i++)
        {
            _world.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i }
            );

            _world.Create(
                new Component1 { Value = i },
                new Component2 { Value = i },
                new Component3 { Value = i },
                new Component4 { Value = i },
                new Component5 { Value = i },
                new Component6 { Value = i }
            );
        }

        _queryDescription = new QueryDescription().WithAll<
            Component1,
            Component2,
            Component3,
            Component4,
            Component5
        >();
    }

    public override void CleanUp()
    {
        _world = null;
        _queryDescription = default;
    }

    [Benchmark]
    public override void Run()
    {
        _world!.Query(
            in _queryDescription,
            (
                Entity entity,
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
