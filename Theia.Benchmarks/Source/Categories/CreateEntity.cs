using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class CreateEntity
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(CreateEntityT1))]
public abstract class CreateEntityT1 : CreateEntity;

[BenchmarkCategory(nameof(CreateEntityT3))]
public abstract class CreateEntityT3 : CreateEntity;

[BenchmarkCategory(nameof(CreateEntityT5))]
public abstract class CreateEntityT5 : CreateEntity;
