using MessagePack;

namespace Theia.ECS.Serialization;

/// <summary>
/// Per-unique payload: the component type and its MessagePack-encoded value. One entry per
/// world unique component populated in the source world.
/// </summary>
[MessagePackObject(AllowPrivate = true)]
internal class UniqueDataTransferObject
{
    /// <summary>Fully-qualified type name of the unique's component.</summary>
    [Key(0)]
    public required string ComponentType { get; set; }

    /// <summary>MessagePack-encoded component value.</summary>
    [Key(1)]
    public required byte[] ComponentData { get; set; }
}
