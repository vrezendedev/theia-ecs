using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class DestroyEntity
{
    [Params(16, 512, 1_024, 4_096, 8_192, 16_384, 32_768, 65_536, 131_072)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(DestroyEntityT1))]
public abstract class DestroyEntityT1 : DestroyEntity;

[BenchmarkCategory(nameof(DestroyEntityT3))]
public abstract class DestroyEntityT3 : DestroyEntity;

[BenchmarkCategory(nameof(DestroyEntityT5))]
public abstract class DestroyEntityT5 : DestroyEntity;
