using System;
using Theia.ECS.Archetypes;

namespace Theia.ECS.Entities;

internal struct EntityMeta : IEquatable<EntityMeta>
{
    internal int _version { get; set; }
    internal Archetype _archetype { get; set; }
    internal int _storageIndex { get; set; }
    internal int _componentIndex { get; set; }

    public bool Equals(EntityMeta other) =>
        _version == other._version
        && _archetype == other._archetype
        && _storageIndex == other._storageIndex
        && _componentIndex == other._componentIndex;

    public override bool Equals(object? obj) => obj is EntityMeta other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(_version, _archetype?._archetypeId ?? -1, _storageIndex, _componentIndex);

    public override string ToString() =>
        $"{nameof(EntityMeta)}(Version: {_version} | Archetype: {_archetype} | StorageIndex: {_storageIndex} | ComponentIndex: {_componentIndex})";

    public static bool operator ==(EntityMeta left, EntityMeta right) => left.Equals(right);

    public static bool operator !=(EntityMeta left, EntityMeta right) => !left.Equals(right);
}
