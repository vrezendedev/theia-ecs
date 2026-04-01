using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    public bool TryAddRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        int relationId = RelationMeta<TRelation>.s_id;

        ThrowIfExpectedTagRelation(relationId);

        RelationAccounted relationAccounted = AddRelation(relationId, owner, target);

        if (!relationAccounted._accounted)
            return false;

        return TryLinkRelation(owner, target, relationAccounted);
    }

    public bool TryAddEvaluatedRelation<TRelation>(
        Entity owner,
        Entity target,
        TRelation value = default
    )
        where TRelation : struct
    {
        int relationId = RelationMeta<TRelation>.s_id;

        ThrowIfExpectedEvaluatedRelation(relationId);

        RelationAccounted relationAccounted = AddRelation(relationId, owner, target);

        if (!relationAccounted._accounted)
            return false;

        return TryLinkEvaluatedRelation(owner, target, in value, relationAccounted);
    }

    public bool TryRemoveRelation<TRelation>(Entity owner)
        where TRelation : struct => AttemptRemoveRelation(RelationMeta<TRelation>.s_id, owner);

    public bool TryRemoveRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct =>
        AttemptRemoveRelation(RelationMeta<TRelation>.s_id, owner, target);

    public bool HasRelation<TRelation>(Entity owner)
        where TRelation : struct => CountRelations<TRelation>(owner) > 0;

    public bool IsRelatedTo<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        if (!IsAlive(owner) || !IsAlive(target))
            return false;

        ref EntityMeta entityMeta = ref _entitiesMeta[owner._id];
        ref EntityMeta targetMeta = ref _entitiesMeta[target._id];

        if (
            entityMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes
            || targetMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes
        )
            return false;

        int relationId = RelationMeta<TRelation>.s_id;

        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref entityMeta);

        int primaryKey;

        lock (ownerIndexer)
        {
            if (!ownerIndexer.HasKey(relationId))
                return false;

            primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();
        }

        RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);

        lock (targetIndexer)
        {
            if (!targetIndexer.HasLink(relationId))
                return false;

            RelationLink relationLink = targetIndexer.GetOrRentLink(relationId);

            return relationLink.HasExternalLink(primaryKey);
        }
    }

    public int CountRelations<TRelation>(Entity owner)
        where TRelation : struct
    {
        if (!IsAlive(owner))
            return -1;

        ref EntityMeta entityMeta = ref _entitiesMeta[owner._id];

        if (entityMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            return 0;

        int relationId = RelationMeta<TRelation>.s_id;

        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref entityMeta);

        lock (ownerIndexer)
        {
            if (!ownerIndexer.HasKey(relationId))
                return 0;

            int primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();

            Relation relation = GetOrCreateRelationStorage(relationId).GetRelation(primaryKey);

            return relation.GetRelatedToCount();
        }
    }

    public bool HasExternalLinks<TRelation>(Entity entity)
        where TRelation : struct => CountExternalLinks<TRelation>(entity) > 0;

    public int CountExternalLinks<TRelation>(Entity entity)
        where TRelation : struct
    {
        if (!IsAlive(entity))
            return -1;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        if (entityMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            return 0;

        int relationId = RelationMeta<TRelation>.s_id;

        RelationsIndexer relationsIndexer = GetOrRentRelationIndexer(ref entityMeta);
        RelationLink relationLink;

        lock (relationsIndexer)
        {
            if (!relationsIndexer.HasLink(relationId))
                return 0;

            relationLink = relationsIndexer.GetRelationLink(relationId);
        }

        lock (relationLink._lock)
        {
            return relationLink.GetExternalLinksCount();
        }
    }

    public void QueryRelation<TRelation>(Entity owner, QueryRelation update)
        where TRelation : struct =>
        GetEntityRelation(RelationMeta<TRelation>.s_id, owner)._relation.Query(update);

    public void QueryEvaluatedRelation<TRelation>(Entity owner, QueryRelation<TRelation> update)
        where TRelation : struct
    {
        int relationdId = RelationMeta<TRelation>.s_id;

        ThrowIfExpectedEvaluatedRelation(relationdId);

        ((EvaluatedRelation<TRelation>)GetEntityRelation(relationdId, owner)._relation).Query(
            update
        );
    }

    public ReadOnlySpan<Entity> GetRelations<TRelation>(Entity owner)
        where TRelation : struct =>
        GetEntityRelation(RelationMeta<TRelation>.s_id, owner)._relation.To();

    public ref TRelation GetEvaluatedRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        int relationdId = RelationMeta<TRelation>.s_id;

        ThrowIfExpectedEvaluatedRelation(relationdId);

        RelationKeyed relationKeyed = GetEntityRelation(relationdId, owner);
        int compositeKey = GetEntityCompositeKey(relationdId, relationKeyed._primaryKey, target);

        return ref ((EvaluatedRelation<TRelation>)relationKeyed._relation).Get(compositeKey);
    }

    public EntityEvaluatedRelations<TRelation> GetEvaluatedRelations<TRelation>(Entity owner)
        where TRelation : struct
    {
        int relationdId = RelationMeta<TRelation>.s_id;

        ThrowIfExpectedEvaluatedRelation(relationdId);

        RelationKeyed relationKeyed = GetEntityRelation(relationdId, owner);

        return new EntityEvaluatedRelations<TRelation>(
            relationKeyed._relation.To(),
            ((EvaluatedRelation<TRelation>)relationKeyed._relation).Get()
        );
    }

    public ReadOnlySpan<ExternalLink> GetExternalLinks<TRelation>(Entity entity)
        where TRelation : struct
    {
        if (!IsAlive(entity))
            ThrowEntityNotAlive(entity);

        int relationId = RelationMeta<TRelation>.s_id;

        ref EntityMeta entityMeta = ref _entitiesMeta[entity._id];

        if (entityMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            ThrowEntityMissingRelationLink(relationId, entity);

        RelationsIndexer relationsIndexer = GetOrRentRelationIndexer(ref entityMeta);

        lock (relationsIndexer)
        {
            if (!relationsIndexer.HasLink(relationId))
                ThrowEntityMissingRelationLink(relationId, entity);

            RelationLink relationLink = relationsIndexer.GetOrRentLink(relationId);

            return relationLink.GetExternalLinks();
        }
    }

    private RelationAccounted AddRelation(int relationId, Entity owner, Entity target)
    {
        if (!IsAlive(owner) || !IsAlive(target))
            return RelationAccounted.Reproved;

        RelationStorage relationStorage = GetOrCreateRelationStorage(relationId);
        Relation relation;

        ref EntityMeta ownerMeta = ref _entitiesMeta[owner._id];
        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref ownerMeta);
        int primaryKey;

        lock (ownerIndexer._lock)
        {
            if (!ownerIndexer.HasKey(relationId))
            {
                lock (relationStorage._lock)
                {
                    RelationKeyed relationKeyed = relationStorage.RentRelation();

                    ownerIndexer.AddKey(relationId, relationKeyed._primaryKey);
                    relation = relationKeyed._relation;
                    primaryKey = relationKeyed._primaryKey;
                    relation.SetOwner(owner);
                }
            }
            else
            {
                primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();
                relation = relationStorage.GetRelation(primaryKey);
            }
        }

        ref EntityMeta targetMeta = ref _entitiesMeta[target._id];
        RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);
        RelationLink targetRelationLink;

        lock (targetIndexer._lock)
        {
            targetRelationLink = targetIndexer.GetOrRentLink(relationId);
        }

        return new RelationAccounted(true, primaryKey, relation, targetRelationLink);
    }

    private bool TryLinkRelation(Entity owner, Entity target, RelationAccounted relationAccounted)
    {
        lock (relationAccounted._relation._lock)
        {
            if (relationAccounted._relation.GetOwner() != owner)
                return false;

            lock (relationAccounted._targetRelationLink._lock)
            {
                int compositeKey = relationAccounted._relation.Relate(target);
                relationAccounted._targetRelationLink.AddExternalLink(
                    owner,
                    relationAccounted._primaryKey,
                    compositeKey
                );

                return true;
            }
        }
    }

    private bool TryLinkEvaluatedRelation<TRelation>(
        Entity owner,
        Entity target,
        in TRelation value,
        RelationAccounted relationAccounted
    )
        where TRelation : struct
    {
        lock (relationAccounted._relation._lock)
        {
            if (relationAccounted._relation.GetOwner() != owner)
                return false;

            lock (relationAccounted._targetRelationLink._lock)
            {
                int compositeKey = relationAccounted._relation.Relate(target);

                relationAccounted._targetRelationLink.AddExternalLink(
                    owner,
                    relationAccounted._primaryKey,
                    compositeKey
                );

                ((EvaluatedRelation<TRelation>)relationAccounted._relation).Set(
                    compositeKey,
                    value
                );

                return true;
            }
        }
    }

    private bool AttemptRemoveRelation(int relationId, Entity owner)
    {
        if (!IsAlive(owner))
            return false;

        ref EntityMeta ownerMeta = ref _entitiesMeta[owner._id];

        if (ownerMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            return false;

        RelationStorage relationStorage = GetOrCreateRelationStorage(relationId);

        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref ownerMeta);

        int primaryKey;
        ReadOnlySpan<Entity> targets;

        lock (ownerIndexer._lock)
        {
            if (!ownerIndexer.HasKey(relationId))
                return false;

            primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();

            Relation relation = relationStorage.GetRelation(primaryKey);
            targets = relation.To();

            ownerIndexer.RemoveRelationKey(relationId);
        }

        for (int i = 0; i < targets.Length; i++)
        {
            ref EntityMeta targetMeta = ref _entitiesMeta[targets[i]._id];
            RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);
            RelationLink targetRelationLink;

            lock (targetIndexer._lock)
            {
                targetRelationLink = targetIndexer.GetOrRentLink(relationId);
            }

            lock (targetRelationLink._lock)
            {
                if (!targetRelationLink.HasExternalLink(primaryKey))
                    continue;

                targetRelationLink.RemoveExternalLink(primaryKey);
            }
        }

        lock (relationStorage._lock)
        {
            relationStorage.ReturnRelation(primaryKey);
        }

        return true;
    }

    private bool AttemptRemoveRelation(int relationId, Entity owner, Entity target)
    {
        if (!IsAlive(owner) || !IsAlive(target))
            return false;

        ref EntityMeta ownerMeta = ref _entitiesMeta[owner._id];
        ref EntityMeta targetMeta = ref _entitiesMeta[owner._id];

        if (
            ownerMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes
            || targetMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes
        )
            return false;

        RelationStorage relationStorage = GetOrCreateRelationStorage(relationId);

        Relation relation;

        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref ownerMeta);

        RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);
        RelationLink targetRelationLink;

        int primaryKey;
        int compositeKey;

        lock (ownerIndexer._lock)
        {
            if (!ownerIndexer.HasKey(relationId))
                return false;

            primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();

            relation = relationStorage.GetRelation(primaryKey);
        }

        lock (targetIndexer)
        {
            if (!targetIndexer.HasLink(relationId))
                return false;

            targetRelationLink = targetIndexer.GetOrRentLink(relationId);
        }

        lock (targetRelationLink._lock)
        {
            if (!targetRelationLink.HasExternalLink(primaryKey))
                return false;

            compositeKey = targetRelationLink.GetCompositeKey(primaryKey);
            targetRelationLink.RemoveExternalLink(primaryKey);
        }

        lock (relation._lock)
        {
            EntitySwapped entitySwapped = relation.Disrelate(compositeKey);

            if (entitySwapped._entityID != EntitySwapped.InvalidEntitySwappedIndexes)
            {
                ref EntityMeta swappedMeta = ref _entitiesMeta[entitySwapped._entityID];
                RelationsIndexer swappedIndexer = GetOrRentRelationIndexer(ref targetMeta);

                lock (swappedIndexer)
                {
                    targetRelationLink = targetIndexer.GetOrRentLink(relationId);
                }

                lock (targetRelationLink._lock)
                {
                    targetRelationLink.UpdateCompositeKey(primaryKey, compositeKey);
                }
            }
        }

        return true;
    }

    private RelationKeyed GetEntityRelation(int relationId, Entity owner)
    {
        if (!IsAlive(owner))
            ThrowEntityNotAlive(owner);

        ref EntityMeta entityMeta = ref _entitiesMeta[owner._id];

        if (entityMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            ThrowEntityMissingRelation(relationId, owner);

        RelationStorage relationStorage = GetOrCreateRelationStorage(relationId);

        RelationsIndexer ownerIndexer = GetOrRentRelationIndexer(ref entityMeta);

        lock (ownerIndexer)
        {
            if (!ownerIndexer.HasKey(relationId))
                ThrowEntityMissingRelation(relationId, owner);

            int primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();
            Relation relation = relationStorage.GetRelation(primaryKey);

            return new RelationKeyed(relation, primaryKey);
        }
    }

    private int GetEntityCompositeKey(int relationId, int primaryKey, Entity target)
    {
        if (!IsAlive(target))
            ThrowEntityNotAlive(target);

        ref EntityMeta entityMeta = ref _entitiesMeta[target._id];

        if (entityMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            ThrowEntityMissingRelationLink(relationId, target);

        RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref entityMeta);

        lock (targetIndexer)
        {
            if (!targetIndexer.HasLink(relationId))
                ThrowEntityMissingRelationLink(relationId, target);

            RelationLink relationLink = targetIndexer.GetOrRentLink(relationId);

            return relationLink.GetCompositeKey(primaryKey);
        }
    }

    private void ResetRelations(Entity owner)
    {
        ref EntityMeta ownerMeta = ref _entitiesMeta[owner._id];

        if (ownerMeta._relationsIndexerIndex == EntityMeta.DefaultInvalidEntityMetaIndexes)
            return;

        int relationIndexerIndex = ownerMeta._relationsIndexerIndex;

        RelationsIndexer ownerIndexer = _relationsIndexers[relationIndexerIndex];

        while (ownerIndexer.GetAddedLinksCount() > 0)
        {
            int relationId = ownerIndexer.GetAddedLinksAt(0);

            RelationLink link = ownerIndexer.GetRelationLink(relationId);

            while (link.GetExternalLinksCount() > 0)
            {
                ExternalLink externalLink = link.GetExternalLinkAt(0);
                AttemptRemoveRelation(relationId, externalLink.Entity, owner);
            }

            ownerIndexer.ReturnLink(relationId);
        }

        while (ownerIndexer.GetAddedRelationsCount() > 0)
        {
            int relationId = ownerIndexer.GetAddedRelationsAt(0);
            AttemptRemoveRelation(relationId, owner);
        }

        ReturnRelationIndexer(relationIndexerIndex);
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

    [DoesNotReturn]
    private static void ThrowEntityMissingRelation(int relationId, Entity entity) =>
        throw new InvalidOperationException(
            $"{entity} does not have relation '{RelationsMeta.GetRelationType(relationId)._type.Name}'. Add the relation before attempting to access it."
        );

    [DoesNotReturn]
    private static void ThrowEntityMissingRelationLink(int relationId, Entity entity) =>
        throw new InvalidOperationException(
            $"{entity} does not have relation '{RelationsMeta.GetRelationType(relationId)._type.Name}' external links."
        );
}
