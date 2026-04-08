using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class CreateEntityDeferred
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(CreateEntityDeferredT1))]
public abstract class CreateEntityDeferredT1 : CreateEntityDeferred;

[BenchmarkCategory(nameof(CreateEntityDeferredT3))]
public abstract class CreateEntityDeferredT3 : CreateEntityDeferred;

[BenchmarkCategory(nameof(CreateEntityDeferredT5))]
public abstract class CreateEntityDeferredT5 : CreateEntityDeferred;
