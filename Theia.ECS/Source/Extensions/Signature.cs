using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Components;
using Theia.ECS.Contracts;

namespace Theia.ECS.Extensions;

internal static class SignatureExtensions
{
    extension(Signature)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ValidateComponents(ReadOnlySpan<int> componentIds)
        {
            for (int i = 0; i < componentIds.Length; i++)
            for (int j = i + 1; j < componentIds.Length; j++)
                if (componentIds[i] == componentIds[j])
                    ThrowArgumentExceptionDuplicatedComponentOnSignature(componentIds[i]);
        }

        [DoesNotReturn]
        private static void ThrowArgumentExceptionDuplicatedComponentOnSignature(int id) =>
            throw new ArgumentException(
                $"Component '{ComponentsMeta.GetComponentType(id)._type.Name}' with ID {id} is duplicated on Signature."
            );

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetMaskLength(int maxId) => (maxId >> 6) + 1;

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

        public static bool IsEqual(
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
    }
}
