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

    /// <summary>
    /// Creates an <see cref="Assemblage{ComponentT1}"/> bound to the archetype whose composition
    /// is exactly <typeparamref name="ComponentT1"/>, finding the archetype if it already exists
    /// or creating it on first use. Each archetype admits at most one assemblage; attempting to
    /// register <b>a second for the same composition throws</b>.
    /// </summary>
    /// <remarks>
    /// The one-assemblage-per-archetype rule is what makes <see cref="Queries.SettlerQuery{ComponentT1}"/>
    /// unambiguous: a settler bound to a specific assemblage, iterates exactly that
    /// archetype, <b>with no possibility of two factories producing entities into the same archetype
    /// under conflicting events or naming</b>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a query is currently iterating, a deferred flush is in progress, or an
    /// assemblage has already been registered for this composition.
    /// </exception>
    public Assemblage<ComponentT1> CreateAssemblage<ComponentT1>()
        where ComponentT1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        Archetype archetype = FindOrCreateArchetype(stackalloc int[1] { componentT1Id });

        Span<int> componentStorageMapping = stackalloc int[1];

        componentStorageMapping[0] = archetype.GetStorageIndex(componentT1Id);

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
