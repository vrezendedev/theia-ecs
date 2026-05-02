namespace Theia.ECS.Contracts;

/// <summary>
/// Result of moving an entity from one archetype to another. Combines the
/// <see cref="EntityAccounted"/> describing the entity's new placement in the destination with
/// the <see cref="EntitySwapped"/> describing the swap-remove that occurred at the source.
/// </summary>
/// <remarks>
/// Returned by <see cref="Archetypes.Archetype.Transfer"/>. Callers use the accounted half to
/// patch the moved entity's metadata to its new chunk and row, and the swapped half to patch
/// the entity (if any) that filled the freed source row.
/// </remarks>
internal readonly ref struct EntityTransferred
{
    internal readonly EntityAccounted _entityAccounted;
    internal readonly EntitySwapped _entitySwapped;

    internal EntityTransferred(EntityAccounted entityAccounted, EntitySwapped entitySwapped)
    {
        _entityAccounted = entityAccounted;
        _entitySwapped = entitySwapped;
    }
}
