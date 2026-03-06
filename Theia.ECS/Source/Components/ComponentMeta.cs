using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection.Types;

namespace Theia.ECS.Components;

internal static class ComponentMeta
{
    internal static int s_count { get; set; }
    internal static readonly object s_lock;

    static ComponentMeta() => s_lock = new object();
}

internal static class ComponentMeta<T>
    where T : struct
{
    internal static readonly int s_id;
    internal static readonly int s_size;

    static ComponentMeta()
    {
        if (!BlittableMeta.IsStrictlyBlittable(typeof(T)))
            ThrowInvalidOperationException();

        s_size = Unsafe.SizeOf<T>();

        lock (ComponentMeta.s_lock)
        {
            s_id = ComponentMeta.s_count;
            ComponentMeta.s_count++;
        }
    }

    [DoesNotReturn]
    private static void ThrowInvalidOperationException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(T).Name}' must be a blittable struct. Ensure it contains only blittable value types and no bools, chars, strings, or reference types."
        );
}
