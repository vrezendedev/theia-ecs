using System;
using System.Threading.Tasks;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldDeferredTests
{
    private static async Task<Entity[]> CreateEntitiesAndRunConcurrently(
        Assemblage<Position> assemblage,
        Action<Entity[], int, int> onEachThread,
        Action<Entity[]>? onEachEntityCreated = null
    )
    {
        const int threadCount = 8;
        const int commandsPerThread = 64;

        Entity[] entities = new Entity[threadCount * commandsPerThread];

        for (int i = 0; i < entities.Length; i++)
            entities[i] = assemblage.Create(new Position());

        onEachEntityCreated?.Invoke(entities);

        Task[] tasks = new Task[threadCount];

        for (int t = 0; t < threadCount; t++)
        {
            int offset = t * commandsPerThread;

            tasks[t] = Task.Run(() =>
            {
                for (int i = 0; i < commandsPerThread; i++)
                    onEachThread(entities, offset, i);
            });
        }

        await Task.WhenAll(tasks);

        return entities;
    }

    [Fact]
    public void DeferredGhoulify_BeforeFlush_EntityRemainsAlive()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredGhoulify(entity);

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void DeferredGhoulify_AfterFlush_EntityIsGhoul()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredGhoulify(entity);

        world.FlushDeferred();

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void DeferredGhoulify_WithGhoul_AfterFlush_DoesNotThrow()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        world.DeferredGhoulify(entity);

        world.FlushDeferred();
    }

    [Fact]
    public void DeferredAdd_BeforeFlush_EntityDoesNotHaveComponent()
    {
        World world = new();
        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredAdd<Velocity>(entity);

        Assert.Throws<InvalidOperationException>(() => world.Get<Velocity>(entity));
    }

    [Fact]
    public void DeferredAdd_AfterFlush_EntityHasComponent()
    {
        World world = new();
        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredAdd<Velocity>(entity);

        world.FlushDeferred();

        Assert.True(world.TryRemoveComponent<Velocity>(entity));
    }

    [Fact]
    public void DeferredAdd_AfterFlush_ComponentDataIsCorrect()
    {
        World world = new();
        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredAdd(entity, new Velocity { X = 4, Y = 8 });
        world.FlushDeferred();

        ref Velocity velocity = ref world.Get<Velocity>(entity);

        Assert.Equal(4, velocity.X);
        Assert.Equal(8, velocity.Y);
    }

    [Fact]
    public void DeferredAdd_ToDeadEntity_AfterFlush_IsDiscardedSilently()
    {
        World world = new();
        Entity entity = world.CreateAssemblage<Position>().Create(new Position());
        world.TryGhoulify(entity);

        world.DeferredAdd<Velocity>(entity);
        world.FlushDeferred();
    }

    [Fact]
    public void DeferredAdd_ComponentAlreadyPresent_AfterFlush_IsDiscardedSilently()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredAdd(entity, new Position { X = 99 });

        world.FlushDeferred();

        Assert.Equal(0, world.Get<Position>(entity).X);
    }

    [Fact]
    public void DeferredRemove_BeforeFlush_EntityStillHasComponent()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryAddComponent<Velocity>(entity);

        world.DeferredRemove<Velocity>(entity);

        Assert.Equal(0, world.Get<Velocity>(entity).X);
    }

    [Fact]
    public void DeferredRemove_AfterFlush_ComponentIsGone()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryAddComponent<Velocity>(entity);

        world.DeferredRemove<Velocity>(entity);

        world.FlushDeferred();

        Assert.Throws<InvalidOperationException>(() => world.Get<Velocity>(entity));
    }

    [Fact]
    public void DeferredRemove_LastComponent_AfterFlush_GhoulifiesEntity()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredRemove<Position>(entity);

        world.FlushDeferred();

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void DeferredRemove_DeadEntity_AfterFlush_DoesNotThrow()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        world.DeferredRemove<Position>(entity);

        world.FlushDeferred();
    }

    [Fact]
    public void FlushDeferred_EmptyQueues_DoesNotThrow()
    {
        World world = new();

        world.FlushDeferred();
    }

    [Fact]
    public void FlushDeferred_ProcessesAllQueuesInOrder()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entityA = assemblage.Create(new Position());
        Entity entityB = assemblage.Create(new Position());

        world.DeferredGhoulify(entityA);
        world.DeferredAdd(entityB, new Velocity { X = 5 });
        world.DeferredRemove<Position>(entityB);

        world.FlushDeferred();

        Assert.False(world.IsAlive(entityA));
        Assert.Equal(5, world.Get<Velocity>(entityB).X);
        Assert.Throws<InvalidOperationException>(() => world.Get<Position>(entityB));
    }

    [Fact]
    public void FlushDeferred_CalledTwice_SecondFlushDoesNotApplyCommandsAgain()
    {
        World world = new();
        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.DeferredAdd<Velocity>(entity);
        world.FlushDeferred();

        int countAfterFirstFlush = world.CountEntities();
        world.FlushDeferred();

        Assert.Equal(countAfterFirstFlush, world.CountEntities());
        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public async Task DeferredGhoulify_ConcurrentEnqueue_AllCommandsAreProcessed()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity[] entities = await CreateEntitiesAndRunConcurrently(
            assemblage,
            (entities, offset, i) => world.DeferredGhoulify(entities[offset + i])
        );

        world.FlushDeferred();

        foreach (Entity entity in entities)
            Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public async Task DeferredAdd_ConcurrentEnqueue_AllCommandsAreProcessed()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity[] entities = await CreateEntitiesAndRunConcurrently(
            assemblage,
            (entities, offset, i) =>
                world.DeferredAdd(entities[offset + i], new Velocity { X = offset + i })
        );

        world.FlushDeferred();

        for (int i = 0; i < entities.Length; i++)
            Assert.Equal(i, world.Get<Velocity>(entities[i]).X);
    }

    [Fact]
    public async Task DeferredRemove_ConcurrentEnqueue_AllCommandsAreProcessed()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity[] entities = await CreateEntitiesAndRunConcurrently(
            assemblage,
            (entities, offset, i) => world.DeferredRemove<Velocity>(entities[offset + i]),
            entities =>
            {
                foreach (Entity entity in entities)
                    world.TryAddComponent<Velocity>(entity);
            }
        );

        world.FlushDeferred();

        foreach (Entity entity in entities)
            Assert.Throws<InvalidOperationException>(() => world.Get<Velocity>(entity));
    }
}
