using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class AddRelations
{
    [Params(1, 16, 100)]
    public int Relations { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(AddTagRelationOnT1))]
public abstract class AddTagRelationOnT1 : AddRelations;

[BenchmarkCategory(nameof(AddEvaluatedRelationOnT1))]
public abstract class AddEvaluatedRelationOnT1 : AddRelations;
