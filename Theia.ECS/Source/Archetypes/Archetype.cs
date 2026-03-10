using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theia.ECS.Components;
using Theia.ECS.Entities;

namespace Theia.ECS.Archetypes;

internal sealed class Archetype
{
    /// <summary>
    /// Accounts for memory overhead, including array headers, object metadata, and runtime cache
    /// pressure, by reserving a conservative 128 bytes per chunk unit. This conservative padding
    /// avoid greedy cache usage without significantly reducing the available chunk budget.
    /// </summary>
    private const int AvailableMemoryPerChunk = 16_384 - 128;
    private const int MinimumEntitiesPerChunk = 64;

    internal readonly int _archetypeId;
    internal readonly Signature _signature;
    private readonly int _capacity;
    private readonly int[] _componentStorageMapping;

    private Indexer[] _indexers;
    private Storage[][] _storages;

    private Stack<int> _free = new();
    private Queue<int> _lazy;

    internal Archetype(int archetypeId, Signature signature)
    {
        _archetypeId = archetypeId;
        _signature = signature;
        _capacity = GetCapacity(signature.SizeOf());
        _componentStorageMapping = GetStorageMapping(signature.Values());

        int components = signature.Length;
        _indexers = [new Indexer(0, _capacity)];
        _storages = new Storage[components][];

        for (int i = 0; i < components; i++)
            _storages[i] = new Storage[1];

        _free = new(1);
        _free.Push(0);
        _lazy = new();
    }

    private int GetCapacity(int signatureSize) =>
        Math.Max(
            AvailableMemoryPerChunk / (Unsafe.SizeOf<Entity>() + signatureSize),
            MinimumEntitiesPerChunk
        );

    private int[] GetStorageMapping(ReadOnlySpan<int> signatureIds)
    {
        int maxId = -1;

        for (int i = 0; i < signatureIds.Length; i++)
            maxId = Math.Max(maxId, signatureIds[i]);

        Span<int> ids = stackalloc int[maxId + 1];
        ids.Fill(-1);

        int index = 0;

        for (int i = 0; i < signatureIds.Length; i++)
        {
            ids[signatureIds[i]] = index;
            index++;
        }

        return ids.ToArray();
    }

    private void InitializeStorages(int index)
    {
        int capacity = _capacity;
        ReadOnlySpan<int> componentIndexes = _signature.Values();

        for (int i = 0; i < componentIndexes.Length; i++)
        {
            int componentId = componentIndexes[i];
            int storageIndex = _componentStorageMapping[componentId];
            ComponentType componentType = ComponentsMeta.GetComponentType(componentId);
            _storages[storageIndex][index] = componentType.CreateStorage(capacity);
        }
    }

    internal void Resize()
    {
        int currentLength = _indexers.Length;
        int length = currentLength * 2;

        Array.Resize(ref _indexers, length);

        for (int i = 0; i < _storages.Length; i++)
            Array.Resize(ref _storages[i], length);

        for (int i = currentLength; i < length; i++)
            _lazy.Enqueue(i);
    }

    internal void Add(in Entity entity, ref EntityMeta entityMeta)
    {
        throw new NotImplementedException();
    }

    internal void Remove(in Entity entity, ref EntityMeta entityMeta)
    {
        throw new NotImplementedException();
    }

    internal void Transfer(ref EntityMeta entityMeta, Archetype to)
    {
        throw new NotImplementedException();
    }

    internal Span<Indexer> GetIndexers() => _indexers;

    internal Span<Storage> GetStorages<T>()
        where T : struct => _storages[_componentStorageMapping[ComponentMeta<T>.s_id]];

    internal ref T Get<T>(in EntityMeta entityMeta)
        where T : struct
    {
        int storageIndex = _componentStorageMapping[ComponentMeta<T>.s_id];

        return ref ((Storage<T>)_storages[storageIndex][entityMeta._componentStorageIndex]).Get(
            entityMeta._componentIndex
        );
    }

    internal void Set<T>(in EntityMeta entityMeta, in T component)
        where T : struct
    {
        int storageIndex = _componentStorageMapping[ComponentMeta<T>.s_id];

        ((Storage<T>)_storages[storageIndex][entityMeta._componentStorageIndex]).Set(
            entityMeta._componentIndex,
            component
        );
    }
}
