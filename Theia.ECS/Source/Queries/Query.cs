using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Worlds;

namespace Theia.ECS.Queries;

public abstract class Query
{
    internal readonly World _world;

    internal Query(in World world) => _world = world;
}

public abstract class SettlerQuery : Query
{
    internal readonly int _matchedArchetypeId;
    private readonly int[] _componentStorageMapping;

    internal SettlerQuery(in World world, in Assemblage assemblage)
        : base(in world)
    {
        _matchedArchetypeId = assemblage._matchedArchetypeId;
        _componentStorageMapping = assemblage._componentStorageMapping;
    }

    internal ReadOnlySpan<int> GetComponentStorageMapping() => _componentStorageMapping.AsSpan();
}

public abstract class NomadQuery : Query
{
    private const int DefaultMatchedArchetypesCapacity = 4;
    private const int DefaultMatchedArchetypesGrowthFactor = 2;

    internal readonly Signature _signature;

    private int _matchedArchetypesCount;
    private int[] _matchedArchetypes;

    internal NomadQuery(in World world, ReadOnlySpan<int> componentIds)
        : base(in world)
    {
        _signature = new Signature(componentIds);
        _matchedArchetypes = new int[DefaultMatchedArchetypesCapacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> GetMatchedArchetypes() =>
        _matchedArchetypes.AsSpan(0, _matchedArchetypesCount);

    internal void Add(int archetypeId)
    {
        int currentLength = _matchedArchetypes.Length;

        if (_matchedArchetypesCount == currentLength)
            Array.Resize(
                ref _matchedArchetypes,
                currentLength * DefaultMatchedArchetypesGrowthFactor
            );

        int index = _matchedArchetypesCount;

        _matchedArchetypes[index] = archetypeId;

        _matchedArchetypesCount++;
    }
}
