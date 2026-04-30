using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Extensions;

/// <summary>
/// Extensions on <see cref="Array"/> used by the framework's hot-path resize patterns.
/// </summary>
internal static class ArrayExtensions
{
    extension(Array)
    {
        /// <summary>
        /// Grows <paramref name="arr"/> by <paramref name="growthFactor"/> when
        /// <paramref name="count"/> has reached its current capacity, and returns
        /// <see langword="true"/> if a resize occurred. When <paramref name="count"/> is below
        /// capacity the array is left untouched and <see langword="false"/> is returned.
        /// </summary>
        /// <remarks>
        /// Designed for the typical "append at index <c>count</c>, then increment <c>count</c>"
        /// pattern: callers check capacity once via this method before writing the new element,
        /// so resizes happen exactly when needed and only at the end of the dense range.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryResize<T>(ref T[] arr, int count, int growthFactor)
        {
            int currentLength = arr.Length;

            if (count >= arr.Length)
            {
                Array.Resize(ref arr, currentLength * growthFactor);
                return true;
            }

            return false;
        }
    }
}
