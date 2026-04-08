using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class DestroyEntityDeferred
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(DestroyEntityDeferredT1))]
public abstract class DestroyEntityDeferredT1 : DestroyEntityDeferred;

[BenchmarkCategory(nameof(DestroyEntityDeferredT3))]
public abstract class DestroyEntityDeferredT3 : DestroyEntityDeferred;

[BenchmarkCategory(nameof(DestroyEntityDeferredT5))]
public abstract class DestroyEntityDeferredT5 : DestroyEntityDeferred;
