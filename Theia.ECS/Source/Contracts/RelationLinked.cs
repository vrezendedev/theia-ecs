using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

/// <summary>
/// Result of attempting to commit the bilateral link after a successful
/// <see cref="RelationAccounted"/>. Carries the owner-side <see cref="Relation"/>, its primary
/// key, and the composite key locating the new edge inside the relation or, when the link
/// could not be committed, <see cref="Reproved"/> with all fields cleared.
/// </summary>
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

    /// <summary>The "rejected" sentinel; <see cref="_linked"/> is <see langword="false"/> and the keys are set to invalid markers.</summary>
    internal static RelationLinked Reproved =>
        new(false, null!, InvalidRelationLinkedKeys, InvalidRelationLinkedKeys);
}
