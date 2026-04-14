using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using MessagePack;
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
            try
            {
                int count = _relatedToCount;

                if (count > 0)
                {
                    ReadOnlySpan<Entity> entities = To();
                    Span<TRelation> relations = Get();

                    for (int i = 0; i < count; i++)
                        update(entities[i], ref relations[i]);
                }
            }
            finally
            {
                DecrementUpdateCount();
            }
        }
    }

    internal override void WriteData(
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions options
    ) =>
        MessagePackSerializer.Serialize(
            arrayBufferWriter,
            _data.AsMemory(0, _relatedToCount),
            options
        );

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
