using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Theia.ECS.Entities;
using Theia.ECS.Relations;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private const int DefaultRelationsIndexerCapacity = 16;
    private const int DefaultRelationsIndexerGrowthFactor = 2;

    private RelationsIndexer[] _relationsIndexers;
    private Stack<int> _freeRelationSlots;
    private Lock _relationsIndexersLock = new();

    private RelationStorage[] _relationStorages;
    private Lock _relationStoragesLock = new();

    private Relation? AttemptAddRelation(int relationId, Entity relationOwner, Entity target)
    {
        if (!IsAlive(relationOwner) || !IsAlive(target))
            return null;

        RelationStorage relationStorage = GetOrCreateRelationStorage(relationId);

        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        ref EntityMeta ownerMeta = ref _entitiesMeta[relationOwner._id];
        RelationsIndexer ownerIndexer = GetOrCreateRelationIndexer(ref ownerMeta);
        RelationKey ownerKey = ownerIndexer.GetOrAddKey(relationId);

        ref EntityMeta targetMeta = ref _entitiesMeta[target._id];
        RelationsIndexer targetIndexer = GetOrCreateRelationIndexer(ref targetMeta);
        RelationKey targetKey = targetIndexer.GetOrAddKey(relationId);

#pragma warning disable CS8524
        return relationType._cardinality switch
        {
            RelationCardinality.Exclusive => AddExclusiveRelation(
                relationStorage,
                relationOwner,
                (ExclusiveKey)ownerKey,
                target,
                (ExclusiveKey)targetKey
            ),
            RelationCardinality.Tree => AddOrUpdateTreeRelation(
                relationStorage,
                (TreeKey)ownerKey,
                (TreeKey)targetKey
            ),
            RelationCardinality.Multiple => AddOrUpdateMultipleRelation(
                relationStorage,
                (MultipleKey)ownerKey,
                (MultipleKey)targetKey
            ),
        };
#pragma warning restore CS8524
    }

    private Relation? AddExclusiveRelation(
        RelationStorage relationStorage,
        Entity owner,
        ExclusiveKey ownerKey,
        Entity target,
        ExclusiveKey targetKey
    )
    {
        if (!ownerKey.IsAvailable())
            ThrowEntityAlreadyHasExclusiveRelation(owner, relationStorage._relationId);

        int primaryKey = relationStorage.AccountRelation();

        Singular relation = (Singular)relationStorage.GetRelation(primaryKey);

        relation.SetOwner(owner);
        relation.Relate(target);

        ownerKey._primaryKey = primaryKey;
        targetKey.AddKeyIndexer(owner, primaryKey);

        return relation;
    }

    private Relation? AddOrUpdateTreeRelation(
        RelationStorage relationStorage,
        TreeKey ownerKey,
        TreeKey targetKey
    )
    {
        throw new NotImplementedException();
    }

    private Relation? AddOrUpdateMultipleRelation(
        RelationStorage relationStorage,
        MultipleKey ownerKey,
        MultipleKey targetKey
    )
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

            if (relationsIndexer is not null)
                relationsIndexer.Reset();
            else
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
        RelationStorage[] relationStorages = _relationStorages;

        if (relationId < relationStorages.Length && relationStorages[relationId] is not null)
            return relationStorages[relationId];

        lock (_relationStoragesLock)
        {
            if (relationId >= _relationStorages.Length)
                Array.Resize(ref _relationStorages, relationId + 1);

            if (_relationStorages[relationId] is null)
                _relationStorages[relationId] = new RelationStorage(relationId);

            return _relationStorages[relationId];
        }
    }

    [DoesNotReturn]
    private static void ThrowEntityAlreadyHasExclusiveRelation(Entity entity, int relationId) =>
        throw new InvalidOperationException(
            $"Entity {entity} already has an exclusive relation of type '{RelationsMeta.GetRelationType(relationId)._type.Name}'. Remove the existing relation before adding another."
        );
}
