using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal sealed class EvaluatedRelation<TRelation> : Relation
    where TRelation : struct
{
    private TRelation[] _data;

    internal EvaluatedRelation()
        : base() => _data = new TRelation[1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<TRelation> Get()
    {
        TRelation[] data = _data;
        return data.AsSpan(0, _relatedToCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TRelation Get(int index) => ref _data[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, in TRelation data) => _data[index] = data;

    internal void Query(QueryRelation<TRelation> update)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            int count = _relatedToCount;

            if (count > 0)
            {
                ReadOnlySpan<Entity> entities = To();
                Span<TRelation> relations = Get();

                for (int i = 0; i < count; i++)
                    update(entities[i], ref relations[i]);
            }

            DecrementUpdateCount();
        }
    }

    protected override void Resize(int currentLength)
    {
        base.Resize(currentLength);
        Array.Resize(ref _data, currentLength * DefaultRelationGrowthFactor);
    }

    protected override void Swap(int from, int to)
    {
        base.Swap(from, to);
        _data[to] = _data[from];
    }
}
