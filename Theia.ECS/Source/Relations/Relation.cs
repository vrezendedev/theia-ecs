using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal abstract class Relation
{
    internal readonly Lock _relationLock = new();
    protected readonly Lock _updateLock = new();

    protected Entity _owner;
    protected int _updateCount;

    protected void IncrementUpdateCount() => Interlocked.Increment(ref _updateCount);

    protected void DecrementUpdateCount() => Interlocked.Decrement(ref _updateCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity GetOwner() => _owner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetOwner(Entity owner) => _owner = owner;

    internal virtual void Reset()
    {
        _owner = default;
        _updateCount = 0;
    }

    internal abstract void Update(UpdateRelation update);

    protected void ThrowIfUpdating()
    {
        if (_updateCount > 0)
            throw new InvalidOperationException(
                "Cannot perform structural changes to a Relation while an Update is in progress. Use deferred commands to Add or Remove relations."
            );
    }
}

internal class TagRelation : Relation
{
    protected const int DefaultRelationGrowthFactor = 2;

    protected int _count;
    protected Entity[] _relatedTo;

    internal TagRelation()
        : base() => _relatedTo = new Entity[1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Relate(Entity entity)
    {
        ThrowIfUpdating();

        int compositeKey = Account();

        _relatedTo[compositeKey] = entity;

        return compositeKey;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal EntitySwapped Disrelate(int compositeKey)
    {
        ThrowIfUpdating();

        int last = _count - 1;

        _count--;

        if (compositeKey < last)
        {
            Swap(last, compositeKey);
            return new EntitySwapped(_relatedTo[compositeKey]._id, compositeKey);
        }

        return EntitySwapped.None;
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
    internal Entity To(int compositeKey) => _relatedTo[compositeKey];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<Entity> To()
    {
        Entity[] entities = _relatedTo;
        return entities.AsSpan(0, _count);
    }

    internal override void Update(UpdateRelation update)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            int count = _count;

            if (count > 0)
            {
                ReadOnlySpan<Entity> entities = To();

                for (int i = 0; i < count; i++)
                    update(entities[i]);
            }

            DecrementUpdateCount();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Resize(int currentLength) =>
        Array.Resize(ref _relatedTo, currentLength * DefaultRelationGrowthFactor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Swap(int from, int to) => _relatedTo[to] = _relatedTo[from];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
        base.Reset();
        _count = 0;
    }
}

internal sealed class EvaluatedRelation<TRelation> : TagRelation
    where TRelation : struct
{
    private TRelation[] _data;

    internal EvaluatedRelation()
        : base() => _data = new TRelation[1];

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<TRelation> Get()
    {
        TRelation[] data = _data;
        return data.AsSpan(0, _count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TRelation Get(int index) => ref _data[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, in TRelation data) => _data[index] = data;

    internal void Update(UpdateRelation<TRelation> update)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            int count = _count;

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
}
