using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;

namespace Theia.ECS.Archetypes;

/// <summary>
/// Storage container for entities sharing the same <see cref="Signature"/>. <b>Each archetype
/// holds its entities in fixed-capacity chunks</b>, paired (<see cref="Indexer"/> and
/// <see cref="Storage"/>[]) bundles. And forms a node in the archetype graph used to navigate
/// structural changes (component add/remove) in O(1).
/// </summary>
/// <remarks>
/// <para>
/// <b>Chunked layout:</b> an archetype owns parallel arrays: <c>_indexers[i]</c> holds the
/// entity slots for chunk <c>i</c>, while <c>_storages[componentSlot][i]</c> holds the
/// component data for that same chunk. Chunks are sized to fit roughly 16 KB of payload
/// (<see cref="AvailableMemoryPerChunk"/>) divided by the per-entity row width, with a floor
/// of <see cref="MinimumEntitiesPerChunk"/>; the divisor includes <see cref="Entity"/> itself
/// plus the sum of all component sizes from the signature.
/// </para>
/// <para>
/// <b>Lazy chunk initialization:</b> chunk slots are reserved up front via array growth, but
/// each chunk's <see cref="Indexer"/> and <see cref="Storage"/> instances are constructed only
/// when first needed. <c>_lazy</c> queues reserved-but-uninitialized chunk indices;
/// <c>_free</c> stacks initialized chunks that still have available capacity;
/// <c>_initializedCount</c> is the high-water mark of chunks ever initialized. <b>Archetypes that never grow past one
/// chunk pay only one chunk's worth of allocation.</b>
/// </para>
/// <para>
/// <b>Component slot indirection:</b> <c>_componentStorageMapping[componentId]</c> returns the
/// dense slot of that component within <c>_storages</c>, or <c>-1</c> if the archetype does
/// not contain the component. The indirection lets the archetype answer
/// <see cref="Has(int)"/> and <b>resolve component reads in a single bounds check plus an array
/// lookup, without scanning the signature.</b>
/// </para>
/// <para>
/// <b>Archetype graph:</b> <c>_addEdges[componentId]</c> points to the archetype reached by
/// adding that component to an entity here; <c>_removeEdges[componentId]</c> is the inverse.
/// Edges are populated lazily as transitions are first taken, amortizing graph discovery
/// across the lifetime of the world.
/// </para>
/// <para>
/// <b>Bound assemblage:</b> an archetype may be claimed by exactly one
/// <see cref="Assemblage"/> via <see cref="TrySetMatchedAssemblage"/>, which gives the
/// assemblage a fast path for entity creation against this exact composition. The first claim
/// wins; subsequent calls return <see langword="false"/>.
/// </para>
/// </remarks>
internal sealed class Archetype
{
    /// <summary>
    /// Accounts for memory overhead, including array headers, object metadata, and runtime cache
    /// pressure, by reserving a conservative 128 bytes per chunk unit. This conservative padding
    /// avoid greedy cache usage without significantly reducing the available chunk budget.
    /// </summary>
    private const int AvailableMemoryPerChunk = 16_384 - 128;

    /// <summary>Floor on chunk capacity. <b>Applied when the per-entity row width is large enough that the byte budget</b> would otherwise produce a tiny chunk.</summary>
    internal const int MinimumEntitiesPerChunk = 64;
    private const int DefaultIndexersAndStoragesGrowthFactor = 2;
    private const int DefaultInvalidStorageMappingIndexes = -1;

    internal readonly int _archetypeId;
    internal readonly Signature _signature;

    /// <summary>Number of entities each chunk can hold. Computed once at construction from the signature's row width.</summary>
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

    /// <summary>
    /// Claims this archetype for <paramref name="assemblage"/>. The first call succeeds and
    /// stores the binding; subsequent calls return <see langword="false"/> without modifying state.
    /// </summary>
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

    /// <summary>Returns the <see cref="Assemblage"/> bound to this archetype, or <see langword="null"/> if none has claimed it.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Assemblage? GetMatchedAssemblage() => _assemblage;

    private int GetCapacity(int signatureSize) =>
        Math.Max(
            AvailableMemoryPerChunk / (Unsafe.SizeOf<Entity>() + signatureSize),
            MinimumEntitiesPerChunk
        );

    private int[] GetStorageMapping(int maxId, ReadOnlySpan<int> signatureIds)
    {
        int[] ids = new int[maxId + 1];
        ids.AsSpan().Fill(DefaultInvalidStorageMappingIndexes);

        int index = 0;

        for (int i = 0; i < signatureIds.Length; i++)
        {
            ids[signatureIds[i]] = index;
            index++;
        }

        return ids;
    }

    /// <summary>
    /// Adds <paramref name="entity"/> to a chunk with available capacity (initializing a new
    /// chunk if needed) and returns an <see cref="EntityAccounted"/> describing the chunk and
    /// row the entity now occupies.
    /// </summary>
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

