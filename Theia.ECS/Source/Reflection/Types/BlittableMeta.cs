using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;
using Theia.ECS.Blittables;
using Theia.ECS.Blittables.Attributes;
using Theia.ECS.Entities;

namespace Theia.ECS.Reflection.Types;

internal static class BlittableMeta
{
    private const BindingFlags Flags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly ConcurrentDictionary<Type, bool> s_cachedBlittables;

    internal static int _totalBlittablesCached => s_cachedBlittables.Count;

    static BlittableMeta()
    {
        s_cachedBlittables = new() { };
        s_cachedBlittables[typeof(decimal)] = false;
        s_cachedBlittables[typeof(DateTime)] = false;
        s_cachedBlittables[typeof(TimeSpan)] = true;
        s_cachedBlittables[typeof(BlittableChar)] = true;
        s_cachedBlittables[typeof(BlittableBoolean)] = true;
        s_cachedBlittables[typeof(Entity)] = true;
        s_cachedBlittables[typeof(EntityMeta)] = true;
    }

    internal static bool IsStrictlyBlittable(Type type)
    {
        if (type.IsPrimitive)
            return type != typeof(bool) && type != typeof(char);

        if (type.IsEnum)
            return true;

        if (
            !type.IsValueType
            || type.IsGenericTypeDefinition
            || type.IsPointer
            || type.IsByRef
            || type.IsByRefLike
        )
            return false;

        if (s_cachedBlittables.TryGetValue(type, out bool cached))
            return cached;
        else if (Attribute.IsDefined(type, typeof(AssumeBlittable)))
        {
            s_cachedBlittables[type] = true;
            return true;
        }

        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return false;

            foreach (Type generic in type.GetGenericArguments())
            {
                if (!IsStrictlyBlittable(generic))
                {
                    s_cachedBlittables[type] = false;
                    return false;
                }
            }
        }

        if (type.StructLayoutAttribute?.Value is LayoutKind.Auto)
        {
            s_cachedBlittables[type] = false;
            return false;
        }

        foreach (FieldInfo field in type.GetFields(Flags))
        {
            if (!IsStrictlyBlittable(field.FieldType))
            {
                s_cachedBlittables[type] = false;
                return false;
            }
        }

        s_cachedBlittables[type] = true;
        return true;
    }
}
