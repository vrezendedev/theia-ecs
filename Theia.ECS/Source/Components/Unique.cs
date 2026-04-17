using System.Buffers;
using MessagePack;

namespace Theia.ECS.Components;

internal abstract class Unique
{
    internal abstract void WriteData(
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions serializerOptions
    );

    internal abstract void CopyData(byte[] data, MessagePackSerializerOptions deserializerOptions);
}
