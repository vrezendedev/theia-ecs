namespace Theia.ECS.Contracts;

internal readonly ref struct EntitySwapped
{
    internal readonly int _entityID = -1;
    internal readonly int _componentIndex = -1;

    internal EntitySwapped(int entityId, int componentIndex)
    {
        _entityID = entityId;
        _componentIndex = componentIndex;
    }

    internal static EntitySwapped None => new();
}
