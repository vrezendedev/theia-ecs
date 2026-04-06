using System;
using System.Diagnostics.CodeAnalysis;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Assemblage[] _assemblages;

    private void AddAssemblage(in Assemblage assemblage)
    {
        int index = _assemblages.Length;

        Array.Resize(ref _assemblages, index + 1);

        _assemblages[index] = assemblage;
    }

    public Assemblage<ComponentT1> CreateAssemblage<ComponentT1>()
        where ComponentT1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        int componentId = ComponentMeta<ComponentT1>.s_id;

        Archetype archetype = FindOrCreateArchetype(stackalloc int[1] { componentId });

        Span<int> componentStorageMapping = stackalloc int[1];

        componentStorageMapping[0] = archetype.GetStorageIndex(componentId);

        Assemblage<ComponentT1> assemblage = new Assemblage<ComponentT1>(
            this,
            in archetype,
            componentStorageMapping
        );

        AddAssemblage(assemblage);

        if (!archetype.TrySetMatchedAssemblage(assemblage))
            ThrowDuplicatedAssemblage();

        return assemblage;
    }

    [DoesNotReturn]
    private static void ThrowDuplicatedAssemblage() =>
        throw new InvalidOperationException(
            "An Assemblage for the matched Archetype already exists. Assemblages must be unique."
        );
}
