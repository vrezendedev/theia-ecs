using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Theia.ECS.Relations;

internal sealed class RelationsIndexer
{
    private const int DefaultRegisteredCapacity = 16;
    private const int DefaultRegisteredGrowthFactor = 2;

    private RelationKey[] _keys;

    private readonly Lock _lock = new();

    //Versioning for handling invalid indexers!
    //Just save the entity metadata version or the entity itself when it was created!

    internal RelationsIndexer()
    {
        _keys = Array.Empty<RelationKey>();
    }

    internal RelationKey GetKey(int relationId) => _keys[relationId];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasKey(int relationId)
    {
        if (relationId >= _keys.Length)
            return false;

        return _keys[relationId] is not null;
    }

    internal void AddKey(int relationId, RelationKey relationKey)
    {
        lock (_lock)
        {
            int maxRelationId = _keys.Length;

            if (maxRelationId == 0 || relationId >= maxRelationId)
                Array.Resize(ref _keys, relationId + 1);

            _keys[relationId] = relationKey;
        }
    }

    internal void DeleteKey(int relationId) => _keys[relationId] = null!;
}
