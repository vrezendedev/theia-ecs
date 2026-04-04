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

    internal void IncrementQueriesBeingExecuted() =>
        Interlocked.Increment(ref _queriesBeingExecuted);

    internal void DecrementQueriesBeingExecuted() =>
        Interlocked.Decrement(ref _queriesBeingExecuted);

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

    public SettlerQuery<ComponentT1> CreateSettlerQuery<ComponentT1>(
        Assemblage<ComponentT1> assemblage
    )
        where ComponentT1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        return new SettlerQuery<ComponentT1>(this, assemblage);
    }

    public NomadQuery<ComponentT1> CreateNomadQuery<ComponentT1>()
        where ComponentT1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        ReadOnlySpan<int> componentIds = stackalloc int[1] { ComponentMeta<ComponentT1>.s_id };

        NomadQuery<ComponentT1> nomadQuery = new NomadQuery<ComponentT1>(this, componentIds);

        AddNomadQuery(nomadQuery);

        AddSatisfiedArchetypes(nomadQuery);

        return nomadQuery;
    }

    internal void ThrowIfQueriesExecuting()
    {
        if (AreThereAnyQueriesBeingExecuted())
            throw new InvalidOperationException(
                "Cannot perform structural changes or entity modifications while queries are executing. Use deferred commands to schedule modifications for after query execution completes."
            );
    }
}
