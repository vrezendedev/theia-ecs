using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using MessagePack;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal class Relation
{
    protected const int DefaultRelationGrowthFactor = 2;

    internal readonly Lock _lock = new();
    protected readonly Lock _updateLock = new();

    protected int _updateCount;

    protected Entity _owner;
    protected int _relatedToCount;
    protected Entity[] _relatedTo;

    internal Relation()
        : base() => _relatedTo = new Entity[1];

    protected void IncrementUpdateCount() => Interlocked.Increment(ref _updateCount);

    protected void DecrementUpdateCount() => Interlocked.Decrement(ref _updateCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity GetOwner() => _owner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetOwner(Entity owner) => _owner = owner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int Relate(Entity entity)
    {
        ThrowIfUpdating();

        int currentLength = _relatedTo.Length;

        int compositeKey = _relatedToCount;

        if (compositeKey == currentLength)
            Resize(currentLength);

        _relatedToCount++;

        _relatedTo[compositeKey] = entity;

        return compositeKey;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal EntitySwapped Disrelate(int compositeKey)
    {
        ThrowIfUpdating();

        int last = _relatedToCount - 1;

        _relatedToCount--;

        if (compositeKey < last)
        {
            Swap(last, compositeKey);
            return new EntitySwapped(_relatedTo[compositeKey]._id, compositeKey);
        }

        return EntitySwapped.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity To(int compositeKey) => _relatedTo[compositeKey];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetRelatedToCount() => _relatedToCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<Entity> To()
    {
        Entity[] entities = _relatedTo;
        return entities.AsSpan(0, _relatedToCount);
    }

    internal void Query(QueryRelation update)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            try
            {
                int count = _relatedToCount;

                if (count > 0)
                {
                    ReadOnlySpan<Entity> entities = To();

                    for (int i = 0; i < count; i++)
                        update(entities[i]);
                }
            }
            finally
            {
                DecrementUpdateCount();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Reset()
    {
        _owner = default;
        Interlocked.Exchange(ref _updateCount, 0);
        _relatedToCount = 0;
    }

    internal virtual void WriteData(
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions serializerOptions
    ) { }

    internal virtual void CopyAllData(
        byte[] rawRelations,
        MessagePackSerializerOptions deserializerOptions
    ) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Resize(int currentLength) =>
        Array.Resize(ref _relatedTo, currentLength * DefaultRelationGrowthFactor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Swap(int from, int to) => _relatedTo[to] = _relatedTo[from];

    protected void ThrowIfUpdating()
    {
        if (_updateCount > 0)
            throw new InvalidOperationException(
                "Cannot perform structural changes to a Relation while an Update is in progress. Use deferred commands to Add or Remove relations."
            );
    }
}
