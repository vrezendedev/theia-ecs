namespace Theia.ECS.Contracts;

internal readonly ref struct EntitySwapped
{
    internal const int InvalidEntitySwappedIndexes = -1;
    internal readonly int _entityID;
    internal readonly int _componentIndex;

    internal EntitySwapped(int entityId, int componentIndex)
    {
        _entityID = entityId;
        _componentIndex = componentIndex;
    }

    internal static EntitySwapped None =>
        new(InvalidEntitySwappedIndexes, InvalidEntitySwappedIndexes);
}
