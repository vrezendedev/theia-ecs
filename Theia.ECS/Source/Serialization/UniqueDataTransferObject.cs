using MessagePack;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal class UniqueDataTransferObject
{
    [Key(0)]
    public required string ComponentType { get; set; }

    [Key(1)]
    public required byte[] ComponentData { get; set; }
}
