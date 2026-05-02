namespace Theia.ECS.Contracts;

/// <summary>
/// Result of a chunk-internal swap-remove: when the removed entity was not already last,
/// another entity is moved into the freed row. Carries the moved entity's ID and its new dense
/// index so the caller can patch the entity's <see cref="Entities.EntityMeta"/> to reflect the
/// new row.
/// </summary>
/// <remarks>
/// <see cref="None"/> is returned when the removed entity was already at the last position and
/// no swap occurred.
/// </remarks>
internal readonly ref struct EntitySwapped
{
    /// <summary>Sentinel value for both fields when no swap occurred; see <see cref="None"/>.</summary>
    internal const int InvalidEntitySwappedIndexes = -1;
    internal readonly int _entityID;
    internal readonly int _dataIndex;

    internal EntitySwapped(int entityId, int dataIndex)
    {
        _entityID = entityId;
        _dataIndex = dataIndex;
    }

    /// <summary>The "no swap occurred" result. Both fields are set to <see cref="InvalidEntitySwappedIndexes"/>.</summary>
    internal static EntitySwapped None =>
        new(InvalidEntitySwappedIndexes, InvalidEntitySwappedIndexes);
}
