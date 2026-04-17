using System;
using System.Buffers;
using MessagePack;

namespace Theia.ECS.Components;

internal abstract class Storage
{
    internal abstract void Move(int from, int to);

    internal abstract void Transfer(int oldIndex, int newIndex, Storage to);

    internal abstract void WriteAllData(
        ReadOnlySpan<Storage> storages,
        int accLength,
        ReadOnlySpan<int> lengths,
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions serializerOptions
    );

    internal abstract void CopyAllData(
        ReadOnlySpan<Storage> storages,
        byte[] rawComponents,
        int perStorageCapacity,
        MessagePackSerializerOptions deserializerOptions
    );
}
