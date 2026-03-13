using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

public class Assemblage<T> : Assemblage
{
    internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
        : base(world, archetype, componentStorageMapping) { }
}
