using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection;
using Theia.ECS.Reflection.Types;

namespace Theia.ECS.Components;

/// <summary>
/// Central registry that assigns and resolves the integer IDs used to identify component types.
/// Maintains the mapping between a <see cref="Type"/> and its
/// associated <see cref="ComponentType"/> metadata.
/// </summary>
/// <remarks>
/// Component IDs are <b>stable within a process but are not guaranteed to be stable across runs</b>.
/// </remarks>
internal static class ComponentsMeta
{
    private static TypeRegistry<ComponentType> s_componentRegistry = new();

    /// <summary>
    /// Returns the component ID for <typeparamref name="TComponent"/>, registering it on first
    /// encounter. Subsequent calls for the same type return the previously assigned ID.
    /// </summary>
    /// <param name="sizeOfT">The size in bytes of <typeparamref name="TComponent"/>, supplied by
    /// the caller to avoid an extra <see cref="Unsafe.SizeOf{T}"/> call when the value is already known.</param>
    /// <returns>The integer ID associated with <typeparamref name="TComponent"/>.</returns>
    /// <remarks>
    /// The fast path resolves an already-registered type without entering the registry's lock;
    /// only first-time registrations pay for synchronization. Atomicity of the registration
    /// itself, including the duplicate-registration check, is handled inside
    /// <see cref="TypeRegistry{T}.Register(in T)"/>.
    /// </remarks>
    internal static int AttemptRegisterComponent<TComponent>(int sizeOfT)
        where TComponent : struct
    {
        if (s_componentRegistry.TryGetTypeId(typeof(TComponent), out int componentId))
            return componentId;

        return s_componentRegistry.Register(
            new ComponentType<TComponent>(typeof(TComponent), sizeOfT)
        );
    }

    /// <summary>
    /// Resolves a component type by its assembly-qualified name and registers it if not already
    /// present. <b>Used during deserialization to restore components that were persisted by name</b>.
    /// </summary>
    /// <param name="name">The assembly-qualified type name, as accepted by <see cref="Type.GetType(string)"/>.</param>
    /// <returns>The integer ID associated with the resolved component type.</returns>
    /// <remarks>
    /// Because the concrete type is not known at compile time,
    /// this overload uses reflection to construct the matching <c>ComponentType&lt;T&gt;</c>
    /// and to compute its unmanaged size. As such, it is significantly more expensive than the
    /// generic overload and <b>is intended for cold paths such as save/load</b>, not for hot loops.
    /// </remarks>
    /// <exception cref="TypeLoadException">Thrown when <paramref name="name"/> cannot be resolved to a loaded type.</exception>
    internal static int AttemptRegisterComponent(string name)
    {
        if (s_componentRegistry.TryGetTypeId(name, out int componentId))
            return componentId;

        Type? type = Type.GetType(name);

        if (type is null)
            TypeRegistry<ComponentType>.ThrowTypeLoadException(name);

        Type genericType = typeof(ComponentType<>).MakeGenericType([type]);
        ComponentType componentType = (ComponentType)Activator.CreateInstance(genericType)!;

        int size = (int)
            typeof(Unsafe)
                .GetMethod(nameof(Unsafe.SizeOf))!
                .MakeGenericMethod(type)
                .Invoke(null, null)!;

        componentType.Initialize(type, size);

        return s_componentRegistry.Register(componentType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetComponentId(Type type) => s_componentRegistry.GetTypeId(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetComponentId(string typeName) => s_componentRegistry.GetTypeId(typeName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ComponentType GetComponentType(int componentId) =>
        s_componentRegistry.GetTypeMeta(componentId);

    /// <summary>Returns the number of components currently registered.</summary>
    internal static int Count() => s_componentRegistry.Count();

    /// <summary>
    /// Returns <see langword="true"/> if <typeparamref name="TStruct"/> declares at least one
    /// field visible under <see cref="BlittableMeta.Flags"/>.
    /// </summary>
    internal static bool ContainsFields<TStruct>()
        where TStruct : struct => typeof(TStruct).GetFields(BlittableMeta.Flags).Length > 0;
}

/// <summary>
/// Per-type cache that <b>lazily registers</b> <typeparamref name="TComponent"/> with the global
/// <see cref="ComponentsMeta"/> registry on first access and exposes its assigned ID.
/// </summary>
/// <remarks>
/// <para>
/// The static constructor enforces that <typeparamref name="TComponent"/> is a valid component.
/// <br/>
/// Violations throw at the moment the type is first touched.
/// </para>
/// <para>
/// Because each closed generic instantiation has its own static state, <see cref="s_id"/> acts
/// as an effectively free lookup after the first access; no dictionary hit, just a static field read.
/// </para>
/// </remarks>
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
