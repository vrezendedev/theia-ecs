using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Relations;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    /// <summary>
    /// The maximum allowed capacity. Any value beyond this would round up to a power of 2
    /// that exceeds <see cref="int.MaxValue"/>, causing an overflow when cast back to <see langword="int"/>.
    /// </summary>
    private const int MaxEntityCapacity = 1 << 30;

    private static int s_worldsCount;
    private static readonly Lock s_lock = new();

    internal readonly int _worldId;

    internal int CountEntities() => _entitiesCount - _ghouls.Count;

    internal int CountGhouls() => _ghouls.Count;

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

        EntitiesEvents = new();
        RelationsEvents = new();

        lock (s_lock)
        {
            _worldId = s_worldsCount;
            s_worldsCount++;
        }
    }
}
