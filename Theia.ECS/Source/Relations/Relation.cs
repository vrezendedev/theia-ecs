using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using MessagePack;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

/// <summary>
/// <b>Owner-side container for one relation type held by one entity</b>: the entity's list of targets
/// under that relation. Tracks the owner, the dense list of related entities and, in the
/// <see cref="EvaluatedRelation{TRelation}"/> derivative, the per-link payload aligned to the
/// targets array.
/// </summary>
/// <remarks>
/// <para>
/// Each entity participating as an owner under a given relation type holds exactly one
/// <see cref="Relation"/> instance, owned by the world's <see cref="RelationStorage"/> for that
/// type. The bilateral counterpart is maintained by <see cref="RelationLink"/> on the target side;
/// both sides are updated atomically when a link is added or removed.
/// </para>
/// <para>
/// <see cref="Query"/> mutates the targets array under <see cref="_updateLock"/> and increments
/// <see cref="_updateCount"/> for the duration of the iteration; structural changes (Relate /
/// Disrelate) check the counter and throw if a query is in flight, mirroring the world-level
/// query/structural-change interlock.
/// </para>
/// <para>
/// Instances are pooled by <see cref="RelationType"/>; <see cref="Reset"/> returns the structure
/// to its empty state without releasing the underlying arrays.
/// </para>
/// </remarks>
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

    /// <summary>Returns the owner entity this relation belongs to.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity GetOwner() => _owner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetOwner(Entity owner) => _owner = owner;

    /// <summary>
    /// Adds <paramref name="entity"/> to the targets array and returns its composite key which is the
    /// dense index that <see cref="RelationLink"/> uses to locate this link in the owner-side
    /// storage.
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if a query is currently iterating over this relation.</exception>
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

    /// <summary>
    /// Removes the target at <paramref name="compositeKey"/> in O(1) by swapping the last entry
    /// into the freed slot. Returns the swapped entity's ID and new index so the caller can
    /// patch the swapped target's <see cref="RelationLink"/> back-reference; returns
    /// <see cref="EntitySwapped.None"/> when the removed entry was already last and no swap occurred.
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if a query is currently iterating over this relation.</exception>
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

    /// <summary>Returns the number of targets currently held.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetRelatedToCount() => _relatedToCount;

    /// <summary>Returns a span over the populated portion of the targets array.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<Entity> To()
    {
        Entity[] entities = _relatedTo;
        return entities.AsSpan(0, _relatedToCount);
    }

    /// <summary>
    /// Iterates every target under <paramref name="query"/> while holding <see cref="_updateLock"/>
    /// and incrementing <see cref="_updateCount"/>. <b>Structural changes to this relation throw
    /// for the duration</b>; mutations to query state happen on the callback's own copy.
    /// </summary>
    internal void Query<TQueryRelation>(ref TQueryRelation query)
        where TQueryRelation : struct, IQueryRelation, allows ref struct
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            int count = _relatedToCount;

            if (count > 0)
            {
                ReadOnlySpan<Entity> entities = To();

                for (int i = 0; i < count; i++)
                    query.Execute(entities[i]);
            }

            DecrementUpdateCount();
        }
    }

    /// <summary>Returns the structure to its empty state, ready for reuse from the pool.</summary>
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

    /// <summary>Throws <see cref="InvalidOperationException"/> if a query is currently iterating; structural changes during query iteration would invalidate the iterator's view.</summary>
    protected void ThrowIfUpdating()
    {
        if (_updateCount > 0)
            throw new InvalidOperationException(
                "Cannot perform structural changes to a Relation while an Update is in progress. Use deferred commands to Add or Remove relations."
            );
    }
}
