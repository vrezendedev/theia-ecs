using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Reflection.Types;

namespace Theia.ECS.Components;

internal static class ComponentsMeta
{
    private const int DefaultComponentTypeMapCapacity = 16;
    private const int DefaultComponentTypeGrowthFactor = 2;

    internal static int s_count { get; private set; }

    private static readonly Dictionary<Type, int> s_componentTypeId = new();
    private static ComponentType[] s_componentTypeMap = s_componentTypeMap = new ComponentType[
        DefaultComponentTypeMapCapacity
    ];

    internal static readonly Lock s_lock = new();

    internal static int RegisterComponent<TComponent>(int sizeOfT)
        where TComponent : struct
    {
        if (s_componentTypeId.TryGetValue(typeof(TComponent), out int id))
            return id;

        lock (s_lock)
        {
            int currentLength = s_componentTypeMap.Length;

            if (s_count == currentLength)
                Array.Resize(
                    ref s_componentTypeMap,
                    currentLength * DefaultComponentTypeGrowthFactor
                );

            int index = s_count;

            Type type = typeof(TComponent);

            s_componentTypeMap[index] = new ComponentType<TComponent>(type, sizeOfT);
            s_componentTypeId[type] = index;

            s_count++;

            return index;
        }
    }

    internal static int GetComponentId(Type type)
    {
        if (!s_componentTypeId.TryGetValue(type, out int id))
            ThrowInvalidComponentType(type);

        return id;
    }

    internal static ComponentType GetComponentType(int index)
    {
        if ((uint)index >= (uint)s_count)
            ThrowComponentIndexOutOfRangeException(index);

        return s_componentTypeMap[index];
    }

    [DoesNotReturn]
    private static void ThrowComponentIndexOutOfRangeException(int index) =>
        throw new IndexOutOfRangeException(
            $"Component index '{index}' is out of range. Valid range is 0 to {s_count - 1}. Ensure the index refers to a registered component type."
        );

    [DoesNotReturn]
    private static void ThrowInvalidComponentType(Type type) =>
        throw new InvalidOperationException(
            $"Component '{type.Name}' not registered. Ensure the type refers to a registered component."
        );
}

internal static class ComponentMeta<TComponent>
    where TComponent : struct
{
    internal static readonly int s_id;

    static ComponentMeta()
    {
        if (!BlittableMeta.IsStrictlyBlittable(typeof(TComponent)))
            ThrowBlittableException();

        int fieldsCounted = typeof(TComponent)
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Length;

        if (fieldsCounted == 0)
            ThrowEmptyStructException();

        int sizeOfT = Unsafe.SizeOf<TComponent>();

        s_id = ComponentsMeta.RegisterComponent<TComponent>(sizeOfT);
    }

    [DoesNotReturn]
    private static void ThrowBlittableException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(TComponent).Name}' must be a blittable struct. Ensure it contains only blittable value types and no bools, chars, strings, or reference types."
        );

    [DoesNotReturn]
    private static void ThrowEmptyStructException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(TComponent).Name}' cannot be an empty struct. Components must contain at least one field."
        );
}
