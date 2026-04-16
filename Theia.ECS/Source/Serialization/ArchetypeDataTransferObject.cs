using MessagePack;
using Theia.ECS.Entities;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal class ArchetypeDataTransferObject
{
    [Key(0)]
    public required string[] ComponentsTypeSet { get; set; }

    [Key(1)]
    public required Entity[] Entities { get; set; }

    [Key(2)]
    public required byte[][] ComponentData { get; set; }
}
