using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

internal readonly ref struct RelationAccounted
{
    internal readonly int _primaryKey;
    internal readonly Relation _relation;
    internal readonly RelationLink _relationLink;

    internal RelationAccounted(int primaryKey, Relation relation, RelationLink relationLink)
    {
        _primaryKey = primaryKey;
        _relation = relation;
        _relationLink = relationLink;
    }
}
