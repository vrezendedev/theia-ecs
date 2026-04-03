namespace Theia.ECS.Relations;

internal sealed class RelationDeferredStorage<TRelation> : RelationDeferredStorage
    where TRelation : struct
{
    private readonly TRelation[] _values;

    internal RelationDeferredStorage(int capacity) => _values = new TRelation[capacity];

    internal override void SetWith(int storageIndex, Relation relation, int compositeKey) { }
}
