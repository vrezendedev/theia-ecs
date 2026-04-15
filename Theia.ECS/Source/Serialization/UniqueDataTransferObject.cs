using MessagePack;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal class UniqueDataTransferObject
{
    [Key(0)]
    public string? ComponentType { get; set; }

    [Key(1)]
    public byte[]? ComponentData { get; set; }
}
