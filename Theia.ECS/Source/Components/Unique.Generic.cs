using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using MessagePack;

namespace Theia.ECS.Components;

/// <summary>
/// Generic concrete <see cref="Unique"/> for a specific <typeparamref name="TComponent"/>.
/// Holds the unique value, exposes direct read and write access, and offers a
/// <see cref="Query"/> entry point that mutates the value under an internal lock.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Read"/>, <see cref="Get"/>, and <see cref="Set"/> <b>are unsynchronized</b>: they hand
/// the value, or a reference to it, directly to the caller.
/// <br/>
/// <see cref="Query"/> <b>is the synchronized alternative</b>: it acquires the internal lock, invokes
/// <see cref="IUniqueQuery{TComponent}.Execute(ref TComponent)"/> on the supplied callback, and
/// releases the lock when the callback returns. Use <see cref="Query"/> when multiple systems
/// may concurrently mutate the same unique.
/// </para>
/// </remarks>
internal sealed class Unique<TComponent> : Unique
    where TComponent : struct
{
    private TComponent _value;
    private readonly Lock _uniqueSet = new();

    /// <summary>Returns a copy of the current value. Unsynchronized.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TComponent Read() => _value;

    /// <summary>Returns a mutable reference to the underlying value. Unsynchronized.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TComponent Get() => ref _value;

    /// <summary>Replaces the current value with <paramref name="component"/>. Unsynchronized.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(TComponent component) => _value = component;

    /// <summary>
    /// Invokes <paramref name="query"/> with a reference to the unique value while holding
    /// the unique's internal lock, providing atomic read-modify-write semantics for the duration
    /// of the callback.
    /// </summary>
    internal void Query<T>(ref T query)
        where T : struct, IUniqueQuery<TComponent>, allows ref struct
    {
        lock (_uniqueSet)
        {
            query.Execute(ref _value);
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
