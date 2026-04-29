namespace Theia.ECS.Entities;

/// <summary>
/// Internal bookkeeping record that tracks where an <see cref="Entity"/>'s data lives across the <see cref="Worlds.World">World's</see>
/// storage structures, along with its current valid version.
/// </summary>
/// <remarks>
/// <para>
/// Each <see cref="Entity"/> ID maps to exactly one <see cref="EntityMeta"/> slot.
/// <br/>
/// The meta holds the indices needed to locate the entity's <see cref="Archetypes.Archetype">Archetype</see>,
/// its index within that archetype's storages, its components index, and its relations;
/// <b>meaning O(1) lookups</b> is enough to reach any of the entity's associated data.
/// </para>
/// <para>
/// When an entity is destroyed, <see cref="Reset"/> increments the version and invalidates
/// the indices, leaving the slot ready to be recycled for a future entity.
/// </para>
/// </remarks>
internal struct EntityMeta
{
    /// <summary>
    /// The initial version assigned to a fresh meta slot.
    /// <br/>
    /// Versions start at <c>1</c> so that a default-constructed <see cref="Entity"/> (version <c>0</c>) is always recognizable as invalid.
    /// </summary>
    internal const int DefaultEntityMetaVersion = 1;

    /// <summary>
    /// Sentinel value used for index fields when the meta does not yet point at any storage;
    /// for example, immediately after <see cref="Reset"/>.
    /// </summary>
    internal const int DefaultInvalidEntityMetaIndexes = -1;

    internal int _version;
    internal int _archetypeIndex;
    internal int _storageIndex;
    internal int _componentIndex;
    internal int _relationsIndexerIndex;

    internal EntityMeta(
        int version,
        int archetypeIndex = DefaultInvalidEntityMetaIndexes,
        int storageIndex = DefaultInvalidEntityMetaIndexes,
        int componentIndex = DefaultInvalidEntityMetaIndexes,
        int relationsIndexerIndex = DefaultInvalidEntityMetaIndexes
    )
    {
        _version = version;
        _archetypeIndex = archetypeIndex;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
        _relationsIndexerIndex = relationsIndexerIndex;
    }

    /// <summary>
    /// Recycles the slot by incrementing the version and clearing all index fields back to
    /// <see cref="DefaultInvalidEntityMetaIndexes"/>.
    /// <br/>
    /// After this call, any previously issued <see cref="Entity"/> handle pointing to this
    /// slot will be detectable as stale.
    /// </summary>
    internal void Reset()
    {
        _version++;
        _archetypeIndex = DefaultInvalidEntityMetaIndexes;
        _storageIndex = DefaultInvalidEntityMetaIndexes;
        _componentIndex = DefaultInvalidEntityMetaIndexes;
        _relationsIndexerIndex = DefaultInvalidEntityMetaIndexes;
    }
}
