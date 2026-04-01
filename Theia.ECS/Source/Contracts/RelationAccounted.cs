using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

internal readonly ref struct RelationAccounted
{
    private const int InvalidRelationAccountedPrimaryKey = -1;
    internal readonly bool _accounted;
    internal readonly int _primaryKey;
    internal readonly Relation _relation;
    internal readonly RelationLink _targetRelationLink;

    internal RelationAccounted(
        bool accounted,
        int primaryKey,
        Relation relation,
        RelationLink targetRelationLink
    )
    {
        _accounted = accounted;
        _primaryKey = primaryKey;
        _relation = relation;
        _targetRelationLink = targetRelationLink;
    }

    internal static RelationAccounted Reproved =>
        new(false, InvalidRelationAccountedPrimaryKey, null!, null!);
}
