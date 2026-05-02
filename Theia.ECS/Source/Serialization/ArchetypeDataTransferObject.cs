using MessagePack;
using Theia.ECS.Entities;

namespace Theia.ECS.Serialization;

/// <summary>
/// Per-archetype payload: a composition descriptor (<see cref="ComponentsTypeSet"/>), the
/// entities that lived in the archetype at serialization time, and the packed per-component
/// data: one <c>byte[]</c> per component type, MessagePack-encoded as a sequence of values
/// aligned with <see cref="Entities"/>.
/// </summary>
/// <remarks>
/// For an archetype with components A, B, C and N entities, <see cref="ComponentData"/> is three
/// <c>byte[]</c>s, <b>each carrying N values of one type</b>. This matches the runtime archetype
/// chunk layout, so deserialization can stream each component's values directly into its
/// destination storage without per-entity reshuffling.
/// </remarks>
[MessagePackObject(AllowPrivate = true)]
internal class ArchetypeDataTransferObject
{
    /// <summary>Fully-qualified type names of the components forming this archetype's composition. Order matches <see cref="ComponentData"/>'s outer dimension.</summary>
    [Key(0)]
    public required string[] ComponentsTypeSet { get; set; }

    /// <summary>The entities living in this archetype at serialization time, in storage order.</summary>
    [Key(1)]
    public required Entity[] Entities { get; set; }

    /// <summary>Packed component values, one inner array per component type, MessagePack-encoded values aligned index-for-index with <see cref="Entities"/>.</summary>
    [Key(2)]
    public required byte[][] ComponentData { get; set; }
}
