namespace Theia.ECS.Contracts;

internal readonly ref struct EntitySwapped
{
    internal readonly int _entityID = -1;
    internal readonly int _componentID = -1;

    internal EntitySwapped(int entityId, int componentId)
    {
        _entityID = entityId;
        _componentID = componentId;
    }

    internal static EntitySwapped None => new();
}
