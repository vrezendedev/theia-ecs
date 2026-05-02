using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.ECS.Components;

/// <summary>
/// Generic concrete <see cref="ComponentDeferredStorage"/> for a specific
/// <typeparamref name="TComponent"/>. Holds the typed FIFO queue and implements the polymorphic
/// flush hooks the world calls without knowing the concrete component type.
/// </summary>
internal sealed class ComponentDeferredStorage<TComponent> : ComponentDeferredStorage
    where TComponent : struct
{
    private readonly Queue<TComponent> _values;

    internal ComponentDeferredStorage(int capacity) => _values = new Queue<TComponent>(capacity);

    /// <summary>Enqueues <paramref name="component"/> for application by the next flush of the matching command queue.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void EnqueueDeferred(TComponent component) => _values.Enqueue(component);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void SetWithNext(in EntityMeta entityMeta, Archetype to) =>
        to.Set(in entityMeta, _values.Dequeue());

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void DiscardNext() => _values.Dequeue();
}