    /// <summary>
    /// Removes the entity at the chunk and row indicated by <paramref name="entityMeta"/>,
    /// swapping the last entity in the chunk into the freed row when necessary, and returns an
    /// <see cref="EntitySwapped"/> describing the swapped entity (or <see cref="EntitySwapped.None"/>
    /// when the removed row was already last).
    /// </summary>
    /// <remarks>
    /// Marks the chunk as free if it was previously full. The component storages for the
    /// affected slot are patched with the indexer's swap so chunk-internal
    /// component-row indices remain valid.
    /// </remarks>
    internal EntitySwapped Remove(EntityMeta entityMeta)
    {
        int storageIndex = entityMeta._storageIndex;
        int componentIndex = entityMeta._componentIndex;

        Indexer indexer = _indexers[storageIndex];

        bool wasFull = indexer.IsFull();

        int swapped = indexer.Remove(componentIndex);

        if (wasFull)
            _free.Push(storageIndex);

        if (swapped != EntitySwapped.InvalidEntitySwappedIndexes)
        {
            for (int i = 0; i < _storages.Length; i++)
                _storages[i][entityMeta._storageIndex].Move(swapped, componentIndex);

            return new EntitySwapped(indexer.Get(componentIndex)._id, componentIndex);
        }

        return EntitySwapped.None;
    }

    /// <summary>
    /// Moves <paramref name="entity"/> from this archetype to <paramref name="to"/>, <b>copying
    /// every component the destination shares with this archetype and removing the entity from
    /// here</b>. Returns an <see cref="EntityTransferred"/> describing both the destination
    /// placement and the swap that occurred at the source.
    /// </summary>
    /// <remarks>
    /// Iterates the destination's signature, mapping each shared component through both
    /// archetypes' <c>_componentStorageMapping</c> arrays to find the source and destination
    /// storage slots before delegating the per-component copy to <see cref="Storage.Transfer"/>.
    /// <b>Components present in this archetype but not in the destination are dropped; components
    /// present in the destination but not here are left at their default value</b> (the caller is
    /// expected to populate them).
    /// </remarks>
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

        _indexers[nextId] = new Indexer(capacity);

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

    /// <summary>Returns the archetype reached by adding <paramref name="componentId"/> to an entity in this archetype, or <see langword="null"/> if no such edge has been recorded yet.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Archetype? TryGetAddEdge(int componentId) => TryGetEdge(componentId, ref _addEdges);

    /// <summary>Records that adding <paramref name="componentId"/> to an entity in this archetype lands in <paramref name="archetype"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetAddEdge(int componentId, in Archetype archetype) =>
        SetEdge(componentId, archetype, ref _addEdges);

    /// <summary>Returns the archetype reached by removing <paramref name="componentId"/> from an entity in this archetype, or <see langword="null"/> if no such edge has been recorded yet.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Archetype? TryGetRemoveEdge(int componentId) =>
        TryGetEdge(componentId, ref _removeEdges);

    /// <summary>Records that removing <paramref name="componentId"/> from an entity in this archetype lands in <paramref name="archetype"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetRemoveEdge(int componentId, in Archetype archetype) =>
        SetEdge(componentId, archetype, ref _removeEdges);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetInitializedCount() => _initializedCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Indexer> GetIndexers() => _indexers.AsSpan(0, _initializedCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetStorageIndex(int id) => _componentStorageMapping[id];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Storage> GetStorages(int componentStorageIndex) =>
        _storages[componentStorageIndex].AsSpan(0, _initializedCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Storage GetStorage(int componentStorageIndex, int storageIndex) =>
        _storages[componentStorageIndex][storageIndex];

    /// <summary>Generic shorthand for <see cref="GetStorages(int)"/> using the static <see cref="ComponentMeta{TComponent}"/> ID; one closed-generic dispatch followed by a single array lookup.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span<Storage> GetStorages<TComponent>()
        where TComponent : struct =>
        _storages[_componentStorageMapping[ComponentMeta<TComponent>.s_id]]
            .AsSpan(0, _initializedCount);

    /// <summary>Returns a reference to the <typeparamref name="TComponent"/> at the row indicated by <paramref name="entityMeta"/>, with the component's storage slot supplied by the caller to skip the lookup.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TComponent Get<TComponent>(int storageIndex, in EntityMeta entityMeta)
        where TComponent : struct =>
        ref ((Storage<TComponent>)_storages[storageIndex][entityMeta._storageIndex]).Get(
            entityMeta._componentIndex
        );

    /// <summary>Returns a reference to the <typeparamref name="TComponent"/> at the row indicated by <paramref name="entityMeta"/>, resolving the storage slot via <see cref="ComponentMeta{TComponent}"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TComponent Get<TComponent>(in EntityMeta entityMeta)
        where TComponent : struct
    {
        int storageIndex = _componentStorageMapping[ComponentMeta<TComponent>.s_id];

        return ref ((Storage<TComponent>)_storages[storageIndex][entityMeta._storageIndex]).Get(
            entityMeta._componentIndex
        );
    }

    /// <summary>Writes <paramref name="component"/> into the row indicated by <paramref name="entityMeta"/>, with the component's storage slot supplied by the caller.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set<TComponent>(
        int storageIndex,
        in EntityMeta entityMeta,
        in TComponent component
    )
        where TComponent : struct =>
        ((Storage<TComponent>)_storages[storageIndex][entityMeta._storageIndex]).Set(
            entityMeta._componentIndex,
            component
        );

    /// <summary>Writes <paramref name="component"/> into the row indicated by <paramref name="entityMeta"/>, resolving the storage slot via <see cref="ComponentMeta{TComponent}"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set<TComponent>(in EntityMeta entityMeta, in TComponent component)
        where TComponent : struct
    {
        int storageIndex = _componentStorageMapping[ComponentMeta<TComponent>.s_id];

        ((Storage<TComponent>)_storages[storageIndex][entityMeta._storageIndex]).Set(
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

    /// <summary>Generic shorthand for <see cref="Has(int)"/> using the static <see cref="ComponentMeta{TComponent}"/> ID.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool Has<TComponent>()
        where TComponent : struct => Has(ComponentMeta<TComponent>.s_id);
}
