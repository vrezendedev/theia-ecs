using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Theia.ECS.Components;

/// <summary>
/// Generic concrete <see cref="Storage"/> for a single component type within one chunk. Holds
/// the typed array that backs the chunk's slot for <typeparamref name="TComponent"/>, and
/// implements the move/transfer/serialize hooks the archetype calls polymorphically.
/// </summary>
internal sealed class Storage<TComponent> : Storage
    where TComponent : struct
{
    private readonly TComponent[] _values;

    internal Storage(int capacity) => _values = new TComponent[capacity];

    /// <summary>
    /// Returns a span over the populated portion of the values array, sized to
    /// <paramref name="length"/> as supplied by the chunk's <see cref="Archetypes.Indexer.Count">Indexer</see>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<TComponent> GetValues(int length) => _values.AsSpan(0, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TComponent Get(int index) => ref _values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, TComponent value) => _values[index] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Move(int from, int to) => _values[to] = _values[from];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Transfer(int oldIndex, int newIndex, Storage to)
    {
        Storage<TComponent> target = (Storage<TComponent>)to;
        target.Set(newIndex, _values[oldIndex]);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Rents a temporary <typeparamref name="TComponent"/> array from <see cref="ArrayPool{T}"/>
    /// sized to <paramref name="accLength"/>, copies each chunk's populated portion into it
    /// contiguously, and serializes the result. The pooled array is returned before exit.
    /// </remarks>
    internal override void WriteAllData(
        ReadOnlySpan<Storage> storages,
        int accLength,
        ReadOnlySpan<int> lengths,
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions serializerOptions
    )
    {
        TComponent[] combined = ArrayPool<TComponent>.Shared.Rent(accLength);

        int offset = 0;

        for (int i = 0; i < storages.Length; i++)
        {
            Storage<TComponent> storage = (Storage<TComponent>)storages[i];
            int length = lengths[i];

            if (length > 0)
            {
                storage.GetValues(length).CopyTo(combined.AsSpan(offset));
                offset += length;
            }
        }

        MessagePackSerializer.Serialize(
            arrayBufferWriter,
            combined.AsMemory(0, accLength),
            serializerOptions
        );

        ArrayPool<TComponent>.Shared.Return(combined);
    }

    internal override void CopyAllData(
        ReadOnlySpan<Storage> storages,
        byte[] rawComponents,
        int capacity,
        MessagePackSerializerOptions deserializerOptions
    )
    {
        TComponent[] components = MessagePackSerializer.Deserialize<TComponent[]>(
            rawComponents,
            deserializerOptions
        );

        int offset = 0;

        for (int i = 0; i < storages.Length; i++)
        {
            Storage<TComponent> storage = (Storage<TComponent>)storages[i];

            int available = Math.Min(capacity, components.Length - offset);

            components.AsSpan(offset, available).CopyTo(storage.GetValues(capacity));

            offset += capacity;
        }
    }
}
