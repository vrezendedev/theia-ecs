using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

internal sealed class RelationsIndexer
{
    private const int DefaultAddedRelationsIdCapacity = 4;
    private const int DefaultRelationAddedGrowthFactor = 2;

    internal readonly Lock _lock = new();

    private int _addedRelationsIdCount;
    private int[] _addedRelationsId;
    private RelationKey[] _keys;

    private RelationLink[] _links;

    internal RelationsIndexer()
    {
        _addedRelationsId = new int[DefaultAddedRelationsIdCapacity];
        _keys = Array.Empty<RelationKey>();
        _links = Array.Empty<RelationLink>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool Has<T>(int relationId, in T[] array) =>
        relationId < array.Length && array[relationId] is not null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasLink(int relationId) => Has(relationId, _links);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasKey(int relationId) => Has(relationId, _keys);

    internal RelationLink GetOrAddLink(int relationId)
    {
        if (HasLink(relationId))
            return _links[relationId];

        RelationLink relationLink = RelationsMeta.GetRelationType(relationId).CreateRelationLink();

        if (relationId >= _links.Length)
            Array.Resize(ref _links, relationId + 1);

        _links[relationId] = relationLink;

        return relationLink;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> GetRelationsAdded() =>
        _addedRelationsId.AsSpan(0, _addedRelationsIdCount);

    internal void AccountPrimary(int relationId, int primaryKey)
    {
        int addedIndex = _addedRelationsIdCount;

        Array.AttemptResize(ref _addedRelationsId, addedIndex, DefaultRelationAddedGrowthFactor);

        _addedRelationsIdCount++;

        _addedRelationsId[addedIndex] = relationId;

        if (relationId >= _keys.Length)
        {
            int length = _keys.Length;
            Array.Resize(ref _keys, relationId + 1);
            _keys.AsSpan(length).Fill(new RelationKey());
        }

        RelationKey relationKey = new();
        relationKey.SetPrimaryKey(primaryKey);
        relationKey.SetIndexerAddedRelationIdIndex(addedIndex);

        _keys[relationId] = relationKey;
    }

    internal void RemovePrimary(int relationId)
    {
        ref RelationKey relationKey = ref _keys[relationId];

        int addedRelationIdIndex = relationKey.GetIndexerAddedRelationIdIndex();

        int last = _addedRelationsIdCount - 1;

        _addedRelationsIdCount--;

        if (addedRelationIdIndex < last)
        {
            int lastAddedRelationId = _addedRelationsId[last];
            _addedRelationsId[addedRelationIdIndex] = lastAddedRelationId;
            _keys[lastAddedRelationId].SetIndexerAddedRelationIdIndex(addedRelationIdIndex);
        }

        relationKey.Reset();
    }
}
