using System.Runtime.CompilerServices;

namespace Theia.ECS.Relations;

internal struct RelationKey
{
    internal const int InvalidKey = -1;

    private int _addedRelationIdIndex;
    private int _primaryKey;

    public RelationKey() => Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetPrimaryKey() => _primaryKey;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetPrimaryKey(int primaryKey) => _primaryKey = primaryKey;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasRelation() => _primaryKey != InvalidKey;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetIndexerAddedRelationIdIndex() => _addedRelationIdIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetIndexerAddedRelationIdIndex(int index) => _addedRelationIdIndex = index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _addedRelationIdIndex = InvalidKey;
        _primaryKey = InvalidKey;
    }
}
