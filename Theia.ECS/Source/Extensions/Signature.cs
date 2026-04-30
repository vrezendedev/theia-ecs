using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Components;
using Theia.ECS.Contracts;

namespace Theia.ECS.Extensions;

/// <summary>
/// Static helpers extending <see cref="Signature"/> with the bitmask construction and matching
/// primitives used during signature creation and query-archetype evaluation.
/// </summary>
internal static class SignatureExtensions
{
    extension(Signature)
    {
        /// <summary>
        /// Verifies that <paramref name="componentIds"/> contains no duplicates, throwing if any
        /// ID appears more than once. Quadratic in the length of the span; acceptable because
        /// signatures are typically a handful of components and validation runs once at construction.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if any component ID appears more than once in <paramref name="componentIds"/>.
        /// </exception>
        internal static void ValidateComponents(ReadOnlySpan<int> componentIds)
        {
            for (int i = 0; i < componentIds.Length; i++)
            for (int j = i + 1; j < componentIds.Length; j++)
                if (componentIds[i] == componentIds[j])
                    ThrowDuplicatedComponentOnSignatureException(componentIds[i]);
        }

        /// <summary>
        /// Computes the derived metadata for a signature: the total byte size of all components,
        /// the largest component ID, and the resulting mask bucket count. Read once during
        /// construction and cached on the <see cref="Signature"/> instance.
        /// </summary>
        internal static SignatureMeta GetSignatureMeta(ReadOnlySpan<int> componentIds)
        {
            int size = 0;
            int maxId = -1;

            for (int i = 0; i < componentIds.Length; i++)
            {
                size += ComponentsMeta.GetComponentType(componentIds[i])._sizeOf;
                maxId = Math.Max(maxId, componentIds[i]);
            }

            return new SignatureMeta()
            {
                _size = size,
                _maxId = maxId,
                _maskLength = GetMaskLength(maxId),
            };
        }

        /// <summary>
        /// Returns the number of <see cref="ulong"/> buckets required to address every bit up to
        /// and including <paramref name="maxId"/>. Each bucket holds 64 bits, so the result is
        /// <c>(maxId / 64) + 1</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetMaskLength(int maxId) => (maxId >> 6) + 1;

        /// <summary>
        /// Sets one bit per component ID in <paramref name="mask"/>: bit <c>id</c> is set in bucket
        /// <c>id / 64</c> at position <c>id % 64</c>.
        /// </summary>
        internal static Span<ulong> SetSignatureMask(
            ReadOnlySpan<int> componentIds,
            Span<ulong> mask
        )
        {
            for (int i = 0; i < componentIds.Length; i++)
            {
                int componentId = componentIds[i];
                mask[componentId >> 6] |= 1UL << (componentId & 63);
            }

            return mask;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="bMask"/> contains every bit set in
        /// <paramref name="aMask"/>.The check short-circuits on the first word where
        /// <paramref name="bMask"/> is missing a required bit.
        /// </summary>
        /// <remarks>
        /// The directional reading is <c>a.IsSatisfiedBy(b)</c> = "<c>a</c>'s requirements are
        /// satisfied by <c>b</c>'s composition." For query-archetype matching, <paramref name="aMask"/>
        /// is the query's required components and <paramref name="bMask"/> is the archetype's.
        /// </remarks>
        internal static bool IsSatisfiedBy(ReadOnlySpan<ulong> aMask, ReadOnlySpan<ulong> bMask)
        {
            if (bMask.Length < aMask.Length)
                return false;

            for (int i = 0; i < aMask.Length; i++)
            {
                if ((aMask[i] & bMask[i]) != aMask[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns <see langword="true"/> if both signatures <b>contain exactly the same components</b>.
        /// Compares lengths first to short-circuit when sizes differ, then XORs each mask word;
        /// a non-zero result means a bit differs.
        /// </summary>
        internal static bool IsEqual(
            int aComponentsLength,
            int bComponentsLength,
            ReadOnlySpan<ulong> aMask,
            ReadOnlySpan<ulong> bMask
        )
        {
            if (aComponentsLength != bComponentsLength || aMask.Length != bMask.Length)
                return false;

            for (int i = 0; i < aMask.Length; i++)
            {
                if ((aMask[i] ^ bMask[i]) != 0)
                    return false;
            }

            return true;
        }

        [DoesNotReturn]
        private static void ThrowDuplicatedComponentOnSignatureException(int id) =>
            throw new ArgumentException(
                $"Component '{ComponentsMeta.GetComponentType(id)._type.Name}' with ID {id} is duplicated on Signature."
            );
    }
}
