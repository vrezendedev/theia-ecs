using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.ECS.Components;

internal sealed class ComponentDeferredStorage<TComponent> : ComponentDeferredStorage
    where TComponent : struct
{
    private readonly Queue<TComponent> _values;

    internal ComponentDeferredStorage(int capacity) => _values = new Queue<TComponent>(capacity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void EnqueueDeferred(TComponent component) => _values.Enqueue(component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void SetWithNext(in EntityMeta entityMeta, Archetype to) =>
        to.Set(in entityMeta, _values.Dequeue());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void DiscardNext() => _values.Dequeue();
}
