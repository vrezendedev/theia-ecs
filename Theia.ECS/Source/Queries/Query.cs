using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Extensions;
using Theia.ECS.Worlds;

namespace Theia.ECS.Queries;

public abstract class Query
{
    protected readonly World _world;

    internal Query(in World world) => _world = world;
}

public abstract class SettlerQuery : Query
{
    internal readonly Archetype _archetype;
    private readonly int[] _componentStorageMapping;

    internal SettlerQuery(in World world, in Assemblage assemblage)
        : base(in world)
    {
        _archetype = assemblage._archetype;
        _componentStorageMapping = assemblage._componentStorageMapping;
    }

    internal ReadOnlySpan<int> GetComponentStorageMapping() => _componentStorageMapping.AsSpan();
}

public abstract class NomadQuery : Query
{
    private const int DefaultMatchedArchetypesCapacity = 4;
    private const int DefaultMatchedArchetypesGrowthFactor = 2;

    internal readonly Signature _signature;

    protected int _matchedArchetypesCount;
    private Archetype[] _matchedArchetypes;

    internal NomadQuery(in World world, ReadOnlySpan<int> componentIds)
        : base(in world)
    {
        _signature = new Signature(componentIds);
        _matchedArchetypes = new Archetype[DefaultMatchedArchetypesCapacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<Archetype> GetMatchedArchetypes() =>
        _matchedArchetypes.AsSpan(0, _matchedArchetypesCount);

    internal void Add(Archetype archetype)
    {
        int index = _matchedArchetypesCount;

        Array.TryResize(ref _matchedArchetypes, index, DefaultMatchedArchetypesGrowthFactor);

        _matchedArchetypes[index] = archetype;

        _matchedArchetypesCount++;
    }
}
