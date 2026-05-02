using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Relations;

/// <summary>
/// Generic concrete <see cref="RelationDeferredStorage"/> for a specific
/// <typeparamref name="TRelation"/>. Holds a dense values array with a free-list of unused slots,
/// so deferred relation payloads can be staged at queue time and applied at flush time without
/// FIFO ordering between unrelated commands.
/// </summary>
/// <remarks>
/// Unlike the component deferred storage, this is keyed by an explicit <c>storageIndex</c>
/// returned at queue time and consumed at flush time, not by FIFO position. This lets relation
/// records carry their staging slot in the matching <see cref="Contracts.AddRelationDeferred"/>.
/// </remarks>
internal sealed class RelationDeferredStorage<TRelation> : RelationDeferredStorage
    where TRelation : struct
{
    private const int DefaultRelationsGrowthFactor = 2;

    private TRelation[] _values;
    private Queue<int> _free;

    internal RelationDeferredStorage(int capacity)
    {
        _values = new TRelation[capacity];
        _free = new(capacity);

        for (int i = 0; i < capacity; i++)
            _free.Enqueue(i);
    }

    /// <summary>
    /// Stages <paramref name="relation"/> in the next available slot and returns the slot index.
    /// The caller carries this index in the deferred-command record so the value can be applied
    /// at flush time via <see cref="SetWith"/>. Grows the values array when the free list is empty.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int AccountDeferred(TRelation relation)
    {
        int index;

        if (_free.Count > 0)
            index = _free.Dequeue();
        else
        {
            index = _values.Length;

            int targetLength = index * DefaultRelationsGrowthFactor;

            Array.Resize(ref _values, targetLength);

            for (int i = index + 1; i < targetLength; i++)
                _free.Enqueue(i);
        }

        _values[index] = relation;

        return index;
    }

    /// <inheritdoc/>
    internal override void SetWith(int storageIndex, Relation relation, int compositeKey)
    {
        EvaluatedRelation<TRelation> evaluatedRelation = (EvaluatedRelation<TRelation>)relation;
        evaluatedRelation.Set(compositeKey, _values[storageIndex]);
        _free.Enqueue(storageIndex);
    }
}
