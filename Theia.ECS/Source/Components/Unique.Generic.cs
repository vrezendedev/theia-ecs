using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using MessagePack;

namespace Theia.ECS.Components;

internal sealed class Unique<TComponent> : Unique
    where TComponent : struct
{
    private TComponent _value;
    private readonly Lock _uniqueSet = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TComponent Read() => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TComponent Get() => ref _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(TComponent component) => _value = component;

    internal void Update(UpdateUnique<TComponent> update)
    {
        lock (_uniqueSet)
        {
            update(ref _value);
        }
    }

    internal override void WriteData(
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions options
    ) => MessagePackSerializer.Serialize(arrayBufferWriter, _value, options);

    internal override void CopyData(
        byte[] data,
        MessagePackSerializerOptions deserializerOptions
    ) => _value = MessagePackSerializer.Deserialize<TComponent>(data, deserializerOptions);
}
