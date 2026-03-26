using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Theia.ECS.Relations;

internal sealed class RelationsIndexer
{
    private RelationKey[] _keys;
    private readonly Lock _lock = new();

    internal RelationsIndexer() => _keys = Array.Empty<RelationKey>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasKey(int relationId)
    {
        RelationKey[] keys = _keys;

        return (uint)relationId < (uint)keys.Length && keys[relationId] is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal RelationKey? GetKey(int relationId)
    {
        RelationKey[] keys = _keys;
        return (uint)relationId < (uint)keys.Length ? keys[relationId] : null;
    }

    internal void AddKey(int relationId, RelationKey relationKey)
    {
        RelationKey? oldKey;

        lock (_lock)
        {
            if (relationId >= _keys.Length)
                Array.Resize(ref _keys, relationId + 1);

            oldKey = _keys[relationId];

            _keys[relationId] = relationKey;
        }

        if (oldKey is not null)
            RelationsMeta.GetRelationType(relationId).PoolRelationKey(oldKey);
    }

    internal RelationKey? DeleteKey(int relationId)
    {
        lock (_lock)
        {
            RelationKey[] keys = _keys;

            if ((uint)relationId >= (uint)keys.Length)
                return null;

            RelationKey key = keys[relationId];

            if (key is not null)
                keys[relationId] = null!;

            return key;
        }
    }
}
