using MessagePack;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal class RelationDataTransferObject
{
    [Key(0)]
    public string? Relation { get; set; }
}
