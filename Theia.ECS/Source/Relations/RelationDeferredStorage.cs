namespace Theia.ECS.Relations;

internal abstract class RelationDeferredStorage
{
    internal abstract void SetWith(int storageIndex, Relation relation, int compositeKey);
}
