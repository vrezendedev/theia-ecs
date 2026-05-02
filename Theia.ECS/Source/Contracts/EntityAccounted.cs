namespace Theia.ECS.Contracts;

/// <summary>
/// Result of placing an entity into an archetype's chunk: the archetype, storage, and component
/// indices locating the entity's row. Carried back to the caller so the entity's
/// <see cref="Entities.EntityMeta">EntityMeta</see> can be patched to point at the new placement.
/// </summary>
internal readonly ref struct EntityAccounted
{
    internal readonly int _archetypeIndex;
    internal readonly int _storageIndex;
    internal readonly int _componentIndex;

    internal EntityAccounted(int archetypeIndex, int storageIndex, int componentIndex)
    {
        _archetypeIndex = archetypeIndex;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
    }
}
