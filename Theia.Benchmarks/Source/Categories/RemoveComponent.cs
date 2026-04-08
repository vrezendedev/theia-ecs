using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class RemoveComponent
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(RemoveComponentOnT2))]
public abstract class RemoveComponentOnT2 : RemoveComponent;

[BenchmarkCategory(nameof(RemoveComponentOnT3))]
public abstract class RemoveComponentOnT3 : RemoveComponent;

[BenchmarkCategory(nameof(RemoveComponentOnT5))]
public abstract class RemoveComponentOnT5 : RemoveComponent;
