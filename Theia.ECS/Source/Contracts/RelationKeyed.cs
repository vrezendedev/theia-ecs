using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

internal readonly ref struct RelationKeyed
{
    internal readonly Relation _relation;
    internal readonly int _primaryKey;

    internal RelationKeyed(Relation relation, int primaryKey)
    {
        _relation = relation;
        _primaryKey = primaryKey;
    }
}
