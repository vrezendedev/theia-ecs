using System;
using System.Threading;
using System.Threading.Tasks;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Systems;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class JobsTests
{
    [Fact]
    public void SettlerForEachParallel_WithNoEntities_DelegateIsNeverCalled()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(0, counter[0]);
    }

    [Fact]
    public void SettlerForEachParallel_WithOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position { X = 1 });

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(1, counter[0]);
    }

    [Fact]
    public void SettlerForEachParallel_WithMultipleEntities_AllEntitiesAreVisited()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        const int count = 2048;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position { X = i });

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(count, counter[0]);
    }

    [Fact]
    public void SettlerForEachParallel_CanMutateComponentData()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position { X = 1 });

        SetXForEach forEach = new() { NewX = 99 };

        query.ForEachParallel(forEach);

        Assert.Equal(99, world.Get<Position>(entity).X);
    }

    [Fact]
    public void SettlerForEachParallel_MutationsArePersistedAfterIteration()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        const int count = 2048;

        Entity[] entities = new Entity[count];

        for (int i = 0; i < count; i++)
            entities[i] = assemblage.Create(new Position { X = i });

        const int multiplier = 3;

        MultiplyXForEach forEach = new() { Multiplier = multiplier };

        query.ForEachParallel(forEach);

        for (int i = 0; i < count; i++)
            Assert.Equal(i * multiplier, world.Get<Position>(entities[i]).X);
    }

    [Fact]
    public void SettlerForEachParallel_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position { X = i });

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(count, counter[0]);
    }

    [Fact]
    public void SettlerForEachParallel_BeyondInitialChunkCapacity_MutationsAreCorrectAcrossAllChunks()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int count = assemblage._archetype._capacity * 3;

        Entity[] entities = new Entity[count];

        for (int i = 0; i < count; i++)
            entities[i] = assemblage.Create(new Position { X = i });

        const int multiplier = 2;

        MultiplyXForEach forEach = new() { Multiplier = multiplier };

        query.ForEachParallel(forEach);

        for (int i = 0; i < count; i++)
            Assert.Equal(i * multiplier, world.Get<Position>(entities[i]).X);
    }

    [Fact]
    public void SettlerForEachParallel_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position());

        int[] counter = new int[1];
        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void NomadForEachParallel_WithNoMatchedArchetypes_DelegateIsNeverCalled()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(0, counter[0]);
    }

    [Fact]
    public void NomadForEachParallel_WithOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position { X = 1 });

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(1, counter[0]);
    }

    [Fact]
    public void NomadForEachParallel_AcrossDistinctArchetypes_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();
        Assemblage<Position, Velocity> positionAndVelocity = world.CreateAssemblage<
            Position,
            Velocity
        >();

        const int perArchetype = 2048;

        for (int i = 0; i < perArchetype; i++)
        {
            positionOnly.Create(new Position { X = i });
            positionAndVelocity.Create(new Position { X = i }, new Velocity());
        }

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(perArchetype * 2, counter[0]);
    }

    [Fact]
    public void NomadForEachParallel_AcrossDistinctArchetypes_MutationsAreCorrectInEachArchetype()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();
        Assemblage<Position, Velocity> positionAndVelocity = world.CreateAssemblage<
            Position,
            Velocity
        >();

        const int perArchetype = 2048;

        Entity[] fromPositionOnly = new Entity[perArchetype];
        Entity[] fromPositionAndVelocity = new Entity[perArchetype];

        for (int i = 0; i < perArchetype; i++)
        {
            fromPositionOnly[i] = positionOnly.Create(new Position { X = i });

            fromPositionAndVelocity[i] = positionAndVelocity.Create(
                new Position { X = i },
                new Velocity()
            );
        }

        const int multiplier = 5;

        MultiplyXForEach forEach = new() { Multiplier = multiplier };

        query.ForEachParallel(forEach);

        for (int i = 0; i < perArchetype; i++)
        {
            Assert.Equal(i * multiplier, world.Get<Position>(fromPositionOnly[i]).X);
            Assert.Equal(i * multiplier, world.Get<Position>(fromPositionAndVelocity[i]).X);
        }
    }

    [Fact]
    public void NomadForEachParallel_AcrossDistinctArchetypes_BeyondChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();
        Assemblage<Position, Velocity> positionAndVelocity = world.CreateAssemblage<
            Position,
            Velocity
        >();

        int perArchetype = positionOnly._archetype._capacity * 2;

        for (int i = 0; i < perArchetype; i++)
        {
            positionOnly.Create(new Position { X = i });
            positionAndVelocity.Create(new Position { X = i }, new Velocity());
        }

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(perArchetype * 2, counter[0]);
    }

    [Fact]
    public void NomadForEachParallel_CanMutateComponentData()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position { X = 1 });

        SetXForEach forEach = new() { NewX = 55 };

        query.ForEachParallel(forEach);

        Assert.Equal(55, world.Get<Position>(entity).X);
    }

    [Fact]
    public void NomadForEachParallel_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position { X = i });

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(count, counter[0]);
    }

    [Fact]
    public void NomadForEachParallel_ArchetypeWithoutRequiredComponent_IsNotVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Velocity> velocityOnly = world.CreateAssemblage<Velocity>();

        velocityOnly.Create(new Velocity { X = 1 });

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.Equal(0, counter[0]);
    }

    [Fact]
    public void NomadForEachParallel_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position());

        int[] counter = new int[1];

        CountingForEach forEach = new() { CallCounter = counter };

        query.ForEachParallel(forEach);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void SettlerForEachEntityParallel_WithNoEntities_DelegateIsNeverCalled()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(0, counter[0]);
    }

    [Fact]
    public void SettlerForEachEntityParallel_WithOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position());

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(1, counter[0]);
    }

    [Fact]
    public void SettlerForEachEntityParallel_WithMultipleEntities_AllEntitiesAreVisited()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        const int count = 2048;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position { X = i });

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(count, counter[0]);
    }

    [Fact]
    public void SettlerForEachEntityParallel_CanMutateComponentData()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position { X = 1 });

        SetXForEachEntity forEach = new() { NewX = 42 };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(42, world.Get<Position>(entity).X);
    }

    [Fact]
    public void SettlerForEachEntityParallel_MutationsArePersistedAfterIteration()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        const int count = 2048;

        Entity[] entities = new Entity[count];

        for (int i = 0; i < count; i++)
            entities[i] = assemblage.Create(new Position { X = i });

        const int multiplier = 4;

        MultiplyXForEachEntity forEach = new() { Multiplier = multiplier };

        query.ForEachEntityParallel(forEach);

        for (int i = 0; i < count; i++)
            Assert.Equal(i * multiplier, world.Get<Position>(entities[i]).X);
    }

    [Fact]
    public void SettlerForEachEntityParallel_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position());

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(count, counter[0]);
    }

    [Fact]
    public void SettlerForEachEntityParallel_BeyondInitialChunkCapacity_MutationsAreCorrectAcrossAllChunks()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int count = assemblage._archetype._capacity * 3;

        Entity[] entities = new Entity[count];

        for (int i = 0; i < count; i++)
            entities[i] = assemblage.Create(new Position { X = i });

        const int multiplier = 2;

        MultiplyXForEachEntity forEach = new() { Multiplier = multiplier };

        query.ForEachEntityParallel(forEach);

        for (int i = 0; i < count; i++)
            Assert.Equal(i * multiplier, world.Get<Position>(entities[i]).X);
    }

    [Fact]
    public void SettlerForEachEntityParallel_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position());

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void NomadForEachEntityParallel_WithNoMatchedArchetypes_DelegateIsNeverCalled()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(0, counter[0]);
    }

    [Fact]
    public void NomadForEachEntityParallel_WithOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position());

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(1, counter[0]);
    }

    [Fact]
    public void NomadForEachEntityParallel_AcrossDistinctArchetypes_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();
        Assemblage<Position, Velocity> positionAndVelocity = world.CreateAssemblage<
            Position,
            Velocity
        >();

        const int perArchetype = 2048;

        for (int i = 0; i < perArchetype; i++)
        {
            positionOnly.Create(new Position { X = i });
            positionAndVelocity.Create(new Position { X = i }, new Velocity());
        }

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(perArchetype * 2, counter[0]);
    }

    [Fact]
    public void NomadForEachEntityParallel_AcrossDistinctArchetypes_MutationsAreCorrectInEachArchetype()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();
        Assemblage<Position, Velocity> positionAndVelocity = world.CreateAssemblage<
            Position,
            Velocity
        >();

        const int perArchetype = 2048;

        Entity[] fromPositionOnly = new Entity[perArchetype];
        Entity[] fromPositionAndVelocity = new Entity[perArchetype];

        for (int i = 0; i < perArchetype; i++)
        {
            fromPositionOnly[i] = positionOnly.Create(new Position { X = i });

            fromPositionAndVelocity[i] = positionAndVelocity.Create(
                new Position { X = i },
                new Velocity()
            );
        }

        const int multiplier = 5;

        MultiplyXForEachEntity forEach = new() { Multiplier = multiplier };

        query.ForEachEntityParallel(forEach);

        for (int i = 0; i < perArchetype; i++)
        {
            Assert.Equal(i * multiplier, world.Get<Position>(fromPositionOnly[i]).X);
            Assert.Equal(i * multiplier, world.Get<Position>(fromPositionAndVelocity[i]).X);
        }
    }

    [Fact]
    public void NomadForEachEntityParallel_AcrossDistinctArchetypes_BeyondChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();
        Assemblage<Position, Velocity> positionAndVelocity = world.CreateAssemblage<
            Position,
            Velocity
        >();

        int perArchetype = positionOnly._archetype._capacity * 2;

        for (int i = 0; i < perArchetype; i++)
        {
            positionOnly.Create(new Position { X = i });
            positionAndVelocity.Create(new Position { X = i }, new Velocity());
        }

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(perArchetype * 2, counter[0]);
    }

    [Fact]
    public void NomadForEachEntityParallel_CanMutateComponentData()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position { X = 1 });

        SetXForEachEntity forEach = new() { NewX = 77 };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(77, world.Get<Position>(entity).X);
    }

    [Fact]
    public void NomadForEachEntityParallel_ArchetypeWithoutRequiredComponent_IsNotVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Velocity> velocityOnly = world.CreateAssemblage<Velocity>();

        velocityOnly.Create(new Velocity { X = 1 });

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(0, counter[0]);
    }

    [Fact]
    public void NomadForEachEntityParallel_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position());

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.Equal(count, counter[0]);
    }

    [Fact]
    public void NomadForEachEntityParallel_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position());

        int[] counter = new int[1];

        CountingForEachEntity forEach = new() { CallCounter = counter };

        query.ForEachEntityParallel(forEach);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ParallelSystemsConstructor_WithOnlyOneSystem_Throws() =>
        Assert.Throws<InvalidOperationException>(() =>
        {
            TestParallelSystems _ = new(new TrackingSystem());
        });

    [Fact]
    public void ParallelSystemsConstructor_WithZeroSystems_Throws() =>
        Assert.Throws<InvalidOperationException>(() =>
        {
            TestParallelSystems _ = new();
        });

    [Fact]
    public void ParallelSystemsRun_WithTwoSystems_BothSystemsExecute()
    {
        TrackingSystem systemA = new();
        TrackingSystem systemB = new();

        TestParallelSystems parallel = new(systemA, systemB);

        parallel.Run();

        Assert.Equal(1, systemA.ExecuteCount);
        Assert.Equal(1, systemB.ExecuteCount);
    }

    [Fact]
    public void ParallelSystemsRun_WithManySystems_AllSystemsExecute()
    {
        const int count = 4;

        TrackingSystem[] systems = new TrackingSystem[count];

        for (int i = 0; i < count; i++)
            systems[i] = new TrackingSystem();

        TestParallelSystems parallel = new(systems);

        parallel.Run();

        for (int i = 0; i < count; i++)
            Assert.Equal(1, systems[i].ExecuteCount);
    }

    [Fact]
    public async Task ParallelSystemsRun_WithTwoSystems_ExecutionIsParallel()
    {
        Barrier barrier = new(participantCount: 2);

        BarrierSystem systemA = new() { Barrier = barrier };
        BarrierSystem systemB = new() { Barrier = barrier };

        TestParallelSystems parallel = new(systemA, systemB);

        await Task.Run(parallel.Run)
            .WaitAsync(TimeSpan.FromSeconds(5))
            .ContinueWith(
                (Task task) =>
                {
                    Assert.True(task.IsCompleted);
                }
            );

        Assert.Equal(1, systemA.ExecuteCount);
        Assert.Equal(1, systemB.ExecuteCount);
    }

    [Fact]
    public void ParallelSystemsRun_CallsBeforeBeforeSystems()
    {
        long[] beforeTimestamp = new long[1];

        TimestampSystem system = new();
        TrackingSystem other = new();

        RecordingParallelSystems parallel = new(
            beforeAction: () => Volatile.Write(ref beforeTimestamp[0], Environment.TickCount64),
            afterAction: null,
            systems: [system, other]
        );

        parallel.Before();
        parallel.Run();

        Assert.True(beforeTimestamp[0] > 0);
        Assert.True(beforeTimestamp[0] <= system.StartedAtTicks);
    }

    [Fact]
    public void ParallelSystemsRun_CallsAfterAfterAllSystemsComplete()
    {
        int afterCallCount = 0;
        int systemsCompletedBeforeAfter = 0;

        TrackingSystem systemA = new();
        TrackingSystem systemB = new();

        RecordingParallelSystems parallel = new(
            beforeAction: null,
            afterAction: () =>
            {
                systemsCompletedBeforeAfter = systemA.ExecuteCount + systemB.ExecuteCount;
                Interlocked.Increment(ref afterCallCount);
            },
            systems: [systemA, systemB]
        );

        parallel.Before();
        parallel.Run();
        parallel.After();

        Assert.Equal(1, afterCallCount);
        Assert.Equal(2, systemsCompletedBeforeAfter);
    }
}

