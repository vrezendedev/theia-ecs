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

    internal bool AreThereAnyQueriesBeingExecuted() => Volatile.Read(ref _queriesBeingExecuted) > 0;

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

    public SettlerQuery<T1> CreateSettlerQuery<T1>(Assemblage<T1> assemblage)
        where T1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        return new SettlerQuery<T1>(this, assemblage);
    }

    public NomadQuery<T1> CreateNomadQuery<T1>()
        where T1 : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        ReadOnlySpan<int> componentIds = stackalloc int[1] { ComponentMeta<T1>.s_id };

        NomadQuery<T1> nomadQuery = new NomadQuery<T1>(this, componentIds);

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
