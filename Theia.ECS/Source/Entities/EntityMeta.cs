using System;

namespace Theia.ECS.Entities;

internal struct EntityMeta : IEquatable<EntityMeta>
{
    internal const int DefaultEntityMetaVersion = 1;
    internal const int DefaultInvalidEntityMetaIndexes = -1;

    internal int _version;
    internal int _archetypeIndex;
    internal int _storageIndex;
    internal int _componentIndex;
    internal int _relationsIndexerIndex;

    internal EntityMeta(
        int version,
        int archetypeIndex = DefaultInvalidEntityMetaIndexes,
        int storageIndex = DefaultInvalidEntityMetaIndexes,
        int componentIndex = DefaultInvalidEntityMetaIndexes,
        int relationsIndexerIndex = DefaultInvalidEntityMetaIndexes
    )
    {
        _version = version;
        _archetypeIndex = archetypeIndex;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
        _relationsIndexerIndex = relationsIndexerIndex;
    }

    public void Reset()
    {
        _version++;
        _archetypeIndex = DefaultInvalidEntityMetaIndexes;
        _storageIndex = DefaultInvalidEntityMetaIndexes;
        _componentIndex = DefaultInvalidEntityMetaIndexes;
        _relationsIndexerIndex = DefaultInvalidEntityMetaIndexes;
    }

    public bool Equals(EntityMeta other) =>
        _version == other._version
        && _archetypeIndex == other._archetypeIndex
        && _storageIndex == other._storageIndex
        && _componentIndex == other._componentIndex
        && _relationsIndexerIndex == other._relationsIndexerIndex;

    public override bool Equals(object? obj) => obj is EntityMeta other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(
            _version,
            _archetypeIndex,
            _storageIndex,
            _componentIndex,
            _relationsIndexerIndex
        );

    public static bool operator ==(EntityMeta left, EntityMeta right) => left.Equals(right);

    public static bool operator !=(EntityMeta left, EntityMeta right) => !left.Equals(right);
}
