using System;
using System.Buffers;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Jobs;
using Theia.ECS.Worlds;

namespace Theia.ECS.Queries;

/// <summary>
/// Single-component <b>assemblage-bound query</b>. Iterates exactly the archetype paired with the
/// constructing <see cref="Assemblage{ComponentT1}"/>, with no archetype matching cost. Offers
/// four iteration shapes: with or without the entity handle, sequential or parallel.
/// </summary>
/// <remarks>
/// <para>
/// Sequential iteration (<see cref="ForEach"/>, <see cref="ForEachEntity"/>) takes its callback
/// by <c>ref</c>, allowing the callback to be a <c>ref struct</c> and capture stack-only state.
/// Parallel iteration (<see cref="ForEachParallel"/>, <see cref="ForEachEntityParallel"/>) takes
/// its callback by value because <b>each chunk's job receives its own copy</b>.
/// </para>
/// <para>
/// All four iteration paths increment the world's "queries executing" counter on entry and
/// decrement it on exit; structural-change APIs throw while the counter is non-zero. To make
/// changes during iteration, <b>use the deferred-command surface</b> on <see cref="World"/> and
/// flush after iteration completes.
/// </para>
/// </remarks>
public sealed class SettlerQuery<ComponentT1> : SettlerQuery
    where ComponentT1 : struct
{
    internal SettlerQuery(in World world, in Assemblage<ComponentT1> assemblage)
        : base(world, assemblage) { }

    /// <summary>
    /// Iterates every entity in the bound archetype, invoking <paramref name="forEachEntity"/>
    /// once per entity with its handle and a <c>ref</c> to its <typeparamref name="ComponentT1"/>.
    /// </summary>
    public void ForEachEntity<T>(ref T forEachEntity)
        where T : struct, IForEachEntity<ComponentT1>, allows ref struct
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        if (indexers.Length == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Storage> storagesT1 = archetype.GetStorages(mapping[0]);

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int count = indexer.Count();

            if (count == 0)
                continue;

            Span<Entity> entities = indexer.GetValues();

            Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(count);

            for (int j = 0; j < count; j++)
                forEachEntity.Execute(entities[j], ref storageT1[j]);
        }

        _world.DecrementQueriesBeingExecuted();
    }

    /// <summary>
    /// Parallel variant of <see cref="ForEachEntity"/>: dispatches one job per non-empty chunk
    /// through <see cref="JobScheduler"/> and blocks until all chunks finish. The callback is
    /// passed by value, so it cannot be a <c>ref struct</c>.
    /// </summary>
    public void ForEachEntityParallel<T>(T forEachEntity)
        where T : struct, IForEachEntity<ComponentT1>
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        int indexersCount = indexers.Length;

        if (indexersCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Storage> storagesT1 = archetype.GetStorages(mapping[0]);

        Job[] jobs = ArrayPool<Job>.Shared.Rent(indexersCount);

        int jobCount = 0;

        for (int i = 0; i < indexersCount; i++)
        {
            int count = indexers[i].Count();

            if (count == 0)
                continue;

            ForEachEntityJob<T, ComponentT1> job = JobPool<ForEachEntityJob<T, ComponentT1>>.Rent();

            job.ForEach = forEachEntity;
            job.Indexer = indexers[i];
            job.StoragesComponentT1 = (Storage<ComponentT1>)storagesT1[i];
            job.Count = count;

            jobs[jobCount] = job;
            jobCount++;
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachEntityJob<T, ComponentT1> job = (ForEachEntityJob<T, ComponentT1>)jobs[i];

            job.Indexer = null;
            job.StoragesComponentT1 = null;

            JobPool<ForEachEntityJob<T, ComponentT1>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }

    /// <summary>
    /// Iterates every entity in the bound archetype, invoking <paramref name="forEach"/> once
    /// per entity with a <c>ref</c> to its <typeparamref name="ComponentT1"/> row. The entity
    /// handle is not loaded, so this is faster than <see cref="ForEachEntity"/> when the
    /// callback does not need it.
    /// </summary>
    public void ForEach<T>(ref T forEach)
        where T : struct, IForEach<ComponentT1>, allows ref struct
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        if (indexers.Length == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Storage> storagesT1 = archetype.GetStorages(mapping[0]);

        for (int i = 0; i < indexers.Length; i++)
        {
            Indexer indexer = indexers[i];

            int count = indexer.Count();

            if (count == 0)
                continue;

            Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(count);

            for (int j = 0; j < count; j++)
                forEach.Execute(ref storageT1[j]);
        }

        _world.DecrementQueriesBeingExecuted();
    }

    /// <summary>
    /// Parallel variant of <see cref="ForEach"/>: dispatches one job per non-empty chunk through
    /// <see cref="JobScheduler"/> and blocks until all chunks finish. The callback is passed by
    /// value, so it cannot be a <c>ref struct</c>.
    /// </summary>
    public void ForEachParallel<T>(T forEach)
        where T : struct, IForEach<ComponentT1>
    {
        Archetype archetype = _archetype;

        Span<Indexer> indexers = archetype.GetIndexers();

        int indexersCount = indexers.Length;

        if (indexersCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

        Span<Storage> storagesT1 = archetype.GetStorages(mapping[0]);

        Job[] jobs = ArrayPool<Job>.Shared.Rent(indexersCount);

        int jobCount = 0;

        for (int i = 0; i < indexersCount; i++)
        {
            int count = indexers[i].Count();

            if (count == 0)
                continue;

            ForEachJob<T, ComponentT1> job = JobPool<ForEachJob<T, ComponentT1>>.Rent();

            job.ForEach = forEach;
            job.StoragesComponentT1 = (Storage<ComponentT1>)storagesT1[i];
            job.Count = count;

            jobs[jobCount] = job;
            jobCount++;
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachJob<T, ComponentT1> job = (ForEachJob<T, ComponentT1>)jobs[i];

            job.StoragesComponentT1 = null;

            JobPool<ForEachJob<T, ComponentT1>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }
}

/// <summary>
/// Single-component <b>composition-based query</b>. Iterates every archetype whose signature contains
/// <typeparamref name="ComponentT1"/>, walking each matched archetype's chunks in turn. Offers
/// the same four iteration shapes as <see cref="SettlerQuery{ComponentT1}"/>.
/// </summary>
/// <remarks>
/// <para>
/// Compared to <see cref="SettlerQuery{ComponentT1}"/>, this query pays a per-archetype storage
/// slot lookup at the start of each archetype's iteration; the assemblage-bound query has the
/// slot precomputed and avoids that lookup entirely. The cost is small but real, so prefer the
/// assemblage path when the entities to visit all share one composition.
/// </para>
/// <para>
/// New archetypes that satisfy this query's signature are appended to the matched-archetypes
/// list when they are first created, so a long-lived nomad query stays current without rescan.
/// </para>
/// </remarks>
public sealed class NomadQuery<ComponentT1> : NomadQuery
    where ComponentT1 : struct
{
    internal NomadQuery(in World world, ReadOnlySpan<int> componentIds)
        : base(world, componentIds) { }

    /// <summary>
    /// Iterates every entity across every matched archetype, invoking
    /// <paramref name="forEachEntity"/> once per entity with its handle and a <c>ref</c> to its
    /// <typeparamref name="ComponentT1"/>.
    /// </summary>
    public void ForEachEntity<T>(ref T forEachEntity)
        where T : struct, IForEachEntity<ComponentT1>, allows ref struct
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<Archetype> matchedArchetypesIds = GetMatchedArchetypes();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        foreach (Archetype archetype in matchedArchetypesIds)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

            int storagesT1Index = archetype.GetStorageIndex(componentT1Id);
            Span<Storage> storagesT1 = archetype.GetStorages(storagesT1Index);

            for (int i = 0; i < indexers.Length; i++)
            {
                Indexer indexer = indexers[i];

                int count = indexer.Count();

                if (count == 0)
                    continue;

                Span<Entity> entities = indexer.GetValues();

                Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(
                    count
                );

                for (int j = 0; j < count; j++)
                    forEachEntity.Execute(entities[j], ref storageT1[j]);
            }
        }

        _world.DecrementQueriesBeingExecuted();
    }

    /// <summary>
    /// Parallel variant of <see cref="ForEachEntity"/>: dispatches one job per non-empty chunk
    /// across all matched archetypes through <see cref="JobScheduler"/> and blocks until all
    /// chunks finish.
    /// </summary>
    public void ForEachEntityParallel<T>(T forEachEntity)
        where T : struct, IForEachEntity<ComponentT1>
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        ReadOnlySpan<Archetype> matchedArchetypes = GetMatchedArchetypes();

        int totalIndexers = 0;

        for (int i = 0; i < matchedArchetypes.Length; i++)
        {
            Archetype archetype = matchedArchetypes[i];
            totalIndexers += archetype.GetInitializedCount();
        }

        if (totalIndexers == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        Job[] jobs = ArrayPool<Job>.Shared.Rent(totalIndexers);

        int jobCount = 0;

        foreach (Archetype archetype in matchedArchetypes)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

            int storagesT1Index = archetype.GetStorageIndex(componentT1Id);
            Span<Storage> storagesT1 = archetype.GetStorages(storagesT1Index);

            for (int i = 0; i < indexers.Length; i++)
            {
                int count = indexers[i].Count();

                if (count == 0)
                    continue;

                ForEachEntityJob<T, ComponentT1> job = JobPool<
                    ForEachEntityJob<T, ComponentT1>
                >.Rent();

                job.ForEach = forEachEntity;
                job.Indexer = indexers[i];
                job.StoragesComponentT1 = (Storage<ComponentT1>)storagesT1[i];
                job.Count = count;

                jobs[jobCount] = job;
                jobCount++;
            }
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachEntityJob<T, ComponentT1> job = (ForEachEntityJob<T, ComponentT1>)jobs[i];

            job.Indexer = null;
            job.StoragesComponentT1 = null;

            JobPool<ForEachEntityJob<T, ComponentT1>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }

    /// <summary>
    /// Iterates every entity across every matched archetype, invoking <paramref name="forEach"/>
    /// with a <c>ref</c> to its <typeparamref name="ComponentT1"/>.
    /// </summary>
    public void ForEach<T>(ref T forEach)
        where T : struct, IForEach<ComponentT1>, allows ref struct
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        ReadOnlySpan<Archetype> matchedArchetypesIds = GetMatchedArchetypes();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        foreach (Archetype archetype in matchedArchetypesIds)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

            int storagesT1Index = archetype.GetStorageIndex(componentT1Id);
            Span<Storage> storagesT1 = archetype.GetStorages(storagesT1Index);

            for (int i = 0; i < indexers.Length; i++)
            {
                Indexer indexer = indexers[i];

                int count = indexer.Count();

                if (count == 0)
                    continue;

                Span<ComponentT1> storageT1 = ((Storage<ComponentT1>)storagesT1[i]).GetValues(
                    count
                );

                for (int j = 0; j < count; j++)
                    forEach.Execute(ref storageT1[j]);
            }
        }

        _world.DecrementQueriesBeingExecuted();
    }

    /// <summary>
    /// Parallel variant of <see cref="ForEach"/>: dispatches one job per non-empty chunk across
    /// all matched archetypes through <see cref="JobScheduler"/> and blocks until all chunks finish.
    /// </summary>
    public void ForEachParallel<T>(T forEach)
        where T : struct, IForEach<ComponentT1>
    {
        int matchedArchetypeCount = _matchedArchetypesCount;

        if (matchedArchetypeCount == 0)
            return;

        ReadOnlySpan<Archetype> matchedArchetypes = GetMatchedArchetypes();

        int totalIndexers = 0;

        for (int i = 0; i < matchedArchetypes.Length; i++)
        {
            Archetype archetype = matchedArchetypes[i];
            totalIndexers += archetype.GetInitializedCount();
        }

        if (totalIndexers == 0)
            return;

        _world.IncrementQueriesBeingExecuted();

        int componentT1Id = ComponentMeta<ComponentT1>.s_id;

        Job[] jobs = ArrayPool<Job>.Shared.Rent(totalIndexers);

        int jobCount = 0;

        foreach (Archetype archetype in matchedArchetypes)
        {
            Span<Indexer> indexers = archetype.GetIndexers();

            if (indexers.Length == 0)
                continue;

            int storagesT1Index = archetype.GetStorageIndex(componentT1Id);
            Span<Storage> storagesT1 = archetype.GetStorages(storagesT1Index);

            for (int i = 0; i < indexers.Length; i++)
            {
                int count = indexers[i].Count();

                if (count == 0)
                    continue;

                ForEachJob<T, ComponentT1> job = JobPool<ForEachJob<T, ComponentT1>>.Rent();

                job.ForEach = forEach;
                job.StoragesComponentT1 = (Storage<ComponentT1>)storagesT1[i];
                job.Count = count;

                jobs[jobCount] = job;
                jobCount++;
            }
        }

        JobScheduler.Run(jobs.AsSpan(0, jobCount));

        for (int i = 0; i < jobCount; i++)
        {
            ForEachJob<T, ComponentT1> job = (ForEachJob<T, ComponentT1>)jobs[i];

            job.StoragesComponentT1 = null;

            JobPool<ForEachJob<T, ComponentT1>>.Return(job);
        }

        ArrayPool<Job>.Shared.Return(jobs, clearArray: true);

        _world.DecrementQueriesBeingExecuted();
    }
}
