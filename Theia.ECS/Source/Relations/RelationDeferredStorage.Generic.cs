using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Relations;

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

    internal override void SetWith(int storageIndex, Relation relation, int compositeKey)
    {
        EvaluatedRelation<TRelation> evaluatedRelation = (EvaluatedRelation<TRelation>)relation;
        evaluatedRelation.Set(compositeKey, _values[storageIndex]);
        _free.Enqueue(storageIndex);
    }
}
