using System;
using System.Collections.Generic;
using System.Numerics;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Relations;

namespace Theia.ECS.Worlds;

/// <summary>
/// <b>Root container for an ECS instance</b>: owns every archetype, assemblage, query, unique, resource,
/// relation storage, deferred-command queue, and event hub. Entities live within a single world
/// and cannot cross world boundaries; <b>multi-world setups must
/// instantiate independent <see cref="World">Worlds</see></b>.
/// </summary>
/// <remarks>
/// <para>
/// The world is the unit of state ownership in Theia. All structural changes, queries, and
/// event subscriptions are scoped to a specific instance, and disposing it tears down the
/// associated event wiring. Pooled resources held inside the world live as long as the world does.
/// </para>
/// <para>
/// <see cref="World"/> is split across many partial files by responsibility: entities,
/// components, relations, queries, uniques, resources, deferred commands, events. This file
/// holds construction, disposal, and entity-count introspection; the per-feature partials
/// carry the matching public surface.
/// </para>
/// </remarks>
public sealed partial class World : IDisposable
{
    /// <summary>
    /// The maximum allowed capacity. Any value beyond this would round up to a power of 2
    /// that exceeds <see cref="int.MaxValue"/>, causing an overflow when cast back to <see langword="int"/>.
    /// </summary>
    private const int MaxEntityCapacity = 1 << 30;

    /// <summary>Returns the total number of entity slots ever allocated, including those currently in the ghoul queue.</summary>
    internal int CountTotalEntities() => _entitiesCount;

    /// <summary>Returns the number of live (non-ghoulified) entities currently in the world.</summary>
    public int CountEntities() => _entitiesCount - _ghouls.Count;

    /// <summary>Returns the number of ghoulified entities awaiting slot recycling.</summary>
    public int CountGhouls() => _ghouls.Count;

    /// <summary>
    /// Constructs a world with initial entity capacity rounded up to the next power of two.
    /// Capacity affects only initial buffer sizes; the world grows past it on demand.
    /// </summary>
    /// <param name="capacity">Initial entity capacity. Rounded up to the next power of two; must be in <c>(0, 2^30]</c>.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="capacity"/> is non-positive or exceeds <c>2^30</c>.</exception>
    public World(int capacity = DefaultEntitiesMetaCapacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Capacity must be greater than zero."
            );

        if (capacity > MaxEntityCapacity)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                $"Capacity must not exceed {MaxEntityCapacity}."
            );

        capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);

        _entitiesMeta = new EntityMeta[capacity];
        _ghouls = new Queue<int>(capacity / DefaultGhoulsInitialCapacityDivisor);

        _assemblages = Array.Empty<Assemblage>();

        _archetypes = new Archetype[DefaultArchetypesCapacity];

        _uniques = Array.Empty<Unique>();

        _nomadQueries = Array.Empty<NomadQuery>();

        _deferredAddComponent = new Queue<EntityComponentDeferred>(DefaultDeferredCommandsCapacity);
        _deferredAddComponentStorages = Array.Empty<ComponentDeferredStorage>();
        _deferredRemoveComponent = new Queue<EntityComponentDeferred>(
            DefaultDeferredCommandsCapacity
        );
        _deferredGhoulify = new Queue<Entity>(DefaultDeferredCommandsCapacity);

        _relationStorages = Array.Empty<RelationStorage>();
        _relationsIndexers = new RelationsIndexer[DefaultRelationsIndexerCapacity];
        _freeRelationSlots = new(DefaultRelationsIndexerCapacity);

        for (int i = DefaultRelationsIndexerCapacity - 1; i >= 0; i--)
            _freeRelationSlots.Push(i);

        _deferredAddRelation = new Queue<AddRelationDeferred>(DefaultDeferredCommandsCapacity);
        _deferredAddRelationStorages = Array.Empty<RelationDeferredStorage>();
        _deferredRemoveRelation = new Queue<RemoveRelationDeferred>(
            DefaultDeferredCommandsCapacity
        );

        _resources = new();

        EntitiesEvents = new();
        RelationsEvents = new();
    }

    /// <summary>
    /// Tears down the world's event wiring by resetting <see cref="EntitiesEvents"/> and
    /// <see cref="RelationsEvents"/>. Pooled state held by the world (archetypes, relation
    /// storage, deferred queues) <b>becomes eligible for collection when the world itself</b> is no
    /// longer reachable.
    /// </summary>
    public void Dispose()
    {
        EntitiesEvents.Reset();
        RelationsEvents.Reset();
    }
}
