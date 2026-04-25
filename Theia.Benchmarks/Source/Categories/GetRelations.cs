using BenchmarkDotNet.Attributes;

namespace Theia.Benchmarks.Source.Categories;

public abstract class GetRelations
{
    [Params(1, 100)]
    public int Relations { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    public abstract void Run();
}

[BenchmarkCategory(nameof(GetTagRelationOnT1))]
public abstract class GetTagRelationOnT1 : GetRelations;

[BenchmarkCategory(nameof(GetTagRelationsOnT1))]
public abstract class GetTagRelationsOnT1 : GetRelations;

[BenchmarkCategory(nameof(GetEvaluatedRelationOnT1))]
public abstract class GetEvaluatedRelationOnT1 : GetRelations;

[BenchmarkCategory(nameof(GetEvaluatedRelationsOnT1))]
public abstract class GetEvaluatedRelationsOnT1 : GetRelations;
