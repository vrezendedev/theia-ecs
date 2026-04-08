using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class AddComponent
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(AddComponentOnT1))]
public abstract class AddComponentOnT1 : AddComponent;

[BenchmarkCategory(nameof(AddComponentOnT3))]
public abstract class AddComponentOnT3 : AddComponent;

[BenchmarkCategory(nameof(AddComponentOnT5))]
public abstract class AddComponentOnT5 : AddComponent;
