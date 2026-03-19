using System;
using System.Diagnostics.CodeAnalysis;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    internal Assemblage[] _assemblages;

    private void AddAssemblage(in Assemblage assemblage)
    {
        int index = _assemblages.Length;

        Array.Resize(ref _assemblages, index + 1);

        _assemblages[index] = assemblage;
    }

    public Assemblage<T> CreateAssemblage<T>()
        where T : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        int componentId = ComponentMeta<T>.s_id;

        Archetype archetype = FindOrCreateArchetype(stackalloc int[1] { componentId });

        Span<int> componentStorageMapping = stackalloc int[1];

        componentStorageMapping[0] = archetype.GetStorageIndex(componentId);

        Assemblage<T> assemblage = new Assemblage<T>(this, in archetype, componentStorageMapping);

        AddAssemblage(assemblage);

        if (!archetype.TrySetMatchedAssemblage(assemblage))
            ThrowInvalidOperationDuplicatedAssemblage();

        return assemblage;
    }

    [DoesNotReturn]
    internal static void ThrowInvalidOperationDuplicatedAssemblage() =>
        throw new InvalidOperationException(
            "An Assemblage for the matched Archetype already exists. Assemblages must be unique."
        );
}
