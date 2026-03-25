namespace Theia.ECS.Contracts;

internal readonly ref struct EntitySwapped
{
    internal const int InvalidEntitySwappedIndexes = -1;
    internal readonly int _entityID;
    internal readonly int _dataIndex;

    internal EntitySwapped(int entityId, int dataIndex)
    {
        _entityID = entityId;
        _dataIndex = dataIndex;
    }

    internal static EntitySwapped None =>
        new(InvalidEntitySwappedIndexes, InvalidEntitySwappedIndexes);
}
