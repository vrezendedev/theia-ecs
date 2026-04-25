using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class GetRelations
{
    [Params(1, 16, 100)]
    public int Relations { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(GetTagRelationOnT1))]
public abstract class GetTagRelationOnT1 : GetRelations;

[BenchmarkCategory(nameof(GetAllTagRelationsOnT1))]
public abstract class GetAllTagRelationsOnT1 : GetRelations;

[BenchmarkCategory(nameof(GetEvaluatedRelationOnT1))]
public abstract class GetEvaluatedRelationOnT1 : GetRelations;

[BenchmarkCategory(nameof(GetAllEvaluatedRelationsOnT1))]
public abstract class GetAllEvaluatedRelationsOnT1 : GetRelations;
