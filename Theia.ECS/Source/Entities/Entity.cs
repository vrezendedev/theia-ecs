using System;

namespace Theia.ECS.Entities;

public readonly struct Entity : IEquatable<Entity>
{
    internal readonly int _id { get; init; }
    internal readonly int _version { get; init; }

    public int ID => _id;

    public bool Equals(Entity other) => _id == other._id && _version == other._version;

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public override int GetHashCode() => _id;

    public override string ToString() => $"{nameof(Entity)}(ID: {_id} | Version: {_version})";

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
}
