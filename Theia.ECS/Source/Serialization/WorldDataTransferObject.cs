using MessagePack;

namespace Theia.ECS.Serialization;

/// <summary>
/// Top-level serialization payload for a <see cref="Worlds.World">World</see>. <b>Captures everything needed
/// to reconstruct world state on deserialization</b>: a format version, the component and relation
/// type rosters, the highest entity ID issued, and the per-archetype, per-relation, and
/// per-unique payloads.
/// </summary>
/// <remarks>
/// The type rosters (<see cref="ComponentsTypesAccounted"/> and <see cref="RelationsTypesAccounted"/>)
/// are stored as fully-qualified type-name strings so the deserializer can match incoming archetype
/// composition against the runtime type registry <b>without depending on type IDs being stable
/// across builds</b>. A type added or removed between serialize and deserialize will simply not be
/// found, and the affected archetype or relation will fail to materialize.
/// </remarks>
[MessagePackObject(AllowPrivate = true)]
internal sealed class WorldDataTransferObject
{
    /// <summary>Format version. Lets the deserializer reject or migrate payloads from incompatible builds.</summary>
    [Key(0)]
    public uint Version { get; set; }

    /// <summary>Fully-qualified type names of every component type referenced by any archetype in the payload.</summary>
    [Key(1)]
    public required string[] ComponentsTypesAccounted { get; set; }

    /// <summary>Fully-qualified type names of every relation type referenced by the payload.</summary>
    [Key(2)]
    public required string[] RelationsTypesAccounted { get; set; }

    /// <summary>The total number of entity slots issued by the source world (live + ghoulified). Used to size the destination world's entity-meta array during deserialization.</summary>
    [Key(3)]
    public int MaxEntityId { get; set; }

    /// <summary>Per-archetype payloads: composition, member entities, and packed component data.</summary>
    [Key(4)]
    public required ArchetypeDataTransferObject[] ArchetypesAccounted { get; set; }

    /// <summary>Per-relation-type payloads: every (owner, targets, optional payload) link group in the source world.</summary>
    [Key(5)]
    public required RelationDataTransferObject[] RelationsAccounted { get; set; }

    /// <summary>Per-unique payloads: every world unique component value in the source world.</summary>
    [Key(6)]
    public required UniqueDataTransferObject[] UniquesAccounted { get; set; }

    public WorldDataTransferObject() { }
}
