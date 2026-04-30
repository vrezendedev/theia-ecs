using System;

namespace Theia.ECS.Contracts;

/// <summary>
/// One entry in a <see cref="Worlds.World">World's</see> resource registration call. Pairs a single enum
/// <typeparamref name="TKey"/> with the array of <typeparamref name="TData"/> instances it
/// represents.
/// </summary>
/// <typeparam name="TKey">An enum whose underlying type is <see cref="int"/>, with values dense and starting at zero.</typeparam>
/// <typeparam name="TData">The managed reference type held under this key.</typeparam>
public struct ResourcesEntries<TKey, TData>
    where TKey : unmanaged, Enum
    where TData : class
{
    /// <summary>The enum value this entry registers data under.</summary>
    public required TKey Key { get; init; }

    /// <summary>The array of <typeparamref name="TData"/> associated with <see cref="Key"/>.</summary>
    public required TData[] Data { get; init; }
}
