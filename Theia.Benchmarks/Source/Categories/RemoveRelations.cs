using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class RemoveRelations
{
    [Params(1, 100)]
    public int Relations { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(RemoveTagRelationOnT1))]
public abstract class RemoveTagRelationOnT1 : RemoveRelations;

[BenchmarkCategory(nameof(RemoveEvaluatedRelationOnT1))]
public abstract class RemoveEvaluatedRelationOnT1 : RemoveRelations;
