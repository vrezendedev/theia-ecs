using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Archetypes;

internal sealed class Indexer
{
    internal readonly int _indexerId;
    private int _count;
    private Entity[] _entities;

    internal Indexer(int indexerId, int capacity)
    {
        _indexerId = indexerId;
        _entities = new Entity[capacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Count() => _count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsFull() => _entities.Length == _count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Entity> GetValues() => _entities.AsSpan(0, _count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref Entity Get(int index) => ref _entities[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, Entity entity) => _entities[index] = entity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Add()
    {
        int index = _count;

        _count++;

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Remove(int index)
    {
        int swapped = -1;
        int last = _count - 1;

        if (index < last)
        {
            _entities[index] = _entities[last];
            swapped = last;
        }

        _count--;

        return swapped;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Transfer(int index, Indexer target)
    {
        int indexTo = target.Add();
        target.Set(indexTo, _entities[index]);
        return indexTo;
    }
}
