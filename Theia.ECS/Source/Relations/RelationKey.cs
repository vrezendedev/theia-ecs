using System.Runtime.CompilerServices;

namespace Theia.ECS.Relations;

/// <summary>
/// Per-(owner, relation type) bookkeeping entry held by <see cref="RelationsIndexer"/>. Records
/// the owner's slot in the matching <see cref="RelationStorage"/> and a back-pointer into the
/// indexer's dense added-list so removal is O(1) via swap.
/// </summary>
internal struct RelationKey
{
    /// <summary>Sentinel value for both fields when the slot is unoccupied.</summary>
    internal const int InvalidKey = -1;

    private int _addedRelationIdIndex;
    private int _primaryKey;

    public RelationKey() => Reset();

    /// <summary>Returns the owner's slot in the matching <see cref="RelationStorage"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetPrimaryKey() => _primaryKey;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetPrimaryKey(int primaryKey) => _primaryKey = primaryKey;

    /// <summary>Returns <see langword="true"/> if this entry currently records a relation; <see langword="false"/> when reset or never assigned.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasRelation() => _primaryKey != InvalidKey;

    /// <summary>Returns this entry's position in the indexer's dense added-relations list, used for O(1) removal via swap-with-last.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetIndexerAddedRelationIdIndex() => _addedRelationIdIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetIndexerAddedRelationIdIndex(int index) => _addedRelationIdIndex = index;

    /// <summary>Returns the entry to its empty state, ready to be repopulated for a new relation.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _addedRelationIdIndex = InvalidKey;
        _primaryKey = InvalidKey;
    }
}
