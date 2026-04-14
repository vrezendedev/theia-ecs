using MessagePack;
using Theia.ECS.Worlds;

namespace Theia.ECS.Serialization;

internal sealed class WorldDeserializer
{
    private readonly WorldDataTransferObject _dtoWorld;
    private readonly MessagePackSerializerOptions _deserializerOptions;

    internal WorldDeserializer(
        WorldDataTransferObject worldDataTransferObject,
        MessagePackSerializerOptions options
    )
    {
        _dtoWorld = worldDataTransferObject;
        _deserializerOptions = options;
    }

    internal void To(in World world) { }
}
