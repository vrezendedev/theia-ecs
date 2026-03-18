using System;
using System.Threading;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    internal Assemblage[] _assemblages;
    private readonly Lock _assemblagesLock = new();

    private void AddAssemblage(in Assemblage assemblage)
    {
        lock (_assemblagesLock)
        {
            int index = _assemblages.Length;

            Array.Resize(ref _assemblages, index + 1);

            _assemblages[index] = assemblage;
        }
    }

    public Assemblage<T> CreateAssemblage<T>()
        where T : struct
    {
        int componentId = ComponentMeta<T>.s_id;

        Archetype archetype = FindOrCreateArchetype(stackalloc int[1] { componentId });

        Span<int> componentStorageMapping = stackalloc int[1];

        componentStorageMapping[0] = archetype.GetStorageIndex(componentId);

        Assemblage<T> assemblage = new Assemblage<T>(this, in archetype, componentStorageMapping);

        AddAssemblage(assemblage);

        return assemblage;
    }
}
