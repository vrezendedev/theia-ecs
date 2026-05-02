using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Contracts;

namespace Theia.ECS.Relations;

/// <summary>
/// Per-relation-type pool of <see cref="Relation"/> instances. Each entity that participates as
/// an owner under this relation type holds one slot here; <see cref="RentRelation"/> hands out
/// fresh slots on demand and <see cref="ReturnRelation"/> recycles them when the owner stops
/// participating.
/// </summary>
/// <remarks>
/// The slot index returned by <see cref="RentRelation"/> is the <see cref="RelationKey._primaryKey"/>
/// stored in the owner's <see cref="RelationsIndexer"/>: <b>a single integer locates the owner's
/// relation data among potentially hundreds of participating entities, in O(1)</b>.
/// </remarks>
internal sealed class RelationStorage
{
    private const int DefaultRelationsCapacity = 8;
    private const int DefaultRelationsGrowthFactor = 2;

    internal readonly Lock _lock = new();
    internal readonly int _relationId;

    private Relation[] _relations;
    private Queue<int> _free;

    /// <summary>Returns the number of slots currently held by entities (capacity minus free-list size).</summary>
    internal int CountStorageSlotsOccupied() => _relations.Length - _free.Count;

    internal RelationStorage(int relationId)
    {
        _relationId = relationId;

        _relations = new Relation[DefaultRelationsCapacity];

        _free = new(DefaultRelationsCapacity);

        for (int i = 0; i < DefaultRelationsCapacity; i++)
            _free.Enqueue(i);
    }

    /// <summary>Returns the <see cref="Relation"/> stored at <paramref name="primaryKey"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Relation GetRelation(int primaryKey)
    {
        Relation[] relations = _relations;
        return relations[primaryKey];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Relation[] GetRelations() => _relations;

    /// <summary>
    /// Allocates the next free slot, populates it with a pooled <see cref="Relation"/>, and
    /// returns the (relation, primary-key) pair. Grows the underlying array when the free list
    /// is empty, seeding the new range into the free list.
    /// </summary>
    internal RelationKeyed RentRelation()
    {
        int index;

        if (_free.Count > 0)
            index = _free.Dequeue();
        else
        {
            index = _relations.Length;

            int targetLength = index * DefaultRelationsGrowthFactor;

            Array.Resize(ref _relations, targetLength);

            for (int i = index + 1; i < targetLength; i++)
                _free.Enqueue(i);
        }

        _relations[index] = RelationsMeta.GetRelationType(_relationId).CreateRelation();

        return new RelationKeyed(_relations[index], index);
    }

    /// <summary>
    /// Returns the <see cref="Relation"/> at <paramref name="primaryKey"/> to the relation
    /// type's pool and frees the slot. The caller must already have unwound the relation's
    /// links via the inverse-side <see cref="RelationsIndexer"/>.
    /// </summary>
    internal void ReturnRelation(int primaryKey)
    {
        Relation relation = _relations[primaryKey];

        RelationsMeta.GetRelationType(_relationId).PoolRelation(relation);

        _relations[primaryKey] = null!;

        _free.Enqueue(primaryKey);
    }
}
