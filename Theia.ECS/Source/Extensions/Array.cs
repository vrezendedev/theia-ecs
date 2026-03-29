using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Extensions;

internal static class ArrayExtensions
{
    extension(Array)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AttemptResize<T>(ref T[] arr, int count, int growthFactor)
        {
            int currentLength = arr.Length;

            if (count == arr.Length)
                Array.Resize(ref arr, currentLength * growthFactor);
        }
    }
}
