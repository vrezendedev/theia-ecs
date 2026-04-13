using MessagePack;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal sealed class WorldDataTransferObject
{
    [Key(0)]
    public uint Version { get; set; }

    [Key(1)]
    public string[]? ComponentsTypesAccounted { get; set; }

    [Key(2)]
    public string[]? RelationsTypesAccounted { get; set; }

    [Key(3)]
    public int MaxEntityId { get; set; }

    [Key(4)]
    public ArchetypeDataTransferObject[]? ArchetypesAccounted { get; set; }

    [Key(5)]
    public RelationDataTransferObject[]? RelationsAccounted { get; set; }

    public WorldDataTransferObject() { }
}
