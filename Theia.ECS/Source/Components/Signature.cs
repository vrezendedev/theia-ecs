using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Extensions;

namespace Theia.ECS.Components;

/// <summary>
/// Immutable, hashable description of a set of component types; used by archetypes to identify
/// their composition and by queries to express their requirements.
/// Carries both the sorted list of component IDs and a precomputed bitmask sized to the largest
/// ID, so set-membership and superset tests run as a handful of <see cref="ulong"/> ANDs.
/// </summary>
/// <remarks>
/// <para>
/// The bitmask is the hot path. Set membership is a single bit test; checking whether one
/// signature is satisfied by another (i.e., whether the other contains every component of this
/// one) is an AND-and-compare across <see cref="_maskLength"/> words.
/// </para>
/// </remarks>
internal readonly struct Signature : IEquatable<Signature>
{
    private readonly int[] _components;
    internal readonly int _length;

    /// <summary>
    /// Sum of the byte sizes of every component in this signature, as reported by
    /// <see cref="ComponentsMeta"/>. Used by <see cref="Archetypes.Archetype">Archetype</see> storage.
    /// </summary>
    internal readonly int _sizeOf;
    internal readonly int _maxId;

    private readonly ulong[] _mask;
    internal readonly int _maskLength;

    /// <summary>
    /// Builds a signature from the given <paramref name="componentIds"/>, deriving size, max ID,
    /// and mask length from the registered <see cref="ComponentsMeta"/> entries.
    /// </summary>
    internal Signature(ReadOnlySpan<int> componentIds)
    {
        Signature.ValidateComponents(componentIds);

        _components = componentIds.ToArray();
        _length = componentIds.Length;

        SignatureMeta signatureMeta = Signature.GetSignatureMeta(componentIds);

        _sizeOf = signatureMeta._size;
        _maxId = signatureMeta._maxId;
        _maskLength = signatureMeta._maskLength;

        ulong[] mask = new ulong[_maskLength];

        Signature.SetSignatureMask(componentIds, mask);

        _mask = mask;
    }

    /// <summary>
    /// Builds a signature reusing precomputed <paramref name="signatureMeta"/> and
    /// <paramref name="mask"/> values. Used by hot paths that have already produced these
    /// values and want to avoid re-deriving them.
    /// </summary>
    internal Signature(
        ReadOnlySpan<int> componentIds,
        SignatureMeta signatureMeta,
        ReadOnlySpan<ulong> mask
    )
    {
        Signature.ValidateComponents(componentIds);

        _components = componentIds.ToArray();
        _length = componentIds.Length;

        _sizeOf = signatureMeta._size;
        _maxId = signatureMeta._maxId;
        _maskLength = signatureMeta._maskLength;

        _mask = mask.ToArray();
    }

    /// <summary>
    /// Bypass constructor intended exclusively for Unit Testing.
    /// Skips <see cref="ComponentsMeta"/> lookups, allowing arbitrary component IDs
    /// to be used without requiring registered component types.
    /// <para><b>Do not use outside of test projects.</b></para>
    /// </summary>
    /// <param name="componentIds">Arbitrary component IDs to build the signature from.</param>
    /// <param name="bypass">Acts as a compile-time discriminator to distinguish this overload.</param>
    [Obsolete("Test-only Constructor")]
    internal Signature(ReadOnlySpan<int> componentIds, bool bypass)
    {
        Signature.ValidateComponents(componentIds);

        _components = componentIds.ToArray();
        _length = componentIds.Length;

        int maxId = -1;

        for (int i = 0; i < componentIds.Length; i++)
            maxId = Math.Max(maxId, componentIds[i]);

        _sizeOf = 0;
        _maxId = maxId;
        _maskLength = Signature.GetMaskLength(maxId);

        ulong[] mask = new ulong[_maskLength];

        Signature.SetSignatureMask(componentIds, mask);

        _mask = mask;
    }

    /// <summary>Returns the sorted component IDs that make up this signature.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> GetComponents() => _components;

    /// <summary>Returns the precomputed bitmask, one bit per component ID, packed into <see cref="ulong"/> buckets.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<ulong> GetMask() => _mask;

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="other"/> contains every component of this
    /// signature.
    /// Used to match a query's required components against an archetype's composition.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsSatisfiedBy(Signature other) => Signature.IsSatisfiedBy(_mask, other._mask);

    /// <summary>
    /// Returns <see langword="true"/> if both signatures contain exactly the same set of components.
    /// Length-checked first to short-circuit when sizes differ.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Signature other) =>
        Signature.IsEqual(_length, other._length, _mask, other._mask);
}
