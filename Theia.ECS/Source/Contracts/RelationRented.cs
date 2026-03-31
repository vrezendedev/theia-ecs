using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

internal readonly ref struct RelationRented
{
    internal readonly Relation _relation;
    internal readonly int _primaryKey;

    internal RelationRented(in Relation relation, int primaryKey)
    {
        _relation = relation;
        _primaryKey = primaryKey;
    }
}
