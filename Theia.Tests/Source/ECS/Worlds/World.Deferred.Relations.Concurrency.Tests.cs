using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theia.ECS.Assemblages;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldDeferredRelationsConcurrencyTests
{
    private const int ThreadCount = 8;
    private static readonly TimeSpan TestTimeoutSpan = TimeSpan.FromSeconds(5);

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
    public async Task ConcurrentDeferredAddRelation_SameOwner_NTargets_AllEstablishedOnFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity[] targets = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        await RunConcurrent(ThreadCount, i => world.DeferredAddRelation<Friend>(owner, targets[i]));

        world.FlushDeferred();

        Assert.Equal(ThreadCount, world.CountRelations<Friend>(owner));

        foreach (Entity target in targets)
            Assert.True(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public async Task ConcurrentDeferredAddRelation_NOwners_SameTarget_AllEstablishedOnFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity target = assemblage.Create(new Position());
        Entity[] owners = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        await RunConcurrent(ThreadCount, i => world.DeferredAddRelation<Friend>(owners[i], target));

        world.FlushDeferred();

        Assert.Equal(ThreadCount, world.CountExternalLinks<Friend>(target));

        foreach (Entity owner in owners)
            Assert.True(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public async Task ConcurrentDeferredAddEvaluatedRelation_SameOwner_NTargets_AllValuesCorrectAfterFlush()
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
            i =>
                world.DeferredAddEvaluatedRelation(owner, targets[i], new Damage { Value = i + 1f })
        );

        world.FlushDeferred();

        Assert.Equal(ThreadCount, world.CountRelations<Damage>(owner));

        for (int i = 0; i < ThreadCount; i++)
            Assert.Equal(i + 1f, world.GetEvaluatedRelation<Damage>(owner, targets[i]).Value);
    }

    [Fact]
    public async Task ConcurrentDeferredAddEvaluatedRelation_NoStorageSlotsLostOrDuplicated()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity[] targets = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        float[] primes = [2f, 3f, 5f, 7f, 11f, 13f, 17f, 19f];
        float expectedSum = 0f;
        foreach (float p in primes)
            expectedSum += p;

        await RunConcurrent(
            ThreadCount,
            i =>
                world.DeferredAddEvaluatedRelation(
                    owner,
                    targets[i],
                    new Damage { Value = primes[i] }
                )
        );

        world.FlushDeferred();

        EntityEvaluatedRelations<Damage> relations = world.GetEvaluatedRelations<Damage>(owner);

        float actualSum = 0f;

        foreach (Damage d in relations.Data)
            actualSum += d.Value;

        Assert.Equal(expectedSum, actualSum);
    }

    [Fact]
    public async Task ConcurrentDeferredRemoveRelation_SameOwner_NTargets_AllRemovedOnFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity[] targets = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        foreach (Entity t in targets)
            world.TryAddTagRelation<Friend>(owner, t);

        await RunConcurrent(
            ThreadCount,
            i => world.DeferredRemoveRelation<Friend>(owner, targets[i])
        );

        world.FlushDeferred();

        Assert.Equal(0, world.CountRelations<Friend>(owner));

        foreach (Entity target in targets)
            Assert.Equal(0, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public async Task ConcurrentDeferredRemoveRelation_NOwners_SameTarget_AllRemovedOnFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity target = assemblage.Create(new Position());
        Entity[] owners = Enumerable
            .Range(0, ThreadCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        foreach (Entity o in owners)
            world.TryAddTagRelation<Friend>(o, target);

        await RunConcurrent(
            ThreadCount,
            i => world.DeferredRemoveRelation<Friend>(owners[i], target)
        );

        world.FlushDeferred();

        Assert.Equal(0, world.CountExternalLinks<Friend>(target));

        foreach (Entity owner in owners)
            Assert.False(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public async Task ConcurrentDeferredAdd_And_DeferredRemove_SameOwner_DisjointTargets_FinalStateIsCorrect()
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

        Barrier barrier = new(ThreadCount);

        Task[] tasks = Enumerable
            .Range(0, half)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    world.DeferredAddRelation<Friend>(owner, addTargets[i]);
                })
            )
            .Concat(
                Enumerable
                    .Range(0, half)
                    .Select(i =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            world.DeferredRemoveRelation<Friend>(owner, removeTargets[i]);
                        })
                    )
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);

        world.FlushDeferred();

        Assert.Equal(half, world.CountRelations<Friend>(owner));

        foreach (Entity t in addTargets)
            Assert.True(world.IsRelatedTo<Friend>(owner, t));

        foreach (Entity t in removeTargets)
            Assert.False(world.IsRelatedTo<Friend>(owner, t));
    }

    [Fact]
    public async Task ConcurrentDeferredCreate_WithTagRelation_SameOwner_AllEntitiesCreatedAndRelated()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        int createdCount = 0;
        assemblage.EntityEvents.SubscribeOnCreated(_ => Interlocked.Increment(ref createdCount));

        await RunConcurrent(
            ThreadCount,
            _ =>
                assemblage.DeferredCreate(
                    new Position(),
                    new DeferredRelationOnCreate<Friend> { Owner = owner }
                )
        );

        world.FlushDeferred();

        Assert.Equal(ThreadCount, createdCount);
        Assert.Equal(ThreadCount, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public async Task ConcurrentDeferredCreate_WithEvaluatedRelation_NoValuesLostOrDuplicated()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        float[] primes = [2f, 3f, 5f, 7f, 11f, 13f, 17f, 19f];
        float expectedSum = 0f;
        foreach (float p in primes)
            expectedSum += p;

        await RunConcurrent(
            ThreadCount,
            i =>
                assemblage.DeferredCreate(
                    new Position(),
                    new DeferredRelationOnCreate<Damage>
                    {
                        Owner = owner,
                        Relation = new Damage { Value = primes[i] },
                    }
                )
        );

        world.FlushDeferred();

        Assert.Equal(ThreadCount, world.CountRelations<Damage>(owner));

        EntityEvaluatedRelations<Damage> relations = world.GetEvaluatedRelations<Damage>(owner);

        float actualSum = 0f;
        foreach (Damage d in relations.Data)
            actualSum += d.Value;

        Assert.Equal(expectedSum, actualSum);
    }

    [Fact]
    public async Task ConcurrentDeferredAdd_Remove_And_Create_MixedWorkload_FinalStateIsConsistent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int addersCount = ThreadCount / 4;
        int removersCount = ThreadCount / 4;
        int creatorsCount = ThreadCount / 2;

        Entity addOwner = assemblage.Create(new Position());
        Entity removeOwner = assemblage.Create(new Position());
        Entity createOwner = assemblage.Create(new Position());

        Entity[] addTargets = Enumerable
            .Range(0, addersCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        Entity[] removeTargets = Enumerable
            .Range(0, removersCount)
            .Select(_ => assemblage.Create(new Position()))
            .ToArray();

        foreach (Entity t in removeTargets)
            world.TryAddTagRelation<Friend>(removeOwner, t);

        int creatorsStarted = 0;

        Barrier barrier = new(ThreadCount);

        Task[] tasks = Enumerable
            .Range(0, addersCount)
            .Select(i =>
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    world.DeferredAddRelation<Friend>(addOwner, addTargets[i]);
                })
            )
            .Concat(
                Enumerable
                    .Range(0, removersCount)
                    .Select(i =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            world.DeferredRemoveRelation<Friend>(removeOwner, removeTargets[i]);
                        })
                    )
            )
            .Concat(
                Enumerable
                    .Range(0, creatorsCount)
                    .Select(_ =>
                        Task.Run(() =>
                        {
                            barrier.SignalAndWait();
                            assemblage.DeferredCreate(
                                new Position(),
                                new DeferredRelationOnCreate<Friend> { Owner = createOwner }
                            );
                            Interlocked.Increment(ref creatorsStarted);
                        })
                    )
            )
            .ToArray();

        await Task.WhenAll(tasks).WaitAsync(TestTimeoutSpan);

        world.FlushDeferred();

        Assert.Equal(addersCount, world.CountRelations<Friend>(addOwner));
        foreach (Entity t in addTargets)
            Assert.True(world.IsRelatedTo<Friend>(addOwner, t));

        Assert.Equal(0, world.CountRelations<Friend>(removeOwner));
        foreach (Entity t in removeTargets)
            Assert.False(world.IsRelatedTo<Friend>(removeOwner, t));

        Assert.Equal(creatorsCount, world.CountRelations<Friend>(createOwner));
    }
}
