using System;
using System.Collections.Generic;
using System.Threading;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Relations;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private const int DefaultRelationsIndexerCapacity = 16;
    private const int DefaultRelationsIndexerGrowthFactor = 2;

    private RelationsIndexer[] _relationsIndexers;
    private Stack<int> _freeRelationSlots;
    private readonly Lock _relationsIndexersLock = new();

    private RelationStorage[] _relationStorages;
    private readonly Lock _relationStoragesLock = new();

    public bool TryAddRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public bool TryAddEvaluatedRelation<TRelation>(
        Entity owner,
        Entity target,
        TRelation value = default
    )
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public bool TryRemoveRelation<TRelation>(Entity entityOwner)
    {
        throw new NotImplementedException();
    }

    public bool TryRemoveRelation<TRelation>(Entity entityOwner, Entity entity)
    {
        throw new NotImplementedException();
    }

    public bool HasRelation<TRelation>(Entity owner)
    {
        throw new NotImplementedException();
    }

    public bool AreRelated<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public int CountRelations<TRelation>(Entity owner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<Entity> GetRelations<TRelation>(Entity owner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public ref TRelation GetEvaluatedRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public EntityEvaluatedRelations<TRelation> GetEvaluatedRelations<TRelation>(Entity owner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public bool HasExternalLinks<TRelation>(Entity entity)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<ExternalLink> GetExternalLinks<TRelation>(Entity entity)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public void UpdateRelation(Entity owner, UpdateRelation update)
    {
        throw new NotImplementedException();
    }

    public void UpdateRelation<TRelation>(Entity owner, UpdateRelation<TRelation> update)
    {
        throw new NotImplementedException();
    }

    private RelationsIndexer GetOrCreateRelationIndexer(ref EntityMeta entityMeta)
    {
        lock (_relationsIndexersLock)
        {
            int index = entityMeta._relationsIndexerIndex;

            if (index != EntityMeta.DefaultInvalidEntityMetaIndexes)
                return _relationsIndexers[index];

            Stack<int> free = _freeRelationSlots;

            int currentLength = _relationsIndexers.Length;

            bool pop = free.Count > 0;

            index = pop ? free.Pop() : currentLength;

            RelationsIndexer relationsIndexer;

            if (!pop)
            {
                int targetLength = currentLength * DefaultRelationsIndexerGrowthFactor;

                Array.Resize(ref _relationsIndexers, targetLength);

                for (int i = targetLength - 1; i > currentLength; i--)
                    free.Push(i);
            }

            relationsIndexer = _relationsIndexers[index];

            if (relationsIndexer is null)
            {
                relationsIndexer = new();
                _relationsIndexers[index] = relationsIndexer;
            }

            entityMeta._relationsIndexerIndex = index;

            return relationsIndexer;
        }
    }

    private RelationStorage GetOrCreateRelationStorage(int relationId)
    {
        lock (_relationStoragesLock)
        {
            if (relationId < _relationStorages.Length && _relationStorages[relationId] is not null)
                return _relationStorages[relationId];

            if (relationId >= _relationStorages.Length)
                Array.Resize(ref _relationStorages, relationId + 1);

            if (_relationStorages[relationId] is null)
                _relationStorages[relationId] = new RelationStorage(relationId);

            return _relationStorages[relationId];
        }
    }
}
