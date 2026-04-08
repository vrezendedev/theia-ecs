using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

[BenchmarkCategory(nameof(AddComponentDeferredOnT3))]
public abstract class AddComponentDeferredOnT3
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}
