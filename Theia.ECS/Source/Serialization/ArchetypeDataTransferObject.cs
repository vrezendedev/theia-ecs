using MessagePack;
using Theia.ECS.Entities;

namespace Theia.ECS.Serialization;

[MessagePackObject(AllowPrivate = true)]
internal class ArchetypeDataTransferObject
{
    [Key(0)]
    public string[]? ComponentSet { get; set; }

    [Key(1)]
    public Entity[]? StaleEntities { get; set; }

    [Key(2)]
    public byte[][]? ComponentData { get; set; }
}
