using System;
using MessagePack;

namespace Theia.ECS.Entities;

/// <summary>
/// A lightweight identifier that uniquely refers to an entity within the <see cref="Worlds.World">World</see>.
/// </summary>
/// <remarks>
/// <para>
/// An <see cref="Entity"/> is a reference to an entity, not the entity itself.
/// <br/>
/// It pairs an integer ID, which locates the entity's <see cref="EntityMeta">metadata</see> slot, with a version counter used to
/// detect stale handles.
/// </para>
/// </remarks>
[MessagePackObject(AllowPrivate = true)]
public readonly struct Entity : IEquatable<Entity>
{
    /// <summary>
    /// The slot index that locates this entity <see cref="EntityMeta">metadata</see> within <see cref="Worlds.World">World's</see> storage.
    /// </summary>
    [Key(0)]
    internal readonly int _id { get; init; }

    /// <summary>
    /// The generational counter used to distinguish from later occupants of the
    /// same slot. Not serialized since versions are not preserved across save or load boundaries.
    /// </summary>
    [IgnoreMember]
    internal readonly int _version { get; init; }

    /// <summary>
    /// Gets the underlying slot index of this entity.
    /// </summary>
    /// <remarks>
    /// Intended for diagnostics, logging, and interop only. Code that needs to identify or
    /// compare entities should use the <see cref="Entity"/> value itself, since the ID alone
    /// does not account for the version.
    /// </remarks>
    [IgnoreMember]
    public int ID => _id;

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="other"/> refers to the same entity,
    /// that is, both the ID and version match.
    /// </summary>
    public bool Equals(Entity other) => _id == other._id && _version == other._version;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    /// <summary>
    /// Returns a hash code derived solely from the entity ID.
    /// </summary>
    public override int GetHashCode() => _id;

    public override string ToString() => $"{nameof(Entity)}(ID: {_id} | Version: {_version})";

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
}
