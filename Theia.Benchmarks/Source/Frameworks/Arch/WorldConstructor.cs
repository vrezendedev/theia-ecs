using Arch.Core;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchWorldConstructor : WorldConstructor
{
    private World? _world;

    public override void CleanUp()
    {
        _world = null;
    }

    [Benchmark]
    public override void Run()
    {
        _world = World.Create(InitialSize);
    }
}
