using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Archetypes;

internal sealed class Storage<T> : Storage
    where T : struct
{
    private T[] _values;

    internal Storage(int capacity) => _values = new T[capacity];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<T> Values(int length) => _values.AsSpan(0, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref T Get(int index) => ref _values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, T value) => _values[index] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Move(int from, int to) => _values[to] = _values[from];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Transfer(int oldIndex, int newIndex, Storage to)
    {
        Storage<T> target = (Storage<T>)to;
        target.Set(newIndex, _values[oldIndex]);
    }
}
