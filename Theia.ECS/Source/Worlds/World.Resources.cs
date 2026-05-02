using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Resources;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Dictionary<ResourcesIdentifier, object> _resources;

    /// <summary>
    /// Creates a <see cref="Resources{TKey, TData}"/> store for the given
    /// (<typeparamref name="TKey"/>, <typeparamref name="TData"/>) pair, registering one entry
    /// per enum value, and returns this <see cref="World">World</see> for fluent chaining.
    /// </summary>
    /// <typeparam name="TKey">An enum whose underlying type is <see cref="int"/>, with values dense and starting at zero.</typeparam>
    /// <typeparam name="TData">The managed reference type held under each enum value.</typeparam>
    /// <param name="entries">
    /// One entry per <typeparamref name="TKey"/> value. Every enum value must appear exactly
    /// once; the order of <paramref name="entries"/> does not matter.
    /// </param>
    /// <returns>This <see cref="World"/>, to allow chaining further <c>CreateResource</c> calls.</returns>
    /// <remarks>
    /// <para>
    /// Resources are immutable after creation. Validation runs in a single pass: <b>the entry count
    /// must equal the number of enum values, each entry's key must fall within the dense range,
    /// and no key may appear more than once</b>. Failures throw before any state is committed, so a
    /// rejected call leaves the world unchanged.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="entries"/> fails during validation.
    /// </exception>
    public World CreateResource<TKey, TData>(
        params ReadOnlySpan<ResourcesEntries<TKey, TData>> entries
    )
        where TKey : unmanaged, Enum
        where TData : class
    {
        ReadOnlySpan<TKey> keys = Resources<TKey, TData>.Keys;

        if (entries.Length != keys.Length)
            ThrowMissingKeys<TKey, TData>();

        ResourcesIdentifier resourcesIdentifier = new ResourcesIdentifier(
            typeof(TKey),
            typeof(TData)
        );

        if (_resources.ContainsKey(resourcesIdentifier))
            ThrowResourceAlreadyCreated<TKey, TData>();

        const int MaxStackSize = 256;
        Span<bool> seen =
            keys.Length <= MaxStackSize ? stackalloc bool[keys.Length] : new bool[keys.Length];

        Resources<TKey, TData> resources = new(entries.Length);

        foreach (ResourcesEntries<TKey, TData> entry in entries)
        {
            TKey key = entry.Key;

            int index = Unsafe.As<TKey, int>(ref key);

            if (index < 0 || index >= keys.Length)
                ThrowOutOfRangeKey<TKey, TData>(entry.Key);
            if (seen[index])
                ThrowDuplicateKey<TKey, TData>(entry.Key);

            seen[index] = true;

            resources.Register(entry.Key, entry.Data);
        }

        _resources.Add(resourcesIdentifier, resources);

        return this;
    }

    /// <summary>
    /// Returns the <see cref="Resources{TKey, TData}"/> store previously created on this world
    /// for the given (<typeparamref name="TKey"/>, <typeparamref name="TData"/>) pair.
    /// </summary>
    /// <typeparam name="TKey">The enum key type used at <see cref="CreateResource"/>.</typeparam>
    /// <typeparam name="TData">The reference data type used at <see cref="CreateResource"/>.</typeparam>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no resource for this pair has been created on this world. Resources are
    /// per-world, so worlds with the same setup code still need to call <see cref="CreateResource"/>
    /// independently.
    /// </exception>
    public Resources<TKey, TData> GetResource<TKey, TData>()
        where TKey : unmanaged, Enum
        where TData : class
    {
        ResourcesIdentifier id = new(typeof(TKey), typeof(TData));

        if (!_resources.TryGetValue(id, out object? resource))
            throw new InvalidOperationException(
                $"Resource ['{typeof(TKey).Name}','{typeof(TData).Name}'] has not been created; create it first."
            );

        return (Resources<TKey, TData>)resource;
    }

    [DoesNotReturn]
    private static void ThrowResourceAlreadyCreated<TKey, TData>() =>
        throw new InvalidOperationException(
            $"Resource with Key of type '{typeof(TKey).Name}' and Data of type '{typeof(TData).Name}' already created."
        );

    [DoesNotReturn]
    private static void ThrowMissingKeys<TKey, TData>() =>
        throw new InvalidOperationException(
            $"Resource ['{typeof(TKey).Name}','{typeof(TData).Name}'] must register every enum value exactly once."
        );

    [DoesNotReturn]
    private static void ThrowOutOfRangeKey<TKey, TData>(TKey key) =>
        throw new InvalidOperationException(
            $"Key '{typeof(TKey).Name}.{key}' is out of range for resource ['{typeof(TKey).Name}','{typeof(TData).Name}']. Enum values must be dense and start at 0."
        );

    [DoesNotReturn]
    private static void ThrowDuplicateKey<TKey, TData>(TKey key) =>
        throw new InvalidOperationException(
            $"Key '{typeof(TKey).Name}.{key}' is registered more than once for resource ['{typeof(TKey).Name}','{typeof(TData).Name}']. Each enum value must appear exactly once."
        );
}