file struct CountingForEach : IForEach<Position>
{
    public int[] CallCounter;

    public void Execute(ref Position c1) => Interlocked.Increment(ref CallCounter[0]);
}

file struct SetXForEach : IForEach<Position>
{
    public int NewX;

    public void Execute(ref Position c1) => c1.X = NewX;
}

file struct MultiplyXForEach : IForEach<Position>
{
    public int Multiplier;

    public void Execute(ref Position c1) => c1.X *= Multiplier;
}

file struct CountingForEachEntity : IForEachEntity<Position>
{
    public int[] CallCounter;

    public void Execute(Entity entity, ref Position c1) =>
        Interlocked.Increment(ref CallCounter[0]);
}

file struct SetXForEachEntity : IForEachEntity<Position>
{
    public int NewX;

    public void Execute(Entity entity, ref Position c1) => c1.X = NewX;
}

file struct MultiplyXForEachEntity : IForEachEntity<Position>
{
    public int Multiplier;

    public void Execute(Entity entity, ref Position c1) => c1.X *= Multiplier;
}

file sealed class TestParallelSystems : ParallelSystems
{
    public TestParallelSystems(params ReadOnlySpan<BaseSystem> systems)
        : base(systems) { }
}

file sealed class TrackingSystem : Theia.ECS.Systems.System
{
    public int ExecuteCount;

    public override void Execute() => Interlocked.Increment(ref ExecuteCount);
}

file sealed class BarrierSystem : Theia.ECS.Systems.System
{
    public Barrier? Barrier;
    public int ExecuteCount;

    public override void Execute()
    {
        Barrier!.SignalAndWait(TimeSpan.FromSeconds(5));
        Interlocked.Increment(ref ExecuteCount);
    }
}

file sealed class TimestampSystem : Theia.ECS.Systems.System
{
    public long StartedAtTicks;

    public override void Execute() => Volatile.Write(ref StartedAtTicks, Environment.TickCount64);
}

file sealed class RecordingParallelSystems : ParallelSystems
{
    private readonly Action? _beforeAction;
    private readonly Action? _afterAction;

    public RecordingParallelSystems(Action? beforeAction, Action? afterAction, BaseSystem[] systems)
        : base(systems)
    {
        _beforeAction = beforeAction;
        _afterAction = afterAction;
    }

    public override void Before() => _beforeAction?.Invoke();

    public override void After() => _afterAction?.Invoke();
}
