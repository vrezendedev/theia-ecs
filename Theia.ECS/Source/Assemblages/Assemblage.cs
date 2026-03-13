using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

public class Assemblage
{
    internal readonly World _world;
    internal readonly Signature _signature;
    internal readonly int _matchedArchetypeId;
    internal int[] _componentStorageMapping;

    internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
    {
        _world = world;
        _signature = archetype._signature;
        _matchedArchetypeId = archetype._archetypeId;
        _componentStorageMapping = componentStorageMapping.ToArray();
    }

    internal ReadOnlySpan<int> GetComponentStorageMapping() => _componentStorageMapping.AsSpan();
}
