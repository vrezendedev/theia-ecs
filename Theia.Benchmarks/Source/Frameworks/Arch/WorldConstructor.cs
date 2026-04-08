using Arch.Core;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Categories;

namespace Theia.Benchmarks.Source.Frameworks.Arch;

public class ArchWorldConstructor : WorldConstructor
{
    [Benchmark]
    public override void Run()
    {
        World world = World.Create(InitialSize);
    }
}
