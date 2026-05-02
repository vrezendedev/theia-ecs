using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Adds a <b>tag relation (fieldless)</b> linking <paramref name="owner"/> to <paramref name="target"/>
    /// under <typeparamref name="TRelation"/>. Returns <see langword="true"/> if the relation was
    /// established, <see langword="false"/> if it was rejected (either entity dead, owner equals
    /// target, or the link already exists).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TRelation"/> is a data relation; use <see cref="TryAddEvaluatedRelation"/> instead.</exception>
    public bool TryAddTagRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        int relationId = RelationMeta<TRelation>.s_id;
        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        ThrowIfExpectedTagRelation(relationType);

        RelationAccounted relationAccounted = AttemptAccountRelation(relationId, owner, target);

        if (!relationAccounted._accounted)
            return false;

        RelationLinked relationLinked = TryRelate(owner, target, relationAccounted);

        if (relationLinked._linked)
            RelationsEvents.InvokeOnRelationAdded(
                new RelationModified(this, owner, target, relationType._type, relationId)
            );

        return relationLinked._linked;
    }

    /// <summary>
    /// Deserialization-only path that establishes a relation link without validating entity
    /// liveness, tag/evaluated category, or firing <see cref="RelationsEvents"/>. Used to restore a
    /// relation graph from a serialized world where the standard guards would either fire spurious
    /// events on a not-yet-fully-loaded world or reject links pointing at entities being rehydrated.
    /// </summary>
    /// <remarks>
    /// <b>Do not call from gameplay code.</b> The public <see cref="TryAddTagRelation"/> and
    /// <see cref="TryAddEvaluatedRelation"/> paths exist for that purpose and enforce the
    /// invariants this method skips.
    /// </remarks>
    internal Relation UnrestrictedAddRelation(int relationId, Entity owner, Entity target)
    {
        RelationAccounted relationAccounted = AttemptAccountRelation(relationId, owner, target);
        TryRelate(owner, target, relationAccounted);
        return relationAccounted._relation;
    }

    /// <summary>
    /// Adds a <b>data relation</b> linking <paramref name="owner"/> to <paramref name="target"/> under
    /// <typeparamref name="TRelation"/> with <paramref name="value"/> as the per-link payload.
    /// Returns <see langword="true"/> if the relation was established, <see langword="false"/>
    /// otherwise.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TRelation"/> is a tag relation; use <see cref="TryAddTagRelation"/> instead.</exception>
    public bool TryAddEvaluatedRelation<TRelation>(
        Entity owner,
        Entity target,
        TRelation value = default
    )
        where TRelation : struct
    {
        int relationId = RelationMeta<TRelation>.s_id;
        RelationType relationType = RelationsMeta.GetRelationType(relationId);

        ThrowIfExpectedEvaluatedRelation(relationType);

        RelationAccounted relationAccounted = AttemptAccountRelation(relationId, owner, target);

        if (!relationAccounted._accounted)
            return false;

        bool linked = TryRelateEvaluated(owner, target, in value, relationAccounted);

        if (linked)
            RelationsEvents.InvokeOnRelationAdded(
                new RelationModified(this, owner, target, relationType._type, relationId)
            );

        return linked;
    }

    /// <summary>
    /// Removes every <typeparamref name="TRelation"/> link <paramref name="owner"/> currently
    /// owns. Each removed link fires <see cref="RelationsEvents.OnAnyRelationRemoved"/>.
    /// Returns <see langword="false"/> if the entity is dead or has no relations of this type.
    /// </summary>
    public bool TryRemoveRelation<TRelation>(Entity owner)
        where TRelation : struct => AttemptRemoveRelation(RelationMeta<TRelation>.s_id, owner);

    /// <summary>
    /// Removes the specific <typeparamref name="TRelation"/> link from <paramref name="owner"/>
    /// to <paramref name="target"/>. Returns <see langword="false"/> if either entity is dead,
    /// they are the same entity, or no such link exists.
    /// </summary>
    public bool TryRemoveRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct =>
        AttemptRemoveRelation(RelationMeta<TRelation>.s_id, owner, target);

    /// <summary>Returns <see langword="true"/> if <paramref name="owner"/> currently owns at least one <typeparamref name="TRelation"/> link.</summary>
    public bool HasRelation<TRelation>(Entity owner)
        where TRelation : struct => CountRelations<TRelation>(owner) > 0;

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="owner"/> currently has a
    /// <typeparamref name="TRelation"/> link pointing at <paramref name="target"/>. Implements
    /// the canonical "is A related to B?" check via the bilateral lookup: O(1) on the owner's
    /// <see cref="RelationsIndexer"/> followed by O(1) on the target's <see cref="RelationLink"/>.
    /// </summary>
    public bool IsRelatedTo<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        if (!IsAlive(owner) || !IsAlive(target) || (owner == target))
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

        lock (ownerIndexer._lock)
        {
            if (!ownerIndexer.HasKey(relationId))
                return false;

            primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();
        }

        RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);

        lock (targetIndexer._lock)
        {
            if (!targetIndexer.HasLink(relationId))
                return false;

            RelationLink relationLink = targetIndexer.GetOrRentLink(relationId);

            return relationLink.HasExternalLink(primaryKey);
        }
    }

    /// <summary>
    /// Returns the <b>number of <typeparamref name="TRelation"/> targets <paramref name="owner"/>
    /// currently points at</b>. Returns <c>-1</c> if the entity is dead, <c>0</c> if alive but has
    /// no relations of this type.
    /// </summary>
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

        lock (ownerIndexer._lock)
        {
            if (!ownerIndexer.HasKey(relationId))
                return 0;

            int primaryKey = ownerIndexer.GetRelationKey(relationId).GetPrimaryKey();

            Relation relation = GetOrCreateRelationStorage(relationId).GetRelation(primaryKey);

            return relation.GetRelatedToCount();
        }
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="entity"/> is currently a target of at least one <typeparamref name="TRelation"/> link from any owner.</summary>
    public bool HasExternalLinks<TRelation>(Entity entity)
        where TRelation : struct => CountExternalLinks<TRelation>(entity) > 0;

    /// <summary>
    /// Returns the <b>number of owners currently pointing at <paramref name="entity"/> under
    /// <typeparamref name="TRelation"/></b>. Returns <c>-1</c> if the entity is dead, <c>0</c> if
    /// alive but not the target of any link of this type.
    /// </summary>
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

        lock (relationsIndexer._lock)
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

    /// <summary>
    /// Iterates every target of <paramref name="owner"/>'s <typeparamref name="TRelation"/>
    /// links under <paramref name="query"/>, <b>holding the relation's update lock for the duration</b>.
    /// <br/>
    /// Use for tag relations or when the per-link payload isn't needed.
    /// </summary>
    public void QueryRelation<TRelation, TQueryRelation>(Entity owner, ref TQueryRelation query)
        where TRelation : struct
        where TQueryRelation : struct, IQueryRelation, allows ref struct =>
        GetEntityRelation(RelationMeta<TRelation>.s_id, owner)._relation.Query(ref query);

    /// <summary>
    /// Iterates every (target, payload) pair of <paramref name="owner"/>'s
    /// <typeparamref name="TRelation"/> links under <paramref name="update"/>, with the payload
    /// supplied by reference for <b>in-place mutation</b>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TRelation"/> is a tag relation; use <see cref="QueryRelation"/> instead.</exception>
    public void QueryEvaluatedRelation<TRelation, TQueryRelation>(
        Entity owner,
        ref TQueryRelation update
    )
        where TRelation : struct
        where TQueryRelation : struct, IQueryEvaluatedRelation<TRelation>, allows ref struct
    {
        int relationdId = RelationMeta<TRelation>.s_id;
        RelationType relationType = RelationsMeta.GetRelationType(relationdId);

        ThrowIfExpectedEvaluatedRelation(relationType);

        (
            (EvaluatedRelation<TRelation>)GetEntityRelation(relationdId, owner)._relation
        ).QueryEvaluated(ref update);
    }

    /// <summary>
    /// Returns a span over the target entities <paramref name="owner"/> currently points at under
    /// <typeparamref name="TRelation"/>. The span aliases the underlying storage.
    /// </summary>
    /// <remarks>
    /// <b>Not thread-safe:</b> concurrent structural changes to this relation while the span is in
    /// use produce undefined behavior; the caller is responsible for ensuring no other thread adds
    /// or removes <typeparamref name="TRelation"/> links from <paramref name="owner"/> for the
    /// span's lifetime.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <paramref name="owner"/> is dead or has no <typeparamref name="TRelation"/>
    /// relations. The caller is responsible for ensuring the relation exists; check via
    /// <see cref="HasRelation"/> first when the presence is not already known.
    /// </exception>
    public ReadOnlySpan<Entity> GetRelations<TRelation>(Entity owner)
        where TRelation : struct =>
        GetEntityRelation(RelationMeta<TRelation>.s_id, owner)._relation.To();

    /// <summary>
    /// Returns a reference to the <typeparamref name="TRelation"/> payload on the link from
    /// <paramref name="owner"/> to <paramref name="target"/>. <b>Mutations through the reference
    /// persist in storage</b>.
    /// </summary>
    /// <remarks>
    /// <b>Not thread-safe:</b> concurrent structural changes to this relation while the reference
    /// is in use produce undefined behavior; the caller is responsible for ensuring no other thread
    /// removes the link or modifies the surrounding relation for the reference's lifetime.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <typeparamref name="TRelation"/> is a tag relation, if <paramref name="owner"/>
    /// equals <paramref name="target"/>, if either entity is dead, or if no link from
    /// <paramref name="owner"/> to <paramref name="target"/> exists. The caller is responsible for
    /// ensuring the link exists; check via <see cref="IsRelatedTo"/> first when the presence is not
    /// already known.
    /// </exception>
    public ref TRelation GetEvaluatedRelation<TRelation>(Entity owner, Entity target)
        where TRelation : struct
    {
        int relationdId = RelationMeta<TRelation>.s_id;
        RelationType relationType = RelationsMeta.GetRelationType(relationdId);

        ThrowIfExpectedEvaluatedRelation(relationType);

        if (owner == target)
            ThrowTargetEqualsOwner();

        RelationKeyed relationKeyed = GetEntityRelation(relationdId, owner);
        int compositeKey = GetEntityCompositeKey(relationdId, relationKeyed._primaryKey, target);

        return ref ((EvaluatedRelation<TRelation>)relationKeyed._relation).Get(compositeKey);
    }

    /// <summary>
    /// Returns the targets and per-link payloads of <paramref name="owner"/>'s
    /// <typeparamref name="TRelation"/> links as a parallel-span pair via
    /// <see cref="EntityEvaluatedRelations{T}"/>. Use when iterating both sides together is more
    /// convenient than the callback shape on <see cref="QueryEvaluatedRelation"/>.
    /// </summary>
    /// <remarks>
    /// <b>Not thread-safe:</b> both spans alias the underlying storage; concurrent structural
    /// changes to this relation while the spans are in use produce undefined behavior. The caller
    /// is responsible for ensuring no other thread adds or removes <typeparamref name="TRelation"/>
    /// links from <paramref name="owner"/> for the spans' lifetime.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <typeparamref name="TRelation"/> is a tag relation, if <paramref name="owner"/>
    /// is dead, or if <paramref name="owner"/> has no <typeparamref name="TRelation"/> relations.
    /// The caller is responsible for ensuring the relation exists; check via
    /// <see cref="HasRelation"/> first when the presence is not already known.
    /// </exception>
    public EntityEvaluatedRelations<TRelation> GetEvaluatedRelations<TRelation>(Entity owner)
        where TRelation : struct
    {
        int relationdId = RelationMeta<TRelation>.s_id;
        RelationType relationType = RelationsMeta.GetRelationType(relationdId);

        ThrowIfExpectedEvaluatedRelation(relationType);

        RelationKeyed relationKeyed = GetEntityRelation(relationdId, owner);

        return new EntityEvaluatedRelations<TRelation>(
            relationKeyed._relation.To(),
            ((EvaluatedRelation<TRelation>)relationKeyed._relation).Get()
        );
    }

    /// <summary>
    /// Returns a <b>span over the inverse view</b>: every owner currently pointing at
    /// <paramref name="entity"/> under <typeparamref name="TRelation"/>. Each
    /// <see cref="ExternalLink"/> implicitly converts to the owning <see cref="Entity"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the entity is dead or has no inverse links of this type.</exception>
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

        lock (relationsIndexer._lock)
        {
            if (!relationsIndexer.HasLink(relationId))
                ThrowEntityMissingRelationLink(relationId, entity);

            RelationLink relationLink = relationsIndexer.GetOrRentLink(relationId);

            return relationLink.GetExternalLinks();
        }
    }

    private RelationAccounted AttemptAccountRelation(int relationId, Entity owner, Entity target)
    {
        if (!IsAlive(owner) || !IsAlive(target) || (owner == target))
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

    private RelationLinked TryRelate(
        Entity owner,
        Entity target,
        RelationAccounted relationAccounted
    )
    {
        lock (relationAccounted._relation._lock)
        {
            if (relationAccounted._relation.GetOwner() != owner)
                return RelationLinked.Reproved;

            lock (relationAccounted._targetRelationLink._lock)
            {
                if (
                    relationAccounted._targetRelationLink.HasExternalLink(
                        relationAccounted._primaryKey
                    )
                )
                    return RelationLinked.Reproved;

                int compositeKey = relationAccounted._relation.Relate(target);

                relationAccounted._targetRelationLink.AddExternalLink(
                    owner,
                    relationAccounted._primaryKey,
                    compositeKey
                );

                return new RelationLinked(
                    true,
                    relationAccounted._relation,
                    relationAccounted._primaryKey,
                    compositeKey
                );
            }
        }
    }

    private bool TryRelateEvaluated<TRelation>(
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
                if (
                    relationAccounted._targetRelationLink.HasExternalLink(
                        relationAccounted._primaryKey
                    )
                )
                    return false;

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

        Type type = RelationsMeta.GetRelationType(relationId)._type;

        for (int i = 0; i < targets.Length; i++)
        {
            Entity target = targets[i];
            ref EntityMeta targetMeta = ref _entitiesMeta[target._id];
            RelationsIndexer targetIndexer = GetOrRentRelationIndexer(ref targetMeta);
            RelationLink targetRelationLink;

            lock (targetIndexer._lock)
            {
                targetRelationLink = targetIndexer.GetOrRentLink(relationId);
            }

            lock (relationStorage._lock)
            {
                lock (targetRelationLink._lock)
                {
                    if (!targetRelationLink.HasExternalLink(primaryKey))
                        continue;

                    targetRelationLink.RemoveExternalLink(primaryKey);
                }
            }

            RelationsEvents.InvokeOnRelationRemoved(
                new RelationModified(this, owner, target, type, relationId)
            );
        }

        lock (relationStorage._lock)
        {
            relationStorage.ReturnRelation(primaryKey);
        }

        return true;
    }

    private bool AttemptRemoveRelation(int relationId, Entity owner, Entity target)
    {
        if (!IsAlive(owner) || !IsAlive(target) || (owner == target))
            return false;

        ref EntityMeta ownerMeta = ref _entitiesMeta[owner._id];
        ref EntityMeta targetMeta = ref _entitiesMeta[target._id];

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

        lock (targetIndexer._lock)
        {
            if (!targetIndexer.HasLink(relationId))
                return false;

            targetRelationLink = targetIndexer.GetOrRentLink(relationId);
        }

        lock (relation._lock)
        {
            lock (targetRelationLink._lock)
            {
                if (!targetRelationLink.HasExternalLink(primaryKey))
                    return false;

                compositeKey = targetRelationLink.GetCompositeKey(primaryKey);
                targetRelationLink.RemoveExternalLink(primaryKey);
            }
        }

        lock (relation._lock)
        {
            EntitySwapped entitySwapped = relation.Disrelate(compositeKey);

            if (entitySwapped._entityID != EntitySwapped.InvalidEntitySwappedIndexes)
            {
                ref EntityMeta swappedMeta = ref _entitiesMeta[entitySwapped._entityID];
                RelationsIndexer swappedIndexer = GetOrRentRelationIndexer(ref swappedMeta);
                RelationLink swappedRelationLink;

                lock (swappedIndexer._lock)
                {
                    swappedRelationLink = swappedIndexer.GetOrRentLink(relationId);
                }

                lock (swappedRelationLink._lock)
                {
                    swappedRelationLink.UpdateCompositeKey(primaryKey, compositeKey);
                }
            }
        }

        RelationsEvents.InvokeOnRelationRemoved(
            new RelationModified(
                this,
                owner,
                target,
                RelationsMeta.GetRelationType(relationId)._type,
                relationId
            )
        );

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

        lock (ownerIndexer._lock)
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

        lock (targetIndexer._lock)
        {
            if (!targetIndexer.HasLink(relationId))
                ThrowEntityMissingRelationLink(relationId, target);

            RelationLink relationLink = targetIndexer.GetOrRentLink(relationId);

            return relationLink.GetCompositeKey(primaryKey);
        }
    }

    /// <summary>
    /// Tears down every relation <paramref name="owner"/> participates in on either side, then
    /// returns its <see cref="RelationsIndexer"/> to the pool. Called as part of entity
    /// destruction so the bilateral graph stays consistent without orphaned links.
    /// </summary>
    /// <remarks>
    /// Walks the inverse links first (every owner pointing at this entity removes its link),
    /// then the forward links (this entity stops pointing at any target). Both walks fire
    /// <see cref="Events.RelationsEvents.OnAnyRelationRemoved">OnAnyRelationRemoved</see> per removed link.
    /// </remarks>
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

        ownerMeta._relationsIndexerIndex = EntityMeta.DefaultInvalidEntityMetaIndexes;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<RelationStorage> GetRelationStorages() => _relationStorages;

    private static void ThrowIfExpectedTagRelation(RelationType relationType)
    {
        if (!relationType._isTag)
            throw new InvalidOperationException(
                $"Relation '{relationType._type.Name}' is an evaluated relation. Expected a tag relation."
            );
    }

    private static void ThrowIfExpectedEvaluatedRelation(RelationType relationType)
    {
        if (relationType._isTag)
            throw new InvalidOperationException(
                $"Relation '{relationType._type.Name}' is a tag relation. Expected an evaluated relation."
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

    [DoesNotReturn]
    private static void ThrowTargetEqualsOwner() =>
        throw new InvalidOperationException($"Target must be different from Owner.");
}
