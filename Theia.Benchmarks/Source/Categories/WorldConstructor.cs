using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

[BenchmarkCategory(nameof(WorldConstructor))]
public abstract class WorldConstructor
{
    public const int InitialSize = 16_384;

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}
