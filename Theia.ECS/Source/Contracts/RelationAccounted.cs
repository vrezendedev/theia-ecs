using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

internal readonly ref struct RelationAccounted
{
    internal readonly Relation _relation;
    internal readonly int _primaryKey;

    internal RelationAccounted(in Relation relation, int primaryKey)
    {
        _relation = relation;
        _primaryKey = primaryKey;
    }
}
