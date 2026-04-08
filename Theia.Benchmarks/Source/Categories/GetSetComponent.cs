using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

[BenchmarkCategory(nameof(GetComponentOnT1))]
public abstract class GetComponentOnT1
{
    [Params(16, 512, 1_024, 4_096, 8_192, 16_384, 32_768, 65_536, 131_072)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(SetComponentOnT1))]
public abstract class SetComponentOnT1
{
    [Params(16, 512, 1_024, 4_096, 8_192, 16_384, 32_768, 65_536, 131_072)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}
