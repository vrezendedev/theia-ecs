using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloWorldConstructor : WorldConstructor
{
    [Benchmark]
    public override void Run()
    {
        EntityStore entityStore = new EntityStore();
        entityStore.EnsureCapacity(InitialSize);
    }
}
