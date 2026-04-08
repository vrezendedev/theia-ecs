using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class CrossArchetypeQuery
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(CrossArchetypeQueryOnT1))]
public abstract class CrossArchetypeQueryOnT1 : CrossArchetypeQuery;

[BenchmarkCategory(nameof(CrossArchetypeQueryOnT3))]
public abstract class CrossArchetypeQueryOnT3 : CrossArchetypeQuery;

[BenchmarkCategory(nameof(CrossArchetypeQueryOnT5))]
public abstract class CrossArchetypeQueryOnT5 : CrossArchetypeQuery;
