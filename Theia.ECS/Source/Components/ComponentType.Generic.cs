using System;

namespace Theia.ECS.Components;

/// <summary>
/// Generic concrete implementation of <see cref="ComponentType"/> for a specific
/// <typeparamref name="TComponent"/>. Provides the type-parameterized factory methods that
/// produce strongly-typed <see cref="Storage{T}"/> and <see cref="Unique{T}"/> instances,
/// which the non-generic base cannot construct on its own.
/// </summary>
internal sealed class ComponentType<TComponent> : ComponentType
    where TComponent : struct
{
    /// <summary>
    /// Parameterless constructor used by the reflection-based registration path; see
    /// <see cref="ComponentType()"/> on the base class.
    /// </summary>
    public ComponentType()
        : base() { }

    internal ComponentType(Type type, int sizeOf)
        : base(type, sizeOf) { }

    internal override Storage CreateStorage(int capacity) => new Storage<TComponent>(capacity);

    internal override Unique CreateUnique() => new Unique<TComponent>();
}
