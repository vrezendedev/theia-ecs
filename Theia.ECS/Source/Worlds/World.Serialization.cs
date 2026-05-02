using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Theia.ECS.Serialization;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    /// <summary>
    /// The default <see cref="MessagePackSerializerOptions"/>.
    ///  Combines the standard resolver with the contractless resolver <b>so user-defined component and relation
    /// types can be serialized without per-type attributes</b>.
    /// </summary>
    /// <remarks>
    /// The contractless resolver is what enables Theia's "one-call serialization" promise:
    /// component and relation structs do not need <c>[MessagePackObject]</c> attributes or
    /// surrogate types. If a project needs custom serialization for a specific type, supply
    /// custom options to the <c>Serialize</c>/<c>Deserialize</c> calls instead of mutating this
    /// shared default.
    /// </remarks>
    public static readonly MessagePackSerializerOptions DefaultMessagePackSerializerOptions =
        MessagePackSerializerOptions.Standard.WithResolver(
            MessagePack.Resolvers.CompositeResolver.Create(
                MessagePack.Resolvers.StandardResolver.Instance,
                MessagePack.Resolvers.ContractlessStandardResolver.Instance
            )
        );

    private WorldDataTransferObject CreateWorldDataTransferObject(
        MessagePackSerializerOptions serializerOptions
    ) => new WorldSerializer(serializerOptions).Create(this);

    private void Deserialize(
        WorldDataTransferObject worldDataTransferObject,
        MessagePackSerializerOptions deserializerOptions
    ) => new WorldDeserializer(worldDataTransferObject, deserializerOptions).To(this);

    /// <summary>
    /// <b>Writes the entire world</b>, every entity, archetype, component, relation, unique, and
    /// resource, to <paramref name="path"/> as a single MessagePack-encoded file. Overwrites
    /// the file if it exists.
    /// </summary>
    /// <param name="path">Filesystem path of the destination file.</param>
    /// <param name="serializerOptions">
    /// MessagePack options. Defaults to <see cref="DefaultMessagePackSerializerOptions"/>, which
    /// handles user-defined component and relation types without per-type attributes.
    /// </param>
    /// <exception cref="InvalidOperationException">Thrown if a query is currently iterating or a deferred flush is in progress.</exception>
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

    /// <summary>
    /// Restores world state from a MessagePack-encoded file at <paramref name="path"/>,
    /// previously written by <see cref="Serialize(string, MessagePackSerializerOptions?)"/>.
    /// </summary>
    /// <param name="path">Filesystem path of the source file.</param>
    /// <param name="deserializerOptions">
    /// MessagePack options. Should match the options used to serialize the file; defaults to
    /// <see cref="DefaultMessagePackSerializerOptions"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">Thrown if a query is currently iterating or a deferred flush is in progress.</exception>
    public void Deserialize(string path, MessagePackSerializerOptions? deserializerOptions = null)
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        deserializerOptions ??= DefaultMessagePackSerializerOptions;

        using FileStream fileStream = new FileStream(path, FileMode.Open);
        Deserialize(
            MessagePackSerializer.Deserialize<WorldDataTransferObject>(
                fileStream,
                deserializerOptions
            ),
            deserializerOptions
        );
    }

    /// <summary>
    /// Asynchronous variant of <see cref="Serialize(string, MessagePackSerializerOptions?)"/>.
    /// Suitable for save points where blocking the main thread is undesirable; the world's data
    /// transfer object is built synchronously before the file write begins.
    /// </summary>
    /// <param name="path">Filesystem path of the destination file.</param>
    /// <param name="serializerOptions">MessagePack options; defaults to <see cref="DefaultMessagePackSerializerOptions"/>.</param>
    /// <param name="cancellationToken">Cancellation token forwarded to the underlying file write.</param>
    /// <exception cref="InvalidOperationException">Thrown if a query is currently iterating or a deferred flush is in progress at call time.</exception>
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

    /// <summary>
    /// Asynchronous variant of <see cref="Deserialize(string, MessagePackSerializerOptions?)"/>.
    /// The file read is async; the world-state restoration phase runs synchronously after the
    /// transfer object is fully loaded.
    /// </summary>
    /// <param name="path">Filesystem path of the source file.</param>
    /// <param name="deserializerOptions">MessagePack options; should match the serialization-time options. Defaults to <see cref="DefaultMessagePackSerializerOptions"/>.</param>
    /// <param name="cancellationToken">Cancellation token forwarded to the underlying file read.</param>
    /// <exception cref="InvalidOperationException">Thrown if a query is currently iterating or a deferred flush is in progress at call time.</exception>
    public async Task DeserializeAsync(
        string path,
        MessagePackSerializerOptions? deserializerOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        deserializerOptions ??= DefaultMessagePackSerializerOptions;

        await using FileStream fileStream = new FileStream(path, FileMode.Open);
        Deserialize(
            await MessagePackSerializer.DeserializeAsync<WorldDataTransferObject>(
                fileStream,
                deserializerOptions,
                cancellationToken
            ),
            deserializerOptions
        );
    }
}
