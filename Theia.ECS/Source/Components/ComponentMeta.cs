using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection.Types;

namespace Theia.ECS.Components;

internal static class ComponentsMeta
{
    private const int DefaultComponentTypeMapCapacity = 16;
    private const int DefaultComponentTypeGrowthFactor = 2;

    internal static int s_count { get; set; }
    private static ComponentType[] s_componentTypeMap = s_componentTypeMap = new ComponentType[
        DefaultComponentTypeMapCapacity
    ];
    internal static readonly object s_lock = new object();

    internal static int RegisterComponent<T>(int sizeOfT)
        where T : struct
    {
        lock (s_lock)
        {
            int currentLength = s_componentTypeMap.Length;

            if (s_count == currentLength)
                Array.Resize(
                    ref s_componentTypeMap,
                    currentLength * DefaultComponentTypeGrowthFactor
                );

            int index = s_count;

            s_componentTypeMap[index] = new ComponentType<T>(typeof(T), sizeOfT);

            s_count++;

            return index;
        }
    }

    internal static ComponentType GetComponentType(int index)
    {
        if ((uint)index >= (uint)s_count)
            ThrowIndexOutOfRangeException(index);

        return s_componentTypeMap[index];
    }

    /// <summary>
    /// Used exclusively for array resizing tests.
    /// </summary>
    /// <returns>The current length of <see cref="s_componentTypeMap"/>.</returns>
    internal static int GetComponentTypeMapCapacity() => s_componentTypeMap.Length;

    [DoesNotReturn]
    private static void ThrowIndexOutOfRangeException(int index) =>
        throw new IndexOutOfRangeException(
            $"Component index '{index}' is out of range. Valid range is 0 to {s_count - 1}. Ensure the index refers to a registered component type."
        );
}

internal static class ComponentMeta<T>
    where T : struct
{
    internal static readonly int s_id;

    static ComponentMeta()
    {
        if (!BlittableMeta.IsStrictlyBlittable(typeof(T)))
            ThrowInvalidOperationBlittableException();

        int fieldsCounted = typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Length;

        if (fieldsCounted == 0)
            ThrowInvalidOperationEmptyStructException();

        int sizeOfT = Unsafe.SizeOf<T>();

        s_id = ComponentsMeta.RegisterComponent<T>(sizeOfT);
    }

    [DoesNotReturn]
    private static void ThrowInvalidOperationBlittableException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(T).Name}' must be a blittable struct. Ensure it contains only blittable value types and no bools, chars, strings, or reference types."
        );

    [DoesNotReturn]
    private static void ThrowInvalidOperationEmptyStructException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(T).Name}' cannot be an empty struct. Components must contain at least one field."
        );
}
