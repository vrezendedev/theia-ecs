using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldRelationConcurrencyTests
{
    private const int ThreadCount = 8;
    private const int TestTimeoutMs = 5_000;
    private static readonly TimeSpan TestTimeoutSpan = TimeSpan.FromMilliseconds(TestTimeoutMs);

    private static async Task RunConcurrent(int threadCount, Action<int> work)
    {
        Barrier barrier = new(threadCount);

        Task[] tasks = Enumerable
            .Range(0, threadCount)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    work(i);
                })
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);
    }

    [Fact]
    public async Task ConcurrentAdd_NOwners_SameTarget_AllLinksRecorded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity[] owners = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        Entity target = assemblage.Create(new Position());

        await RunConcurrent(ThreadCount, i => world.TryAddTagRelation<Friend>(owners[i], target));

        Assert.Equal(ThreadCount, world.CountExternalLinks<Friend>(target));

        foreach (Entity owner in owners)
            Assert.Equal(1, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public async Task ConcurrentAdd_NOwners_SameTarget_EachOwnerIsRelatedToTarget()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity[] owners = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        Entity target = assemblage.Create(new Position());

        await RunConcurrent(ThreadCount, i => world.TryAddTagRelation<Friend>(owners[i], target));

        foreach (Entity owner in owners)
            Assert.True(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public async Task ConcurrentAdd_SameOwner_NTargets_AllRelationsRecorded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity[] targets = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        await RunConcurrent(ThreadCount, i => world.TryAddTagRelation<Friend>(owner, targets[i]));

        Assert.Equal(ThreadCount, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public async Task ConcurrentAdd_SameOwner_NTargets_OwnerIsRelatedToAllTargets()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity[] targets = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        await RunConcurrent(ThreadCount, i => world.TryAddTagRelation<Friend>(owner, targets[i]));

        foreach (Entity target in targets)
            Assert.True(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public async Task ConcurrentAdd_Evaluated_SameOwner_NTargets_AllDataStoredCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity[] targets = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        await RunConcurrent(
            ThreadCount,
            i => world.TryAddEvaluatedRelation(owner, targets[i], new Damage { Value = i + 1f })
        );

        Assert.Equal(ThreadCount, world.CountRelations<Damage>(owner));

        for (int i = 0; i < ThreadCount; i++)
            Assert.Equal(i + 1f, world.GetEvaluatedRelation<Damage>(owner, targets[i]).Value);
    }

    [Fact]
    public async Task ConcurrentRead_CountRelations_NeverThrowsOrReturnsNegative()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        for (int i = 0; i < ThreadCount; i++)
            world.TryAddTagRelation<Friend>(owner, assemblage.Create(new Position()));

        int[] results = new int[ThreadCount];

        await RunConcurrent(ThreadCount, i => results[i] = world.CountRelations<Friend>(owner));

        foreach (int result in results)
            Assert.True(result >= 0);
    }

    [Fact]
    public async Task ConcurrentRead_HasRelation_AlwaysConsistent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, assemblage.Create(new Position()));

        bool[] results = new bool[ThreadCount];

        await RunConcurrent(ThreadCount, i => results[i] = world.HasRelation<Friend>(owner));

        Assert.All(results, r => Assert.True(r));
    }

    [Fact]
    public async Task ConcurrentRead_IsRelatedTo_AlwaysConsistent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, target);

        bool[] results = new bool[ThreadCount];

        await RunConcurrent(
            ThreadCount,
            i => results[i] = world.IsRelatedTo<Friend>(owner, target)
        );

        Assert.All(results, r => Assert.True(r));
    }

    [Fact]
    public async Task ConcurrentRead_CountExternalLinks_NeverThrowsOrReturnsNegative()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity target = assemblage.Create(new Position());

        for (int i = 0; i < ThreadCount; i++)
            world.TryAddTagRelation<Friend>(assemblage.Create(new Position()), target);

        int[] results = new int[ThreadCount];

        await RunConcurrent(
            ThreadCount,
            i => results[i] = world.CountExternalLinks<Friend>(target)
        );

        foreach (int result in results)
            Assert.True(result >= 0);
    }

    [Fact]
    public async Task ConcurrentQuery_NThreads_SameOwner_AllCallbacksFire()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        const int TargetCount = 4;

        Entity owner = assemblage.Create(new Position());

        for (int i = 0; i < TargetCount; i++)
            world.TryAddTagRelation<Friend>(owner, assemblage.Create(new Position()));

        int totalCallbacks = 0;

        await RunConcurrent(
            ThreadCount,
            _ => world.QueryRelation<Friend>(owner, _ => Interlocked.Increment(ref totalCallbacks))
        );

        Assert.Equal(ThreadCount * TargetCount, totalCallbacks);
    }

    [Fact]
    public async Task ConcurrentQuery_Evaluated_NThreads_SameOwner_AllCallbacksFire()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        const int TargetCount = 4;

        Entity owner = assemblage.Create(new Position());

        for (int i = 0; i < TargetCount; i++)
            world.TryAddEvaluatedRelation(
                owner,
                assemblage.Create(new Position()),
                new Damage { Value = 1f }
            );

        int totalCallbacks = 0;

        await RunConcurrent(
            ThreadCount,
            _ =>
                world.QueryEvaluatedRelation<Damage>(
                    owner,
                    (Entity _, ref Damage _) => Interlocked.Increment(ref totalCallbacks)
                )
        );

        Assert.Equal(ThreadCount * TargetCount, totalCallbacks);
    }

    [Fact]
    public async Task QueryRelation_BlocksStructuralChange_AddThrowsWhileQueryRunning()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity existingTarget = assemblage.Create(new Position());
        Entity newTarget = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, existingTarget);

        using ManualResetEventSlim queryEntered = new(false);
        using ManualResetEventSlim queryRelease = new(false);

        bool finished = false;

        Task queryTask = Task.Run(() =>
            world.QueryRelation<Friend>(
                owner,
                _ =>
                {
                    queryEntered.Set();
                    queryRelease.Wait();
                    finished = true;
                }
            )
        );

        bool entered = queryEntered.Wait(TestTimeoutMs);
        Assert.True(entered);

        Assert.Throws<InvalidOperationException>(() =>
            world.TryAddTagRelation<Friend>(owner, newTarget)
        );

        queryRelease.Set();

        await queryTask.WaitAsync(TestTimeoutSpan);

        Assert.True(finished);
    }

    [Fact]
    public async Task QueryRelation_BlocksStructuralChange_RemoveThrowsWhileQueryRunning()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, targetA);
        world.TryAddTagRelation<Friend>(owner, targetB);

        using ManualResetEventSlim queryEntered = new(false);
        using ManualResetEventSlim queryRelease = new(false);

        bool finished = false;

        Task queryTask = Task.Run(() =>
            world.QueryRelation<Friend>(
                owner,
                _ =>
                {
                    queryEntered.Set();
                    queryRelease.Wait();
                    finished = true;
                }
            )
        );

        bool entered = queryEntered.Wait(TestTimeoutMs);

        Assert.True(entered);

        Assert.Throws<InvalidOperationException>(() =>
            world.TryRemoveRelation<Friend>(owner, targetA)
        );

        queryRelease.Set();

        await queryTask.WaitAsync(TestTimeoutSpan);

        Assert.True(finished);
    }

    [Fact]
    public async Task QueryEvaluatedRelation_BlocksStructuralChange_AddThrowsWhileQueryRunning()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity existingTarget = assemblage.Create(new Position());
        Entity newTarget = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, existingTarget, new Damage { Value = 5f });

        using ManualResetEventSlim queryEntered = new(false);
        using ManualResetEventSlim queryRelease = new(false);

        bool finished = false;

        Task queryTask = Task.Run(() =>
            world.QueryEvaluatedRelation(
                owner,
                (Entity _, ref Damage _) =>
                {
                    queryEntered.Set();
                    queryRelease.Wait();
                    finished = true;
                }
            )
        );

        bool entered = queryEntered.Wait(TestTimeoutMs);

        Assert.True(entered);

        Assert.Throws<InvalidOperationException>(() =>
            world.TryAddEvaluatedRelation(owner, newTarget, new Damage { Value = 99f })
        );

        queryRelease.Set();

        await queryTask.WaitAsync(TestTimeoutSpan);

        Assert.True(finished);
    }

    [Fact]
    public async Task ConcurrentAdd_And_RemoveAll_DisjointTargets_AllSucceed()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int half = ThreadCount / 2;

        Entity owner = assemblage.Create(new Position());

        Entity[] removeTargets = Enumerable
            .Range(0, half)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        foreach (Entity t in removeTargets)
            world.TryAddTagRelation<Friend>(owner, t);

        Entity[] addTargets = Enumerable
            .Range(0, half)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        bool[] addResults = new bool[half];
        bool[] removeResults = new bool[half];

        Barrier barrier = new(ThreadCount);

        Task[] tasks = Enumerable
            .Range(0, half)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    addResults[i] = world.TryAddTagRelation<Friend>(owner, addTargets[i]);
                })
            )
            .Concat(
                Enumerable
                    .Range(0, half)
                    .Select(i =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            removeResults[i] = world.TryRemoveRelation<Friend>(
                                owner,
                                removeTargets[i]
                            );
                        })
                    )
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);

        Assert.All(addResults, r => Assert.True(r));
        Assert.All(removeResults, r => Assert.True(r));
    }

    [Fact]
    public async Task ConcurrentAdd_Evaluated_And_Read_NoExceptionsAndValuesAreValid()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int half = ThreadCount / 2;

        Entity owner = assemblage.Create(new Position());

        Entity primeTarget = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, primeTarget, new Damage { Value = 1f });

        Entity[] writeTargets = Enumerable
            .Range(0, half)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        List<Exception> readerExceptions = new();

        object exLock = new();

        Barrier barrier = new(ThreadCount);

        Task[] tasks = Enumerable
            .Range(0, half)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    world.TryAddEvaluatedRelation(
                        owner,
                        writeTargets[i],
                        new Damage { Value = i + 10f }
                    );
                })
            )
            .Concat(
                Enumerable
                    .Range(0, half)
                    .Select(_ =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            try
                            {
                                int count = world.CountRelations<Damage>(owner);
                                Assert.True(count >= 0);
                            }
                            catch (Exception ex)
                            {
                                lock (exLock)
                                    readerExceptions.Add(ex);
                            }
                        })
                    )
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);

        Assert.Empty(readerExceptions);

        int finalCount = world.CountRelations<Damage>(owner);
        Assert.True(finalCount >= 1);
        Assert.True(finalCount <= half + 1);
    }

    [Fact]
    public async Task ConcurrentQueryEvaluated_Mutations_SerializeCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, target, new Damage { Value = 0f });

        await RunConcurrent(
            ThreadCount,
            _ =>
                world.QueryEvaluatedRelation<Damage>(
                    owner,
                    (Entity _, ref Damage d) => d.Value += 1f
                )
        );

        Assert.Equal(ThreadCount, world.GetEvaluatedRelation<Damage>(owner, target).Value);
    }

    [Fact]
    public async Task ConcurrentAddAndRemoveAll_NoExceptionsAndFinalStateIsConsistent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int adders = ThreadCount - 2;
        int removers = 2;

        Entity owner = assemblage.Create(new Position());

        Entity[] addTargets = Enumerable
            .Range(0, adders)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        Barrier barrier = new(ThreadCount);

        Task[] tasks = Enumerable
            .Range(0, adders)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    world.TryAddTagRelation<Friend>(owner, addTargets[i]);
                })
            )
            .Concat(
                Enumerable
                    .Range(0, removers)
                    .Select(_ =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            world.TryRemoveRelation<Friend>(owner);
                        })
                    )
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);

        int count = world.CountRelations<Friend>(owner);
        Assert.True(count >= 0);
        Assert.True(count <= adders);

        bool hasRelation = world.HasRelation<Friend>(owner);

        Assert.Equal(count > 0, hasRelation);
    }

    [Fact]
    public async Task ConcurrentAdd_IndependentPairs_AllSucceed()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity[] owners = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        Entity[] targets = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        bool[] results = new bool[ThreadCount];

        await RunConcurrent(
            ThreadCount,
            i => results[i] = world.TryAddTagRelation<Friend>(owners[i], targets[i])
        );

        Assert.All(results, r => Assert.True(r));

        for (int i = 0; i < ThreadCount; i++)
        {
            Assert.Equal(1, world.CountRelations<Friend>(owners[i]));
            Assert.Equal(1, world.CountExternalLinks<Friend>(targets[i]));
        }
    }

    [Fact]
    public async Task ConcurrentAddAndRemove_NOwners_SameTarget_FinalCountIsCorrect()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int half = ThreadCount / 2;

        Entity target = assemblage.Create(new Position());

        Entity[] removeOwners = Enumerable
            .Range(0, half)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        foreach (Entity o in removeOwners)
            world.TryAddTagRelation<Friend>(o, target);

        Entity[] addOwners = Enumerable
            .Range(0, half)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        bool[] addResults = new bool[half];
        bool[] removeResults = new bool[half];

        Barrier barrier = new(ThreadCount);

        Task[] tasks = Enumerable
            .Range(0, half)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    addResults[i] = world.TryAddTagRelation<Friend>(addOwners[i], target);
                })
            )
            .Concat(
                Enumerable
                    .Range(0, half)
                    .Select(i =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            removeResults[i] = world.TryRemoveRelation<Friend>(
                                removeOwners[i],
                                target
                            );
                        })
                    )
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);

        Assert.All(addResults, r => Assert.True(r));
        Assert.All(removeResults, r => Assert.True(r));

        Assert.Equal(half, world.CountExternalLinks<Friend>(target));

        foreach (Entity owner in addOwners)
            Assert.True(world.IsRelatedTo<Friend>(owner, target));

        foreach (Entity owner in removeOwners)
            Assert.False(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public async Task ConcurrentAddAndRemove_SameOwner_NTargets_FinalCountIsCorrect()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int half = ThreadCount / 2;

        Entity owner = assemblage.Create(new Position());

        Entity[] removeTargets = Enumerable
            .Range(0, half)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        foreach (Entity t in removeTargets)
            world.TryAddTagRelation<Friend>(owner, t);

        Entity[] addTargets = Enumerable
            .Range(0, half)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        bool[] addResults = new bool[half];
        bool[] removeResults = new bool[half];

        Barrier barrier = new(ThreadCount);

        Task[] tasks = Enumerable
            .Range(0, half)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    addResults[i] = world.TryAddTagRelation<Friend>(owner, addTargets[i]);
                })
            )
            .Concat(
                Enumerable
                    .Range(0, half)
                    .Select(i =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            removeResults[i] = world.TryRemoveRelation<Friend>(
                                owner,
                                removeTargets[i]
                            );
                        })
                    )
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);

        Assert.All(addResults, r => Assert.True(r));
        Assert.All(removeResults, r => Assert.True(r));

        Assert.Equal(half, world.CountRelations<Friend>(owner));

        foreach (Entity target in addTargets)
            Assert.True(world.IsRelatedTo<Friend>(owner, target));

        foreach (Entity target in removeTargets)
            Assert.False(world.IsRelatedTo<Friend>(owner, target));
    }
}
