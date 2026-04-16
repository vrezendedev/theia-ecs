using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Theia.ECS.Reflection;
using Theia.ECS.Reflection.Types;

namespace Theia.ECS.Components;

internal static class ComponentsMeta
{
    private static TypeRegistry<ComponentType> s_componentRegistry = new();

    internal static int AttemptRegisterComponent<TComponent>(int sizeOfT)
        where TComponent : struct
    {
        if (s_componentRegistry.TryGetTypeId(typeof(TComponent), out int componentId))
            return componentId;

        componentId = s_componentRegistry.Account();

        ComponentType<TComponent> componentType = new ComponentType<TComponent>(
            typeof(TComponent),
            sizeOfT
        );

        s_componentRegistry.Set(componentId, componentType);

        return componentId;
    }

    internal static int AttemptRegisterComponent(string name)
    {
        if (!s_componentRegistry.TryGetTypeId(name, out int componentId))
        {
            Type? type = Type.GetType(name);

            if (type is null)
                TypeRegistry<ComponentType>.ThrowTypeLoadException(name);

            componentId = s_componentRegistry.Account();

            Type genericType = typeof(ComponentType<>).MakeGenericType([type]);

            ComponentType componentType = (ComponentType)Activator.CreateInstance(genericType)!;

            componentType.Initialize(type, Marshal.SizeOf(type));

            s_componentRegistry.Set(componentId, componentType);
        }

        return componentId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetComponentId(Type type) => s_componentRegistry.GetTypeId(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetComponentId(string typeName) => s_componentRegistry.GetTypeId(typeName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ComponentType GetComponentType(int index) =>
        s_componentRegistry.GetTypeMeta(index);

    internal static int Count() => s_componentRegistry.Count();

    internal static bool ContainsFields<TStruct>()
        where TStruct : struct => typeof(TStruct).GetFields(BlittableMeta.Flags).Length > 0;
}

internal static class ComponentMeta<TComponent>
    where TComponent : struct
{
    internal static readonly int s_id;

    static ComponentMeta()
    {
        if (!BlittableMeta.IsStrictlyBlittable(typeof(TComponent)))
            BlittableMeta.ThrowBlittableException<TComponent>();

        if (typeof(TComponent).IsPrimitive || typeof(TComponent).IsEnum)
            ThrowComponentNotAStructException();

        int fieldsCounted = typeof(TComponent).GetFields(BlittableMeta.Flags).Length;

        if (fieldsCounted == 0)
            ThrowEmptyStructException();

        int sizeOfT = Unsafe.SizeOf<TComponent>();

        s_id = ComponentsMeta.AttemptRegisterComponent<TComponent>(sizeOfT);
    }

    [DoesNotReturn]
    private static void ThrowEmptyStructException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(TComponent).Name}' cannot be an empty struct. Components must contain at least one field."
        );

    [DoesNotReturn]
    private static void ThrowComponentNotAStructException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(TComponent).Name}' must be a struct."
        );
}
