using MessagePack;
using Theia.ECS.Entities;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal class ArchetypeDataTransferObject
{
    [Key(0)]
    public string[]? ComponentsTypeSet { get; set; }

    [Key(1)]
    public Entity[]? Entities { get; set; }

    [Key(2)]
    public byte[][]? ComponentData { get; set; }
}
