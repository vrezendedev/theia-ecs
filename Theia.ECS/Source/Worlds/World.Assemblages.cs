using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    public Assemblage<T> CreateAssemblage<T>()
        where T : struct
    {
        int componentId = ComponentMeta<T>.s_id;

        Archetype archetype = FindOrCreateArchetype(stackalloc int[1] { componentId });

        Span<int> componentStorageMapping = stackalloc int[1];

        componentStorageMapping[0] = archetype.GetStorageIndex(componentId);

        return new Assemblage<T>(this, in archetype, componentStorageMapping);
    }
}
