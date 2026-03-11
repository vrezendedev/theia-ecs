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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                _bucketCount = GetBucketCount(maxId),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetBucketCount(int maxId) => (maxId >> 6) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Span<ulong> GetSignatureBuckets(
            ReadOnlySpan<int> componentIds,
            Span<ulong> buckets
        )
        {
            for (int i = 0; i < componentIds.Length; i++)
            {
                int componentId = componentIds[i];
                buckets[componentId >> 6] |= 1UL << (componentId & 63);
            }

            return buckets;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSatisfiedBy(
            ReadOnlySpan<ulong> aBuckets,
            ReadOnlySpan<ulong> bBuckets
        )
        {
            if (bBuckets.Length < aBuckets.Length)
                return false;

            for (int i = 0; i < aBuckets.Length; i++)
            {
                if ((aBuckets[i] & bBuckets[i]) != aBuckets[i])
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(
            int aComponentsLength,
            int bComponentsLength,
            ReadOnlySpan<ulong> aBuckets,
            ReadOnlySpan<ulong> bBuckets
        )
        {
            if (aComponentsLength != bComponentsLength || aBuckets.Length != bBuckets.Length)
                return false;

            for (int i = 0; i < aBuckets.Length; i++)
            {
                if ((aBuckets[i] ^ bBuckets[i]) != 0)
                    return false;
            }

            return true;
        }
    }
}
