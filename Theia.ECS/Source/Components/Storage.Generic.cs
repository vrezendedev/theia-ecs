using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Theia.ECS.Components;

internal sealed class Storage<TComponent> : Storage
    where TComponent : struct
{
    private readonly TComponent[] _values;

    internal Storage(int capacity) => _values = new TComponent[capacity];

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void WriteAllData(
        ReadOnlySpan<Storage> storages,
        int accLength,
        ReadOnlySpan<int> lengths,
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions options
    )
    {
        TComponent[] combined = ArrayPool<TComponent>.Shared.Rent(accLength);

        try
        {
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
                options
            );
        }
        finally
        {
            ArrayPool<TComponent>.Shared.Return(combined);
        }
    }
}
