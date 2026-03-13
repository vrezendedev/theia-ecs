using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private const int DefaultArchetypesGrowthFactor = 2;

    private int _archetypesCount;
    private Archetype[] _archetypes;

    internal Archetype CreateArchetype(in Signature signature)
    {
        int currentLength = _archetypes.Length;

        if (_archetypesCount == currentLength)
            Array.Resize(ref _archetypes, currentLength * DefaultArchetypesGrowthFactor);

        int index = _archetypesCount;

        Archetype archetype = new Archetype(index, signature);
        _archetypes[index] = archetype;

        _archetypesCount++;

        return archetype;
    }
}
