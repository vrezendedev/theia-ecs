using System;
using System.Buffers;
using MessagePack;

namespace Theia.ECS.Components;

/// <summary>
/// Non-generic base for a per-chunk component storage block. <b>Carries the contract used by an
/// <see cref="Archetypes.Archetype">Archetype</see> to move and transfer rows without knowing the concrete
/// component type</b>, plus the bulk serialization hooks used during world serialization and deserialization.
/// </summary>
/// <remarks>
/// <para>
/// One <see cref="Storage"/> instance holds the values for a single component type within a
/// single chunk. The <see cref="Archetypes.Archetype">Archetype</see> keeps a <c>Storage[][]</c> indexed first by component slot
/// and then by chunk index; together with the matching <see cref="Archetypes.Indexer">Indexer</see>, these
/// form the chunk's full row layout.
/// </para>
/// <para>
/// The generic <see cref="Storage{TComponent}"/> derivative carries the actual typed array. The
/// non-generic surface here exists so the archetype can iterate, swap, and serialize component
/// data uniformly across all of its component slots without static <c>T</c>.
/// </para>
/// </remarks>
internal abstract class Storage
{
    /// <summary>
    /// Copies the component value at <paramref name="from"/> into the slot at
    /// <paramref name="to"/> within this same storage, <b>used by swap-remove to keep chunk indices
    /// dense after an entity is removed</b>.
    /// </summary>
    internal abstract void Move(int from, int to);

    /// <summary>
    /// Copies the component value at <paramref name="oldIndex"/> in this storage into
    /// <paramref name="newIndex"/> in <paramref name="to"/>. <b>Used during cross-archetype moves
    /// when an entity's composition changes</b>.
    /// </summary>
    internal abstract void Transfer(int oldIndex, int newIndex, Storage to);

    /// <summary>
    /// Serializes the populated portions of every chunk's storage for this component type into
    /// <paramref name="arrayBufferWriter"/> as a single contiguous MessagePack array. The
    /// generic override knows the element type and uses an <see cref="ArrayPool{T}"/> staging
    /// buffer to avoid allocating a fresh array per save.
    /// </summary>
    internal abstract void WriteAllData(
        ReadOnlySpan<Storage> storages,
        int accLength,
        ReadOnlySpan<int> lengths,
        ArrayBufferWriter<byte> arrayBufferWriter,
        MessagePackSerializerOptions serializerOptions
    );

    /// <summary>
    /// Inverse of <see cref="WriteAllData"/>: deserializes <paramref name="rawComponents"/> into
    /// a typed array and distributes it across the chunk storages, <paramref name="perStorageCapacity"/>
    /// elements per chunk.
    /// </summary>
    internal abstract void CopyAllData(
        ReadOnlySpan<Storage> storages,
        byte[] rawComponents,
        int perStorageCapacity,
        MessagePackSerializerOptions deserializerOptions
    );
}
