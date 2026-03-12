using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;

namespace Theia.ECS.Queries;

public abstract class Query
{
    internal readonly int _worldId;
    internal readonly Signature _signature;

    internal Query(int worldId, ReadOnlySpan<int> componentIds)
    {
        _worldId = worldId;
        _signature = new Signature(componentIds);
    }

    internal Query(int worldId, Signature signature)
    {
        _worldId = worldId;
        _signature = signature;
    }
}

public abstract class SettlerQuery : Query
{
    internal readonly int _matchedArchetype;
    private readonly int[] _componentStorageMapping;

    internal SettlerQuery(int worldId, in Assemblage assemblage)
        : base(worldId, assemblage._signature)
    {
        _matchedArchetype = assemblage._matchedArchetype;
        _componentStorageMapping = assemblage._componentStorageMapping;
    }

    internal ReadOnlySpan<int> ComponentStorageMapping() => _componentStorageMapping.AsSpan();
}

public abstract class NomadQuery : Query
{
    private const int DefaultMatchedArchetypesCapacity = 4;
    private const int DefaultMatchedArchetypesGrowthFactor = 2;

    private int _matchedArchetypesCount;
    private int[] _matchedArchetypes;

    internal NomadQuery(int worldId, ReadOnlySpan<int> componentIds)
        : base(worldId, componentIds)
    {
        Span<int> initialArr = stackalloc int[DefaultMatchedArchetypesCapacity];
        initialArr.Fill(-1);

        _matchedArchetypes = initialArr.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> Archetypes() =>
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
