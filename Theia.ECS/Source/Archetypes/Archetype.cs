using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
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
    private const int DefaultIndexersAndStoragesGrowthFactor = 2;
    private const int DefaultInvalidStorageMappingIndexes = -1;

    internal readonly int _archetypeId;
    internal readonly Signature _signature;
    internal readonly int _capacity;

    private readonly int[] _componentStorageMapping;

    private int _initializedCount;
    private Indexer[] _indexers;
    private Storage[][] _storages;

    private Assemblage? _assemblage;
    private Archetype[] _addEdges;
    private Archetype[] _removeEdges;

    private Stack<int> _free;
    private Queue<int> _lazy;

    internal Archetype(int archetypeId, Signature signature)
    {
        _archetypeId = archetypeId;
        _signature = signature;
        _capacity = GetCapacity(signature._sizeOf);
        _componentStorageMapping = GetStorageMapping(signature._maxId, signature.GetComponents());

        int componentsLength = signature._length;

        _indexers = new Indexer[1];
        _storages = new Storage[componentsLength][];

        for (int i = 0; i < _storages.Length; i++)
            _storages[i] = new Storage[1];

        _addEdges = Array.Empty<Archetype>();
        _removeEdges = Array.Empty<Archetype>();

        _free = new(1);
        _lazy = new(1);
        _lazy.Enqueue(0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TrySetMatchedAssemblage(Assemblage assemblage)
    {
        if (_assemblage is null)
        {
            _assemblage = assemblage;
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Assemblage? GetMatchedAssemblage() => _assemblage;

    private int GetCapacity(int signatureSize) =>
        Math.Max(
            AvailableMemoryPerChunk / (Unsafe.SizeOf<Entity>() + signatureSize),
            MinimumEntitiesPerChunk
        );

    private int[] GetStorageMapping(int maxId, ReadOnlySpan<int> signatureIds)
    {
        Span<int> ids = stackalloc int[maxId + 1];
        ids.Fill(DefaultInvalidStorageMappingIndexes);

        int index = 0;

        for (int i = 0; i < signatureIds.Length; i++)
        {
            ids[signatureIds[i]] = index;
            index++;
        }

        return ids.ToArray();
    }

    internal EntityAccounted Add(Entity entity)
    {
        int storageIndex = GetValidIndexerIndex();

        Indexer indexer = _indexers[storageIndex];

        int componentIndex = indexer.Add();
        indexer.Set(componentIndex, entity);

        if (!indexer.IsFull())
            _free.Push(storageIndex);

        return new EntityAccounted(_archetypeId, storageIndex, componentIndex);
    }

    internal EntitySwapped Remove(EntityMeta entityMeta)
    {
        int storageIndex = entityMeta._storageIndex;
        int componentIndex = entityMeta._componentIndex;

        Indexer indexer = _indexers[storageIndex];
        int swapped = indexer.Remove(componentIndex);

        _free.Push(storageIndex);

        if (swapped != EntitySwapped.InvalidEntitySwappedIndexes)
        {
            for (int i = 0; i < _storages.Length; i++)
                _storages[i][entityMeta._storageIndex].Move(swapped, componentIndex);

            return new EntitySwapped(indexer.Get(componentIndex)._id, componentIndex);
        }

        return EntitySwapped.None;
    }

    internal EntityTransferred Transfer(Entity entity, EntityMeta entityMeta, Archetype to)
    {
        EntityAccounted accounted = to.Add(entity);

        ReadOnlySpan<int> fromComponentStorageMappedIndexes = _componentStorageMapping;
        ReadOnlySpan<int> toComponentStorageMappedIndexes = to._componentStorageMapping;

        ReadOnlySpan<int> ids = to._signature.GetComponents();

        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] < fromComponentStorageMappedIndexes.Length)
            {
                int fromComponentStoragesIndex = fromComponentStorageMappedIndexes[ids[i]];

                if (fromComponentStoragesIndex != -1)
                {
                    int toComponentStoragesIndex = toComponentStorageMappedIndexes[ids[i]];

                    Storage fromStorage = _storages[fromComponentStoragesIndex][
                        entityMeta._storageIndex
                    ];
                    Storage toStorage = to._storages[toComponentStoragesIndex][
                        accounted._storageIndex
                    ];

                    fromStorage.Transfer(
                        entityMeta._componentIndex,
                        accounted._componentIndex,
                        toStorage
                    );
                }
            }
        }

        return new EntityTransferred(accounted, Remove(entityMeta));
    }

    private void Resize()
    {
        int currentLength = _indexers.Length;
        int targetLength = currentLength * DefaultIndexersAndStoragesGrowthFactor;

        Array.Resize(ref _indexers, targetLength);

        for (int i = 0; i < _storages.Length; i++)
            Array.Resize(ref _storages[i], targetLength);

        for (int i = currentLength; i < targetLength; i++)
            _lazy.Enqueue(i);
    }

    private void InitializeNextLazyIndexerAndStorages()
    {
        int nextId = _lazy.Dequeue();
        int capacity = _capacity;

        _indexers[nextId] = new Indexer(nextId, capacity);

        ReadOnlySpan<int> componentIndexes = _signature.GetComponents();

        for (int i = 0; i < componentIndexes.Length; i++)
        {
            int componentId = componentIndexes[i];
            int storageIndex = _componentStorageMapping[componentId];
            ComponentType componentType = ComponentsMeta.GetComponentType(componentId);
            _storages[storageIndex][nextId] = componentType.CreateStorage(capacity);
        }

        _initializedCount++;

        _free.Push(nextId);
    }

    private int GetValidIndexerIndex()
    {
        if (_free.Count == 0)
        {
            if (_lazy.Count == 0)
                Resize();

            InitializeNextLazyIndexerAndStorages();
        }

        return _free.Pop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Archetype? TryGetEdge(int componentId, ref Archetype[] edges) =>
        componentId > edges.Length - 1 ? null : edges[componentId];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetEdge(int componentId, Archetype archetype, ref Archetype[] edges)
    {
        if (componentId > edges.Length - 1)
            Array.Resize(ref edges, componentId + 1);

        edges[componentId] = archetype;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Archetype? GetAddEdge(int componentId) => TryGetEdge(componentId, ref _addEdges);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetAddEdge(int componentId, in Archetype archetype) =>
        SetEdge(componentId, archetype, ref _addEdges);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Archetype? GetRemoveEdge(int componentId) => TryGetEdge(componentId, ref _removeEdges);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetRemoveEdge(int componentId, in Archetype archetype) =>
        SetEdge(componentId, archetype, ref _removeEdges);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Indexer> GetIndexers() => _indexers.AsSpan(0, _initializedCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetStorageIndex(int id) => _componentStorageMapping[id];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Storage> GetStorages(int componentStorageIndex) =>
        _storages[componentStorageIndex].AsSpan(0, _initializedCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Storage> GetStorages<T>()
        where T : struct =>
        _storages[_componentStorageMapping[ComponentMeta<T>.s_id]].AsSpan(0, _initializedCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref T Get<T>(int storageIndex, in EntityMeta entityMeta)
        where T : struct =>
        ref ((Storage<T>)_storages[storageIndex][entityMeta._storageIndex]).Get(
            entityMeta._componentIndex
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref T Get<T>(in EntityMeta entityMeta)
        where T : struct
    {
        int storageIndex = _componentStorageMapping[ComponentMeta<T>.s_id];

        return ref ((Storage<T>)_storages[storageIndex][entityMeta._storageIndex]).Get(
            entityMeta._componentIndex
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set<T>(int storageIndex, in EntityMeta entityMeta, in T component)
        where T : struct =>
        ((Storage<T>)_storages[storageIndex][entityMeta._storageIndex]).Set(
            entityMeta._componentIndex,
            component
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set<T>(in EntityMeta entityMeta, in T component)
        where T : struct
    {
        int storageIndex = _componentStorageMapping[ComponentMeta<T>.s_id];

        ((Storage<T>)_storages[storageIndex][entityMeta._storageIndex]).Set(
            entityMeta._componentIndex,
            component
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool Has(int componentId)
    {
        if (componentId >= _componentStorageMapping.Length)
            return false;

        return _componentStorageMapping[componentId] != DefaultInvalidStorageMappingIndexes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool Has<T>()
        where T : struct => Has(ComponentMeta<T>.s_id);
}
