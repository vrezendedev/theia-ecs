using BenchmarkDotNet.Attributes;
using Friflo.Engine.ECS;
using Theia.Benchmarks.Source.Categories;

namespace Theia.Benchmarks.Source.Frameworks.Friflo;

public class FrifloWorldConstructor : WorldConstructor
{
    private EntityStore? _entityStore;

    public override void CleanUp()
    {
        _entityStore = null;
    }

    [Benchmark]
    public override void Run()
    {
        EntityStore entityStore = new EntityStore();
        entityStore.EnsureCapacity(InitialSize);
    }
}
