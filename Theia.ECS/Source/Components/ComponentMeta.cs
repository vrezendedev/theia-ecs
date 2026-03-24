using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection;
using Theia.ECS.Reflection.Types;

namespace Theia.ECS.Components;

internal static class ComponentsMeta
{
    private static TypeRegistry<ComponentType> s_componentRegistry = new();

    internal static int RegisterComponent<TComponent>(int sizeOfT)
        where TComponent : struct
    {
        int componentId = s_componentRegistry.Account();
        ComponentType<TComponent> componentType = new ComponentType<TComponent>(
            typeof(TComponent),
            sizeOfT
        );
        s_componentRegistry.Set(componentId, componentType);
        return componentId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetComponentId(Type type) => s_componentRegistry.GetTypeId(type);

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

        int fieldsCounted = typeof(TComponent).GetFields(BlittableMeta.Flags).Length;

        if (fieldsCounted == 0)
            ThrowEmptyStructException();

        int sizeOfT = Unsafe.SizeOf<TComponent>();

        s_id = ComponentsMeta.RegisterComponent<TComponent>(sizeOfT);
    }

    [DoesNotReturn]
    private static void ThrowEmptyStructException() =>
        throw new InvalidOperationException(
            $"Component '{typeof(TComponent).Name}' cannot be an empty struct. Components must contain at least one field."
        );
}
