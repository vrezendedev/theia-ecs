using System;

namespace Theia.ECS.Entities;

internal struct EntityMeta : IEquatable<EntityMeta>
{
    internal const int DefaultEntityMetaVersion = 1;
    internal const int DefaultInvalidEntityMetaIndexes = -1;
    internal int _version { get; set; }
    internal int _archetypeIndex { get; set; }
    internal int _storageIndex { get; set; }
    internal int _componentIndex { get; set; }

    internal EntityMeta(int version, int archetypeIndex, int storageIndex, int componentIndex)
    {
        _version = version;
        _archetypeIndex = archetypeIndex;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
    }

    public bool Equals(EntityMeta other) =>
        _version == other._version
        && _archetypeIndex == other._archetypeIndex
        && _storageIndex == other._storageIndex
        && _componentIndex == other._componentIndex;

    public override bool Equals(object? obj) => obj is EntityMeta other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(_version, _archetypeIndex, _storageIndex, _componentIndex);

    public override string ToString() =>
        $"{nameof(EntityMeta)}(Version: {_version} | Archetype: {_archetypeIndex} | StorageIndex: {_storageIndex} | ComponentIndex: {_componentIndex})";

    public static bool operator ==(EntityMeta left, EntityMeta right) => left.Equals(right);

    public static bool operator !=(EntityMeta left, EntityMeta right) => !left.Equals(right);
}
