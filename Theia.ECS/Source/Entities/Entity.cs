using System;
using MessagePack;

namespace Theia.ECS.Entities;

[MessagePackObject(AllowPrivate = true)]
public readonly struct Entity : IEquatable<Entity>
{
    [Key(0)]
    internal readonly int _id { get; init; }

    [IgnoreMember]
    internal readonly int _version { get; init; }

    [IgnoreMember]
    public int ID => _id;

    public bool Equals(Entity other) => _id == other._id && _version == other._version;

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public override int GetHashCode() => _id;

    public override string ToString() => $"{nameof(Entity)}(ID: {_id} | Version: {_version})";

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
}
