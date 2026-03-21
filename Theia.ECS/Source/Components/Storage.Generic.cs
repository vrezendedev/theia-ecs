using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Components;

internal sealed class Storage<TComponent> : Storage
    where TComponent : struct
{
    private readonly TComponent[] _values;

    internal Storage(int capacity) => _values = new TComponent[capacity];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<TComponent> GetValues(int length) => _values.AsSpan(0, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TComponent Get(int index) => ref _values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, TComponent value) => _values[index] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Move(int from, int to) => _values[to] = _values[from];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Transfer(int oldIndex, int newIndex, Storage to)
    {
        Storage<TComponent> target = (Storage<TComponent>)to;
        target.Set(newIndex, _values[oldIndex]);
    }
}
