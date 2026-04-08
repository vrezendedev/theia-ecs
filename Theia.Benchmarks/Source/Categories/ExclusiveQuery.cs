using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class ExclusiveQuery
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(ExclusiveQueryOnT1))]
public abstract class ExclusiveQueryOnT1 : ExclusiveQuery;

[BenchmarkCategory(nameof(ExclusiveQueryOnT3))]
public abstract class ExclusiveQueryOnT3 : ExclusiveQuery;

[BenchmarkCategory(nameof(ExclusiveQueryOnT5))]
public abstract class ExclusiveQueryOnT5 : ExclusiveQuery;
