using System;
using System.Threading.Tasks;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Assemblages;

public sealed class AssemblageTests
{
    [Fact]
    public void CreateAssemblage_ReturnsNonNullInstance()
    {
        Assemblage<Position> assemblage = new World().CreateAssemblage<Position>();

        Assert.NotNull(assemblage);
    }

    [Fact]
    public void CreateAssemblage_AssemblageSignatureContainsRequestedComponent()
    {
        Assemblage<Position> assemblage = new World().CreateAssemblage<Position>();

        Assert.True(assemblage._signature.IsSatisfiedBy(assemblage._archetype._signature));
    }

    [Fact]
    public void CreateAssemblage_AssemblageArchetypeHasRequestedComponent()
    {
        Assemblage<Position> assemblage = new World().CreateAssemblage<Position>();

        Assert.True(assemblage._archetype.Has<Position>());
    }

    [Fact]
    public void CreateAssemblage_ComponentStorageMappingHasOneEntry()
    {
        Assemblage<Position> assemblage = new World().CreateAssemblage<Position>();

        Assert.Equal(1, assemblage.GetComponentStorageMapping().Length);
    }

    [Fact]
    public void CreateAssemblage_SetsMatchedAssemblageOnArchetype()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Assert.Same(assemblage, assemblage._archetype.GetMatchedAssemblage());
    }

    [Fact]
    public void CreateAssemblage_CalledTwiceForSameType_Throws()
    {
        World world = new();

        world.CreateAssemblage<Position>();

        Assert.Throws<InvalidOperationException>(() => world.CreateAssemblage<Position>());
    }

    [Fact]
    public void CreateAssemblage_DifferentTypes_EachArchetypeHasOwnMatchedAssemblage()
    {
        World world = new();

        Assemblage<Position> positionAssemblage = world.CreateAssemblage<Position>();
        Assemblage<Velocity> velocityAssemblage = world.CreateAssemblage<Velocity>();

        Assert.Same(positionAssemblage, positionAssemblage._archetype.GetMatchedAssemblage());
        Assert.Same(velocityAssemblage, velocityAssemblage._archetype.GetMatchedAssemblage());
    }

    [Fact]
    public void Create_ReturnsAliveEntity()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position { X = 1, Y = 2 });

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void Create_ComponentDataIsStoredCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position { X = 5, Y = 10 });

        ref Position position = ref world.Get<Position>(entity);

        Assert.Equal(5, position.X);
        Assert.Equal(10, position.Y);
    }

    [Fact]
    public void Create_IncreasesEntityCount()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int countBefore = world.CountEntitiesAlive();
        assemblage.Create(new Position { X = 1, Y = 2 });

        Assert.Equal(countBefore + 1, world.CountEntitiesAlive());
    }

    [Fact]
    public void Create_MultipleEntities_ReturnDistinctEntities()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entityA = assemblage.Create(new Position { X = 1 });
        Entity entityB = assemblage.Create(new Position { X = 2 });

        Assert.NotEqual(entityA._id, entityB._id);
    }

    [Fact]
    public void Create_MultipleEntities_ComponentDataIsIndependent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entityA = assemblage.Create(new Position { X = 1, Y = 2 });
        Entity entityB = assemblage.Create(new Position { X = 3, Y = 4 });

        ref Position positionA = ref world.Get<Position>(entityA);
        ref Position positionB = ref world.Get<Position>(entityB);

        Assert.Equal(1, positionA.X);
        Assert.Equal(2, positionA.Y);
        Assert.Equal(3, positionB.X);
        Assert.Equal(4, positionB.Y);
    }

    [Fact]
    public void Create_DifferentAssemblages_EntitiesAreBothAlive()
    {
        World world = new();

        Assemblage<Position> positionAssemblage = world.CreateAssemblage<Position>();
        Assemblage<Velocity> velocityAssemblage = world.CreateAssemblage<Velocity>();

        Entity positionEntity = positionAssemblage.Create(new Position { X = 1 });
        Entity velocityEntity = velocityAssemblage.Create(new Velocity { X = 2 });

        Assert.True(world.IsAlive(positionEntity));
        Assert.True(world.IsAlive(velocityEntity));
    }

    [Fact]
    public void DeferredCreate_BeforeFlush_EntityCountDoesNotIncrease()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int countBefore = world.CountEntitiesAlive();
        assemblage.DeferredCreate(new Position { X = 1, Y = 2 });

        Assert.Equal(countBefore, world.CountEntitiesAlive());
    }

    [Fact]
    public void DeferredCreate_AfterFlush_EntityCountIncreases()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int countBefore = world.CountEntitiesAlive();
        assemblage.DeferredCreate(new Position { X = 1, Y = 2 });
        world.FlushDeferred();

        Assert.Equal(countBefore + 1, world.CountEntitiesAlive());
    }

    [Fact]
    public void DeferredCreate_AfterFlush_ComponentDataIsCorrect()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int x = 7;
        int y = 13;

        Entity prior = assemblage.Create(new Position { X = 0 });
        assemblage.DeferredCreate(new Position { X = x, Y = y });
        world.FlushDeferred();

        Entity deferred = new Entity() { _id = prior._id + 1, _version = 1 };

        Assert.Equal(x, world.Get<Position>(deferred).X);
        Assert.Equal(y, world.Get<Position>(deferred).Y);
    }

    [Fact]
    public void DeferredCreate_Multiple_AfterFlush_AllEntitiesAreCreated()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int countBefore = world.CountEntitiesAlive();
        const int deferredCount = 5;

        for (int i = 0; i < deferredCount; i++)
            assemblage.DeferredCreate(new Position { X = i });

        world.FlushDeferred();

        Assert.Equal(countBefore + deferredCount, world.CountEntitiesAlive());
    }

    [Fact]
    public void CreateAssemblage_ComponentStorageMappingIndexMatchesArchetypeStorageIndex()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int mappedIndex = assemblage.GetComponentStorageMapping()[0];
        int archetypeIndex = assemblage._archetype.GetStorageIndex(ComponentMeta<Position>.s_id);

        Assert.Equal(archetypeIndex, mappedIndex);
    }

    [Fact]
    public async Task DeferredCreate_ConcurrentEnqueue_AllEntitiesCreatedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        const int threadCount = 8;
        const int commandsPerThread = 64;

        int expectedTotal = threadCount * commandsPerThread;

        int countBefore = world.CountEntitiesAlive();

        Task[] tasks = new Task[threadCount];

        for (int t = 0; t < threadCount; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                for (int i = 0; i < commandsPerThread; i++)
                    assemblage.DeferredCreate(new Position { X = i });
            });
        }

        await Task.WhenAll(tasks);

        world.FlushDeferred();

        Assert.Equal(countBefore + expectedTotal, world.CountEntitiesAlive());
    }
}
