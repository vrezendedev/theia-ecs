using System;

namespace Theia.ECS.Contracts;

/// <summary>
/// Composite key identifying a resource bucket in a <see cref="Worlds.World">World's</see> resource
/// dictionary by the pair of <see cref="Type"/>s that parameterize <see cref="Resources.Resources{TKey, TData}">Resources{TKey, TData}</see>.
/// </summary>
internal readonly struct ResourcesIdentifier
{
    internal readonly Type _key;
    internal readonly Type _data;

    internal ResourcesIdentifier(Type key, Type data)
    {
        _key = key;
        _data = data;
    }

    public bool Equals(ResourcesIdentifier other) =>
        ReferenceEquals(_key, other._key) && ReferenceEquals(_data, other._data);

    public override bool Equals(object? obj) => obj is ResourcesIdentifier other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_key, _data);

    public static bool operator ==(ResourcesIdentifier left, ResourcesIdentifier right) =>
        left.Equals(right);

    public static bool operator !=(ResourcesIdentifier left, ResourcesIdentifier right) =>
        !left.Equals(right);
}
