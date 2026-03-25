namespace Theia.ECS.Relations;

internal abstract class Relation
{
    protected const int DefaultRelationGrowthFactor = 2;

    internal abstract void Reset();
}
