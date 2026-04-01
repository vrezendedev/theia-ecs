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

    private readonly Lock _relationsIndexersLock = new();
    private readonly Lock _relationStoragesLock = new();

    private RelationsIndexer[] _relationsIndexers;
    private Stack<int> _freeRelationSlots;

    private RelationStorage[] _relationStorages;

    private RelationAccounted AddRelation(int relationId, Entity owner, Entity target)
    {
        RelationStorage storage = GetOrCreateRelationStorage(relationId);
        Relation relation;

        ref EntityMeta ownerMeta = ref _entitiesMeta[owner._id];
        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref ownerMeta);
        int primaryKey;

        lock (ownerIndexer._lock)
        {
            if (!ownerIndexer.HasKey(relationId))
            {
                lock (storage._lock)
                {
                    RelationRented relationRented = storage.RentRelation();

                    ownerIndexer.AddKey(relationId, relationRented._primaryKey);
                    relation = relationRented._relation;
                    primaryKey = relationRented._primaryKey;
                    relation.SetOwner(owner);
                }
            }
            else
            {
                primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();
                relation = storage.GetRelation(primaryKey);
            }
        }

        ref EntityMeta targetMeta = ref _entitiesMeta[target._id];
        RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);
        RelationLink relationLink;

        lock (targetIndexer._lock)
        {
            relationLink = targetIndexer.GetOrRentLink(relationId);
        }

        return new RelationAccounted(primaryKey, relation, relationLink);
    }

    public bool TryAddRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        if (!IsAlive(owner) || !IsAlive(target))
            return false;

        int relationId = RelationMeta<TRelation>.s_id;

        ThrowIfExpectedTagRelation(relationId);

        RelationAccounted relationAccounted = AddRelation(relationId, owner, target);

        lock (relationAccounted._relation._lock)
        {
            if (relationAccounted._relation.GetOwner() != owner)
                return false;

            lock (relationAccounted._relationLink._lock)
            {
                int compositeKey = relationAccounted._relation.Relate(target);
                relationAccounted._relationLink.AddExternalLink(
                    owner,
                    relationAccounted._primaryKey,
                    compositeKey
                );
            }
        }

        return true;
    }

    public bool TryAddEvaluatedRelation<TRelation>(
        Entity owner,
        Entity target,
        TRelation value = default
    )
        where TRelation : struct
    {
        if (!IsAlive(owner) || !IsAlive(target))
            return false;

        int relationId = RelationMeta<TRelation>.s_id;

        ThrowIfExpectedEvaluatedRelation(relationId);

        RelationAccounted relationAccounted = AddRelation(relationId, owner, target);

        lock (relationAccounted._relation._lock)
        {
            if (relationAccounted._relation.GetOwner() != owner)
                return false;

            lock (relationAccounted._relationLink._lock)
            {
                int compositeKey = relationAccounted._relation.Relate(target);

                relationAccounted._relationLink.AddExternalLink(
                    owner,
                    relationAccounted._primaryKey,
                    compositeKey
                );

                ((EvaluatedRelation<TRelation>)relationAccounted._relation).Set(
                    compositeKey,
                    value
                );
            }
        }

        return true;
    }

    public bool TryRemoveRelation<TRelation>(Entity owner)
        where TRelation : struct
    {
        if (!IsAlive(owner))
            return false;

        ref EntityMeta ownerMeta = ref _entitiesMeta[owner._id];

        if (ownerMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            return false;

        int relationId = RelationMeta<TRelation>.s_id;

        RelationStorage storage = GetOrCreateRelationStorage(relationId);

        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref ownerMeta);

        int primaryKey;
        ReadOnlySpan<Entity> targets;

        lock (ownerIndexer._lock)
        {
            if (!ownerIndexer.HasKey(relationId))
                return false;

            primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();

            Relation relation = storage.GetRelation(primaryKey);
            targets = relation.To();

            ownerIndexer.RemoveRelationKey(relationId);
        }

        for (int i = 0; i < targets.Length; i++)
        {
            ref EntityMeta targetMeta = ref _entitiesMeta[targets[i]._id];
            RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);
            RelationLink relationLink;

            lock (targetIndexer._lock)
            {
                relationLink = targetIndexer.GetOrRentLink(relationId);
            }

            lock (relationLink._lock)
            {
                relationLink.RemovalExternalLink(primaryKey);
            }
        }

        lock (storage._lock)
        {
            storage.ReturnRelation(primaryKey);
        }

        return true;
    }

    public bool TryRemoveRelation<TRelation>(Entity owner, Entity entity)
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

    private RelationsIndexer GetOrRentRelationIndexer(ref EntityMeta entityMeta)
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

    private void ReturnRelationIndexer(int relationIndexerIndex)
    {
        lock (_relationsIndexersLock)
        {
            _freeRelationSlots.Push(relationIndexerIndex);
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

    private static void ThrowIfExpectedTagRelation(int relationId)
    {
        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        if (!relationType._isTag)
            throw new InvalidOperationException(
                $"Relation '{relationType._type.Name}' is an evaluated relation. Use TryAddEvaluatedRelation to add it instead."
            );
    }

    private static void ThrowIfExpectedEvaluatedRelation(int relationId)
    {
        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        if (relationType._isTag)
            throw new InvalidOperationException(
                $"Relation '{relationType._type.Name}' is a tag relation. Use TryAddRelation to add it instead."
            );
    }
}
