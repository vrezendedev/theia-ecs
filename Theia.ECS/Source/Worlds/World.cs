using System;
using System.Collections.Generic;
using System.Numerics;
using Theia.ECS.Archetypes;
using Theia.ECS.Queries;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private const int DefaultEntitiesMetaCapacity = 16_384;
    private const int DefaultGhoulsInitialCapacityDivisor = 4;
    private const int DefaultArchetypesCapacity = 16;
    private const int DefaultDeferredCommandsCapacity = 256;

    /// <summary>
    /// The maximum allowed capacity. Any value beyond this would round up to a power of 2
    /// that exceeds <see cref="int.MaxValue"/>, causing an overflow when cast back to <see langword="int"/>.
    /// </summary>
    private const int MaxEntityCapacity = 1 << 30;

    private static int s_worldsCount;
    private static readonly object s_lock = new object();

    internal readonly int _worldId;

    internal int CountEntities() => _entitiesCount - _ghoulsCount;

    internal int CountGhouls() => _ghoulsCount;

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

        _entitiesMeta = new Entities.EntityMeta[capacity];
        _ghouls = new Queue<int>(capacity / DefaultGhoulsInitialCapacityDivisor);

        _archetypes = new Archetype[DefaultArchetypesCapacity];

        _nomadQueries = Array.Empty<NomadQuery>();
        _settlerQueries = Array.Empty<SettlerQuery>();

        _deferredCommands = new Queue<Action>(DefaultDeferredCommandsCapacity);

        lock (s_lock)
        {
            _worldId = s_worldsCount;
            s_worldsCount++;
        }
    }
}
