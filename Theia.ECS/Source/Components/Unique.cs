using System.Buffers;
using MessagePack;

namespace Theia.ECS.Components;

/// <summary>
/// Non-generic base for a world single instance component holder. Exposes the serialization hooks;
/// the actual typed value lives in <see cref="Unique{TComponent}"/>.
/// </summary>
/// <remarks>
/// A <see cref="Unique"/> represents <b> a single world-wide instance of a component type</b>; used
/// for state that conceptually belongs to the world itself rather than to any
/// individual entity.
/// </remarks>
internal abstract class Unique
{
    internal abstract void WriteData(
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions serializerOptions
    );

    internal abstract void CopyData(byte[] data, MessagePackSerializerOptions deserializerOptions);
}
