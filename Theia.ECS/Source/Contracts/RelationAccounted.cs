using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

/// <summary>
/// Result of attempting to record an "owner relates to target" intent before the bilateral link
/// is committed. Carries the owner-side <see cref="Relation"/>, its primary key in storage, and
/// the target-side <see cref="RelationLink"/> ready to receive the inverse edge or, when the
/// attempt was rejected (e.g., link already exists), <see cref="Reproved"/> with all fields cleared.
/// </summary>
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

    /// <summary>The "rejected" sentinel; <see cref="_accounted"/> is <see langword="false"/> and the reference fields are <see langword="null"/>. Returned when no further work should be done with this attempt.</summary>
    internal static RelationAccounted Reproved =>
        new(false, InvalidRelationAccountedPrimaryKey, null!, null!);
}
