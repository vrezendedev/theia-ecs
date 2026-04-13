using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Theia.ECS.Serialization;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private static readonly MessagePackSerializerOptions DefaultMessagePackOptions =
        MessagePackSerializerOptions.Standard.WithResolver(
            MessagePack.Resolvers.CompositeResolver.Create(
                MessagePack.Resolvers.StandardResolver.Instance,
                MessagePack.Resolvers.ContractlessStandardResolver.Instance
            )
        );

    private WorldDataTransferObject CreateWorldDataTransferObject() =>
        new WorldSerializer().Create(this);

    public void Serialize(string path)
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        using FileStream fileStream = new FileStream(path, FileMode.Create);
        MessagePackSerializer.Serialize(
            fileStream,
            CreateWorldDataTransferObject(),
            DefaultMessagePackOptions
        );
    }

    public async Task SerializeAsync(string path, CancellationToken cancellationToken = default)
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        await using FileStream fileStream = new FileStream(path, FileMode.Create);
        await MessagePackSerializer.SerializeAsync(
            fileStream,
            CreateWorldDataTransferObject(),
            DefaultMessagePackOptions,
            cancellationToken
        );
    }
}
