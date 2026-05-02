using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using MessagePack;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

/// <summary>
/// Generic <see cref="Relation"/> derivative that carries per-link payload data alongside the
/// targets array. Used for data relations; tag relations use <see cref="Relation"/> directly.
/// </summary>
/// <remarks>
/// The <typeparamref name="TRelation"/> array is kept aligned with the base class's targets
/// array: <see cref="Relation.Resize"/> grows both, <see cref="Relation.Swap"/> moves both,
/// so a target's composite key indexes the same row in both arrays. This means the data array's
/// row layout matches the targets', and <b>iteration over both is cache-coherent</b>.
/// </remarks>
internal sealed class EvaluatedRelation<TRelation> : Relation
    where TRelation : struct
{
    private TRelation[] _data;

    internal EvaluatedRelation()
        : base() => _data = new TRelation[1];

    /// <summary>Returns a span over the populated portion of the per-link data array, aligned with <see cref="Relation.To"/>.</summary>
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

    /// <summary>
    /// Iterates every (target, payload) pair under <paramref name="query"/>. Same locking
    /// semantics as <see cref="Relation.Query"/>; <b>payload mutations through the <c>ref</c>
    /// parameter persist in the data array</b>.
    /// </summary>
    internal void QueryEvaluated<TQueryRelation>(ref TQueryRelation query)
        where TQueryRelation : struct, IQueryEvaluatedRelation<TRelation>, allows ref struct
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
                    query.Execute(entities[i], ref relations[i]);
            }

            DecrementUpdateCount();
        }
    }

    internal override void WriteData(
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions serializerOptions
    ) =>
        MessagePackSerializer.Serialize(
            arrayBufferWriter,
            _data.AsMemory(0, _relatedToCount),
            serializerOptions
        );

    internal override void CopyAllData(
        byte[] rawRelations,
        MessagePackSerializerOptions deserializerOptions
    ) =>
        MessagePackSerializer
            .Deserialize<TRelation[]>(rawRelations, deserializerOptions)
            .AsSpan()
            .CopyTo(_data.AsSpan(0, _relatedToCount));

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
