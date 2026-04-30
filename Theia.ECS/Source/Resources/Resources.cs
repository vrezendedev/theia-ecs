using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Resources;

/// <summary>
/// Per-<see cref="Worlds.World">World</see> store for managed reference-type data keyed by an enum.
/// Components in Theia ECS must be strictly blittable, so they cannot hold managed references
/// directly; instead, a component holds a small <typeparamref name="TKey"/> value and looks the
/// associated <typeparamref name="TData"/> up here in O(1) via bare array indexing.
/// </summary>
/// <typeparam name="TKey">
/// An enum whose underlying type is <see cref="int"/>, with values dense and starting at zero.
/// Both constraints are enforced once per closed generic in the static constructor and reported
/// with a diagnostic that names the offending position.
/// </typeparam>
/// <typeparam name="TData">
/// The managed reference type held in the store; typically game assets such as sprites, audio
/// clips, prefabs, or other resources that components reference by key but cannot store directly.
/// </typeparam>
/// <remarks>
/// <para>
/// Instances are created and populated exclusively through
/// <c>World.CreateResource&lt;TKey, TData&gt;(...)</c>, which validates that every enum value is
/// registered exactly once. After construction the store has no public mutators: lookups are the
/// only operation user code can perform on it. This is enforced by the type system, not by a
/// runtime flag; there is simply no public way to modify a <see cref="Resources{TKey, TData}"/>
/// instance.
/// </para>
/// <para>
/// Each closed generic instantiation carries its own static <see cref="Keys"/> cache, populated
/// once on first access. Per-instance state lives in a single jagged array indexed by the enum's
/// underlying integer; lookups reinterpret the enum as an <see cref="int"/> via
/// <see cref="Unsafe.As{TFrom, TTo}(ref TFrom)"/> for a zero-cost conversion.
/// </para>
/// <para>
/// <b>Thread safety:</b> reads from <c>Get</c> are safe to perform concurrently. Writes happen
/// only inside <c>World.CreateResource</c>, which is <b>expected to run during world setup before
/// any system reads from the store</b>.
/// </para>
/// </remarks>
public sealed class Resources<TKey, TData>
    where TKey : unmanaged, Enum
    where TData : class
{
    private static readonly TKey[] s_keys;

    /// <summary>
    /// All values of <typeparamref name="TKey"/>, cached once per closed generic. Used by
    /// <c>World.CreateResource</c> to verify that every enum value has been registered.
    /// </summary>
    internal static ReadOnlySpan<TKey> Keys => s_keys;

    private readonly TData[][] _data;

    static Resources()
    {
        if (Enum.GetUnderlyingType(typeof(TKey)) != typeof(int))
            throw new InvalidOperationException(
                $"'{typeof(TKey).Name}' must have an underlying type of int. Declare the enum as 'enum {typeof(TKey).Name} : int'."
            );

        s_keys = Enum.GetValues<TKey>();

        for (int i = 0; i < Keys.Length; i++)
        {
            TKey k = Keys[i];
            if (Unsafe.As<TKey, int>(ref k) != i)
                throw new InvalidOperationException(
                    $"Enum '{typeof(TKey).Name}' must have dense values starting at 0. "
                        + $"Found value {Unsafe.As<TKey, int>(ref k)} at position {i}."
                );
        }
    }

    internal Resources(int length) => _data = new TData[length][];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Register(TKey key, TData[] data)
    {
        int index = Unsafe.As<TKey, int>(ref key);
        _data[index] = data;
    }

    /// <summary>
    /// Returns the array registered under <paramref name="key"/> as a <see cref="ReadOnlySpan{T}"/>,
    /// preventing the caller from mutating the underlying storage.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<TData> Get(TKey key) => _data[Unsafe.As<TKey, int>(ref key)];

    /// <summary>
    /// Returns a read-only reference to the element at <paramref name="index"/> within the array
    /// registered under <paramref name="key"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly TData Get(TKey key, int index) =>
        ref _data[Unsafe.As<TKey, int>(ref key)][index];
}
