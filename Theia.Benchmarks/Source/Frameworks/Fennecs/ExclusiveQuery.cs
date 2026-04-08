using BenchmarkDotNet.Attributes;
using fennecs;
using Theia.Benchmarks.Source.Categories;
using Theia.Benchmarks.Source.Resources;

namespace Theia.Benchmarks.Source.Frameworks.Fennecs;

public class FennecsExclusiveQueryOnT1 : ExclusiveQueryOnT1
{
    private World? _world;
    private Stream<Component1> _stream;

    public override void Setup()
    {
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _world.Spawn().Add(new Component1 { Value = i });

        _stream = _world.Query<Component1>().Stream();
    }

    public override void CleanUp()
    {
        _world = null;
        _stream = default;
    }

    [Benchmark]
    public override void Run()
    {
        _stream.For(
            (in Entity e, ref Component1 c1) =>
            {
                c1.Value += 1;
            }
        );
    }
}

public class FennecsExclusiveQueryOnT3 : ExclusiveQueryOnT3
{
    private World? _world;
    private Stream<Component1, Component2, Component3> _stream;

    public override void Setup()
    {
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _world
                .Spawn()
                .Add(new Component1 { Value = i })
                .Add(new Component2 { Value = i })
                .Add(new Component3 { Value = i });

        _stream = _world.Query<Component1, Component2, Component3>().Stream();
    }

    public override void CleanUp()
    {
        _world = null;
        _stream = default;
    }

    [Benchmark]
    public override void Run()
    {
        _stream.For(
            (in Entity e, ref Component1 c1, ref Component2 c2, ref Component3 c3) =>
            {
                c1.Value += 1;
                c2.Value += 1;
                c3.Value += 1;
            }
        );
    }
}

public class FennecsExclusiveQueryOnT5 : ExclusiveQueryOnT5
{
    private World? _world;
    private Stream<Component1, Component2, Component3, Component4, Component5> _stream;

    public override void Setup()
    {
        _world = new World();

        for (int i = 0; i < EntityCount; i++)
            _world
                .Spawn()
                .Add(new Component1 { Value = i })
                .Add(new Component2 { Value = i })
                .Add(new Component3 { Value = i })
                .Add(new Component4 { Value = i })
                .Add(new Component5 { Value = i });

        _stream = _world
            .Query<Component1, Component2, Component3, Component4, Component5>()
            .Stream();
    }

    public override void CleanUp()
    {
        _world = null;
        _stream = default;
    }

    [Benchmark]
    public override void Run()
    {
        _stream.For(
            (
                in Entity e,
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
