using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Extensions;

namespace Theia.ECS.Relations;

internal sealed class RelationsIndexer
{
    private const int DefaultAddedCapacity = 4;
    private const int DefaultAddedGrowthFactor = 2;

    internal readonly Lock _lock = new();

    private int _addedRelationsIdCount;
    private int[] _addedRelationsId;
    private RelationKey[] _keys;

    private int _addedLinksCount;
    private int[] _addedLinks;
    private RelationLink[] _links;

    internal RelationsIndexer()
    {
        _addedRelationsId = new int[DefaultAddedCapacity];
        _keys = Array.Empty<RelationKey>();
        _addedLinks = new int[DefaultAddedCapacity];
        _links = Array.Empty<RelationLink>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasKey(int relationId) =>
        relationId < _keys.Length && _keys[relationId].HasRelation();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasLink(int relationId) =>
        relationId < _links.Length && _links[relationId] is not null;

    internal RelationLink GetOrRentLink(int relationId)
    {
        if (HasLink(relationId))
            return _links[relationId];

        int addedIndex = _addedLinksCount;

        Array.TryResize(ref _addedLinks, addedIndex, DefaultAddedGrowthFactor);

        _addedLinksCount++;
        _addedLinks[addedIndex] = relationId;

        RelationLink relationLink = RelationsMeta.GetRelationType(relationId).CreateRelationLink();

        if (relationId >= _links.Length)
            Array.Resize(ref _links, relationId + 1);

        relationLink.SetIndexerAddedLinkIndex(addedIndex);

        _links[relationId] = relationLink;

        return relationLink;
    }

    internal void ReturnLink(int relationId)
    {
        RelationLink relationLink = _links[relationId];

        int addedLinkIndex = relationLink.GetIndexerAddedLinkIndex();

        int last = _addedLinksCount - 1;

        _addedLinksCount--;

        if (addedLinkIndex < last)
        {
            int lastAddedLinkId = _addedLinks[last];
            _addedLinks[addedLinkIndex] = lastAddedLinkId;
            _links[lastAddedLinkId].SetIndexerAddedLinkIndex(addedLinkIndex);
        }

        _links[relationId] = null!;

        RelationsMeta.GetRelationType(relationId).PoolRelationLink(relationLink);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> GetRelationsAdded() =>
        _addedRelationsId.AsSpan(0, _addedRelationsIdCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref RelationKey GetRelationKey(int relationId) => ref _keys[relationId];

    internal void AddKey(int relationId, int primaryKey)
    {
        int addedIndex = _addedRelationsIdCount;

        Array.TryResize(ref _addedRelationsId, addedIndex, DefaultAddedGrowthFactor);

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

    internal void RemoveRelationKey(int relationId)
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
