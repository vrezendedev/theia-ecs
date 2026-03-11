using System;

namespace Theia.ECS.Contracts;

internal readonly ref struct EntitySwapped : IEquatable<EntitySwapped>
{
    internal readonly int _entityID = -1;
    internal readonly int _storageIndex = -1;
    internal readonly int _componentIndex = -1;

    internal EntitySwapped(int entityId, int storageIndex, int componentIndex)
    {
        _entityID = entityId;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
    }

    internal static EntitySwapped None => new();

    public bool Equals(EntitySwapped other) =>
        _entityID == other._entityID
        && _storageIndex == other._storageIndex
        && _componentIndex == other._componentIndex;
}
