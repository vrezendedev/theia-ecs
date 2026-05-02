using System;
using System.Threading;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Queries;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private int _queriesBeingExecuted;

    private NomadQuery[] _nomadQueries;

    /// <summary>Increments the world's "queries executing" counter; called by query iteration entry points.</summary>
    internal void IncrementQueriesBeingExecuted() =>
        Interlocked.Increment(ref _queriesBeingExecuted);

    /// <summary>Decrements the world's "queries executing" counter; called when a query iteration completes.</summary>
    internal void DecrementQueriesBeingExecuted() =>
        Interlocked.Decrement(ref _queriesBeingExecuted);

    /// <summary>
    /// Returns <see langword="true"/> if any query is currently iterating on this world.
    /// </summary>
    public bool AreThereAnyQueriesBeingExecuted() => Volatile.Read(ref _queriesBeingExecuted) > 0;

    private void AddNomadQuery(NomadQuery nomadQuery)
    {
        int index = _nomadQueries.Length;

        Array.Resize(ref _nomadQueries, index + 1);

        _nomadQueries[index] = nomadQuery;
    }

    private void AddSatisfiedArchetypes(NomadQuery nomadQuery)
    {
        int archetypesCount = _archetypesCount;

        Span<Archetype> archetypes = _archetypes.AsSpan(0, _archetypesCount);

        for (int i = 0; i < archetypesCount; i++)
        {
            Archetype archetype = archetypes[i];

            if (nomadQuery._signature.IsSatisfiedBy(archetype._signature))
                nomadQuery.Add(archetype);
        }
    }

    /// <summary>
    /// Creates a <see cref="SettlerQuery{ComponentT1}"/> bound to <paramref name="assemblage"/>'s
    /// archetype. The returned query iterates exactly that archetype with no matching cost.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any query is currently iterating, or if a deferred flush is in progress.
    /// </exception>
    public SettlerQuery<ComponentT1> CreateSettlerQuery<ComponentT1>(
        Assemblage<ComponentT1> assemblage
    )
        where ComponentT1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        return new SettlerQuery<ComponentT1>(this, assemblage);
    }

    /// <summary>
    /// Creates a <see cref="NomadQuery{ComponentT1}"/> matching every archetype that contains
    /// <typeparamref name="ComponentT1"/>. The query is registered with the world so newly
    /// created archetypes that satisfy the signature are appended to its matched-archetypes
    /// list automatically; the existing archetypes are seeded immediately.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any query is currently iterating, or if a deferred flush is in progress.
    /// </exception>
    public NomadQuery<ComponentT1> CreateNomadQuery<ComponentT1>()
        where ComponentT1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        ReadOnlySpan<int> componentIds = stackalloc int[1] { componentT1Id };

        NomadQuery<ComponentT1> nomadQuery = new NomadQuery<ComponentT1>(this, componentIds);

        AddNomadQuery(nomadQuery);

        AddSatisfiedArchetypes(nomadQuery);

        return nomadQuery;
    }

    internal void ThrowIfQueriesExecuting()
    {
        if (AreThereAnyQueriesBeingExecuted())
            throw new InvalidOperationException(
                "Cannot perform structural changes or entity modifications while queries are executing. For entity modifications, use deferred commands to schedule modifications for after query execution completes."
            );
    }
}
