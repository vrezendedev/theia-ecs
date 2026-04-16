using MessagePack;
using Theia.ECS.Entities;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal class RelationDataTransferObject
{
    [Key(0)]
    public required string RelationType { get; set; }

    [Key(1)]
    public required EntityRelationDataTransferObject[] EntityRelations { get; set; }
}

[MessagePackObject(AllowPrivate = true)]
internal class EntityRelationDataTransferObject
{
    [Key(0)]
    public required Entity Owner { get; set; }

    [Key(1)]
    public required Entity[] Related { get; set; }

    [Key(2)]
    public required byte[] RelationData { get; set; }
}
