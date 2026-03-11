using System;
using Theia.ECS.Archetypes;

namespace Theia.ECS.Contracts;

internal readonly ref struct EntityAccounted : IEquatable<EntityAccounted>
{
    internal readonly Archetype _archetype;
    internal readonly int _storageIndex;
    internal readonly int _componentIndex;

    internal EntityAccounted(Archetype archetype, int storageIndex, int componentIndex)
    {
        _archetype = archetype;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
    }

    public bool Equals(EntityAccounted other) =>
        _archetype == other._archetype
        && _storageIndex == other._storageIndex
        && _componentIndex == other._componentIndex;
}
