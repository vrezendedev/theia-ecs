using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Theia.ECS.Serialization;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    public static MessagePackSerializerOptions DefaultMessagePackSerializerOptions { get; } =
        MessagePackSerializerOptions.Standard.WithResolver(
            MessagePack.Resolvers.CompositeResolver.Create(
                MessagePack.Resolvers.StandardResolver.Instance,
                MessagePack.Resolvers.ContractlessStandardResolver.Instance
            )
        );

    private WorldDataTransferObject CreateWorldDataTransferObject(
        MessagePackSerializerOptions serializerOptions
    ) => new WorldSerializer(serializerOptions).Create(this);

    public void Serialize(string path, MessagePackSerializerOptions? serializerOptions = null)
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        serializerOptions ??= DefaultMessagePackSerializerOptions;

        using FileStream fileStream = new FileStream(path, FileMode.Create);
        MessagePackSerializer.Serialize(
            fileStream,
            CreateWorldDataTransferObject(serializerOptions),
            serializerOptions
        );
    }

    public async Task SerializeAsync(
        string path,
        MessagePackSerializerOptions? serializerOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        serializerOptions ??= DefaultMessagePackSerializerOptions;

        await using FileStream fileStream = new FileStream(path, FileMode.Create);
        await MessagePackSerializer.SerializeAsync(
            fileStream,
            CreateWorldDataTransferObject(serializerOptions),
            serializerOptions,
            cancellationToken
        );
    }
}
