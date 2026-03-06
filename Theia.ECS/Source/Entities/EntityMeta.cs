using System;

namespace Theia.ECS.Entities;

internal struct EntityMeta : IEquatable<EntityMeta>
{
    internal int _version { get; set; }
    internal int _archetypeIndex { get; set; }
    internal int _componentStorageIndex { get; set; }
    internal int _componentIndex { get; set; }

    public bool Equals(EntityMeta other) =>
        _version == other._version
        && _archetypeIndex == other._archetypeIndex
        && _componentStorageIndex == other._componentStorageIndex
        && _componentIndex == other._componentIndex;

    public override bool Equals(object? obj) => obj is EntityMeta other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(_version, _archetypeIndex, _componentStorageIndex, _componentIndex);

    public override string ToString() =>
        $"{nameof(EntityMeta)}(Version: {_version} | ArchetypeIndex: {_archetypeIndex} | ComponentStorageIndex: {_componentStorageIndex} | ComponentIndex: {_componentIndex})";

    public static bool operator ==(EntityMeta left, EntityMeta right) => left.Equals(right);

    public static bool operator !=(EntityMeta left, EntityMeta right) => !left.Equals(right);
}
