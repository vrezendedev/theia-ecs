using System;
using System.Collections.Generic;
using Theia.ECS.Archetypes;
using Theia.ECS.Entities;
using Theia.ECS.Queries;

namespace Theia.ECS.Worlds;

public sealed class World
{
    private static int _worldsCount;
    private static readonly object _lock = new object();

    internal readonly int _worldId;

    private int _archetypesCount;
    private Archetype[] _archetypes;

    private int _queriesBeingExecuted;
    private int _queriesCount;
    private Query[] _queries;

    private int _entitiesCount;
    private EntityMeta[] _entitiesMeta;

    private int _ghoulsCount;
    private Queue<int> _ghouls;

    private Queue<Action> _deferredCommands;

    internal int Entities => _entitiesCount - _ghoulsCount;
    internal int Ghouls => _ghoulsCount;

    public World()
    {
        lock (_lock)
        {
            _worldId = _worldsCount;
            _worldsCount++;
        }
    }
}
