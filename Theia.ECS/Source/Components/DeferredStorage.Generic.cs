using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.ECS.Components;

internal sealed class DeferredStorage<T> : DeferredStorage
    where T : struct
{
    private readonly Queue<T> _values;

    internal DeferredStorage(int capacity) => _values = new Queue<T>(capacity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void EnqueueDeferred(T component) => _values.Enqueue(component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void SetWithNext(in EntityMeta entityMeta, Archetype to) =>
        to.Set(in entityMeta, _values.Dequeue());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void DiscardNext() => _values.Dequeue();
}
