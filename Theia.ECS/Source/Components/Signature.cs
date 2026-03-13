using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Extensions;

namespace Theia.ECS.Components;

internal readonly struct Signature : IEquatable<Signature>
{
    private readonly int[] _components;
    internal readonly int _length;

    internal readonly int _sizeOf;
    internal readonly int _maxId;

    private readonly ulong[] _mask;
    internal readonly int _maskLength;

    internal Signature(ReadOnlySpan<int> componentIds)
    {
        Signature.ValidateComponents(componentIds);

        _components = componentIds.ToArray();
        _length = componentIds.Length;

        SignatureMeta signatureMeta = Signature.GetSignatureMeta(componentIds);

        _sizeOf = signatureMeta._size;
        _maxId = signatureMeta._maxId;
        _maskLength = signatureMeta._maskLength;

        Span<ulong> mask = stackalloc ulong[_maskLength];

        Signature.GetSignatureMask(componentIds, mask);

        _mask = mask.ToArray();
    }

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

        Span<ulong> mask = stackalloc ulong[_maskLength];

        Signature.GetSignatureMask(componentIds, mask);

        _mask = mask.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> GeComponents() => _components;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<ulong> GetMask() => _mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsSatisfiedBy(Signature other) => Signature.IsSatisfiedBy(_mask, other._mask);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Signature other) =>
        Signature.IsEqual(_length, other._length, _mask, other._mask);

    public override bool Equals(object? obj) => obj is Signature other && Equals(other);

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();

        for (int i = 0; i < _mask.Length; i++)
            hash.Add(_mask[i]);

        return hash.ToHashCode();
    }

    public override string ToString() =>
        $"{nameof(Signature)}(Components: {string.Join(", ", _components)} | Length: {_length} | Sizeof: {_sizeOf} | MaxId: {_maxId} | Mask: {string.Join(", ", _mask)} | Mask Length: {_maskLength})";

    public static bool operator ==(Signature left, Signature right) => left.Equals(right);

    public static bool operator !=(Signature left, Signature right) => !left.Equals(right);
}
