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

    public Assemblage<TComponent> CreateAssemblage<TComponent>()
        where TComponent : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        int componentId = ComponentMeta<TComponent>.s_id;

        Archetype archetype = FindOrCreateArchetype(stackalloc int[1] { componentId });

        Span<int> componentStorageMapping = stackalloc int[1];

        componentStorageMapping[0] = archetype.GetStorageIndex(componentId);

        Assemblage<TComponent> assemblage = new Assemblage<TComponent>(
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
    internal static void ThrowDuplicatedAssemblage() =>
        throw new InvalidOperationException(
            "An Assemblage for the matched Archetype already exists. Assemblages must be unique."
        );
}
