using MessagePack;
using Theia.ECS.Entities;

namespace Theia.ECS.Serialization;

/// <summary>
/// Per-relation-type payload: every owner that participated in this relation type, paired with
/// its target list and, for data relations, per-link payloads.
/// </summary>
[MessagePackObject(AllowPrivate = true)]
internal class RelationDataTransferObject
{
    /// <summary>Fully-qualified type name of the relation.</summary>
    [Key(0)]
    public required string RelationType { get; set; }

    /// <summary>Per-owner relation groups: every entity that owns at least one link of this relation type, with its targets and per-link data.</summary>
    [Key(1)]
    public required EntityRelationDataTransferObject[] EntityRelations { get; set; }
}

/// <summary>
/// Single owner's relation payload: the owner entity, its target list, and, for data relations,
/// the per-link payload bytes aligned to <see cref="Related"/>.
/// </summary>
/// <remarks>
/// For tag relations <see cref="RelationData"/> is empty; for data relations it carries
/// MessagePack-encoded values aligned index-for-index with <see cref="Related"/>.
/// </remarks>
[MessagePackObject(AllowPrivate = true)]
internal class EntityRelationDataTransferObject
{
    /// <summary>The entity that owns the relation links described by this payload.</summary>
    [Key(0)]
    public required Entity Owner { get; set; }

    /// <summary>The targets <see cref="Owner"/> points at under this relation type, in dense storage order.</summary>
    [Key(1)]
    public required Entity[] Related { get; set; }

    /// <summary>MessagePack-encoded per-link payloads aligned with <see cref="Related"/>; empty for tag relations.</summary>
    [Key(2)]
    public required byte[] RelationData { get; set; }
}
