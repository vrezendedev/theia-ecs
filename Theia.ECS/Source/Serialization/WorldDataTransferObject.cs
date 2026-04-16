using MessagePack;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal sealed class WorldDataTransferObject
{
    [Key(0)]
    public uint Version { get; set; }

    [Key(1)]
    public required string[] ComponentsTypesAccounted { get; set; }

    [Key(2)]
    public required string[] RelationsTypesAccounted { get; set; }

    [Key(3)]
    public int MaxEntityId { get; set; }

    [Key(4)]
    public required ArchetypeDataTransferObject[] ArchetypesAccounted { get; set; }

    [Key(5)]
    public required RelationDataTransferObject[] RelationsAccounted { get; set; }

    [Key(6)]
    public required UniqueDataTransferObject[] UniquesAccounted { get; set; }

    public WorldDataTransferObject() { }
}
