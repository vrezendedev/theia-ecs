using System;

namespace Theia.ECS.Contracts;

internal readonly ref struct EntityAccounted : IEquatable<EntityAccounted>
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

    public bool Equals(EntityAccounted other) =>
        _archetypeIndex == other._archetypeIndex
        && _storageIndex == other._storageIndex
        && _componentIndex == other._componentIndex;
}
