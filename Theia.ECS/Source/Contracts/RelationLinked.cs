using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

internal readonly ref struct RelationLinked
{
    private const int InvalidRelationLinkedKeys = -1;

    internal readonly bool _linked;
    internal readonly Relation _relation;
    internal readonly int _primaryKey;
    internal readonly int _compositeKey;

    internal RelationLinked(bool linked, Relation relation, int primaryKey, int compositeKey)
    {
        _linked = linked;
        _relation = relation;
        _primaryKey = primaryKey;
        _compositeKey = compositeKey;
    }

    internal static RelationLinked Reproved =>
        new(false, null!, InvalidRelationLinkedKeys, InvalidRelationLinkedKeys);
}
