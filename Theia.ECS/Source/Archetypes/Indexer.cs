using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Archetypes;

/// <summary>
/// Per-chunk entity registry within an <see cref="Archetype"/>. Holds the dense list of entities
/// occupying this chunk's storage rows, and provides O(1) add and swap-remove against that list.
/// </summary>
/// <remarks>
/// <para>
/// An <see cref="Indexer"/> is paired with one row per component type in the archetype's
/// <see cref="Components.Storage">Storage</see> arrays; together they form a chunk. The indexer owns the entity slots
/// for that chunk, while each <see cref="Components.Storage">Storage</see> owns the matching component data slots.
/// Removing an entity at index <c>i</c> swaps in the last entity, returning its old index so
/// the archetype can patch the component storages.
/// </para>
/// </remarks>
internal sealed class Indexer
{
    private int _count;
    private Entity[] _entities;

    internal Indexer(int capacity) => _entities = new Entity[capacity];

    /// <summary>Returns the number of entities currently held in this chunk.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Count() => _count;

    /// <summary>Returns <see langword="true"/> when the chunk <b>has reached its capacity and cannot accept further entities</b></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsFull() => _entities.Length == _count;

    /// <summary>Returns a span over the entities currently held, suitable for iteration without exposing trailing capacity.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Entity> GetValues() => _entities.AsSpan(0, _count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref Entity Get(int index) => ref _entities[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, Entity entity) => _entities[index] = entity;

    /// <summary>
    /// Reserves the next slot and returns its index, advancing <see cref="Count"/>. The caller
    /// is responsible for filling the slot via <see cref="Set"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Add()
    {
        int index = _count;

        _count++;

        return index;
    }

    /// <summary>
    /// Removes the entity at <paramref name="index"/> <b>by swapping the last entity into its slot</b>
    /// and decrementing <see cref="Count"/>. Returns the old index of the swapped entity, or
    /// <c>-1</c> when the removed entity was already last and no swap occurred.
    /// </summary>
    /// <remarks>
    /// The returned old-index lets the archetype move the swapped entity's component data in
    /// the paired <see cref="Components.Storage">Storage</see> arrays so chunk-internal indices stay consistent.
    /// </remarks>
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

    /// <summary>
    /// Adds the entity at <paramref name="index"/> to <paramref name="target"/> and returns the
    /// destination index. Used during cross-archetype moves where component data is transferred
    /// with the entity slot.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Transfer(int index, Indexer target)
    {
        int indexTo = target.Add();
        target.Set(indexTo, _entities[index]);
        return indexTo;
    }
}
