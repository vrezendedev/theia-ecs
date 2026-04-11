#pragma warning disable CS8618
using MessagePack;

namespace Theia.ECS.Serialization;

[MessagePackObject(true, AllowPrivate = true)]
internal sealed class WorldDataTransferObject
{
    internal uint Version { get; set; }
    internal string[] ComponentsAccounted { get; set; }
    internal string[] RelationsAccounted { get; set; }

    // internal EntityDataTransfer[] EntityDataTransfer { get; set; }
    // internal ArchetypeDataTransferObject[] Archetypes { get; set; }

    public WorldDataTransferObject() { }
}
#pragma warning restore CS8618
