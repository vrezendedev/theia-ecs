using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal abstract class RelationMany : Relation
{
    protected int _count;
    protected Entity[] _relatedTo;

    protected readonly Lock _lock = new();

    internal RelationMany() => _relatedTo = new Entity[1];

    internal int Relate(Entity entity)
    {
        lock (_lock)
        {
            int index = Account();

            _relatedTo[index] = entity;

            return index;
        }
    }

    internal EntitySwapped Disrelate(int relationIndex)
    {
        lock (_lock)
        {
            int last = _count - 1;

            _count--;

            if (relationIndex < last)
            {
                Swap(last, relationIndex);
                return new EntitySwapped(_relatedTo[relationIndex]._id, relationIndex);
            }

            return EntitySwapped.None;
        }
    }

    private int Account()
    {
        int currentLength = _relatedTo.Length;

        int index = _count;

        if (index == currentLength)
            Resize(currentLength);

        _count++;

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Resize(int currentLength) =>
        Array.Resize(ref _relatedTo, currentLength * DefaultRelationGrowthFactor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Swap(int from, int to) => _relatedTo[to] = _relatedTo[from];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset() => _count = 0;
}
