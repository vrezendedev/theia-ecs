using System;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldQueriesTest
{
    [Fact]
    public void CreateSettlerQuery_ReturnsNonNullInstance()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Assert.NotNull(query);
    }

    [Fact]
    public void CreateNomadQuery_ReturnsNonNullInstance()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assert.NotNull(query);
    }

    [Fact]
    public void CreateSettlerQuery_WhenQueriesExecuting_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        world.IncrementQueriesBeingExecuted();

        Assert.Throws<InvalidOperationException>(() => world.CreateSettlerQuery(assemblage));
    }

    [Fact]
    public void CreateNomadQuery_WhenQueriesExecuting_Throws()
    {
        World world = new();

        world.IncrementQueriesBeingExecuted();

        Assert.Throws<InvalidOperationException>(() => world.CreateNomadQuery<Position>());
    }

    [Fact]
    public void CreateNomadQuery_BackfillsExistingMatchingArchetypes()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.TryCreate(new Position { X = 1 });

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void CreateNomadQuery_PicksUpArchetypesCreatedAfterRegistration()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.TryCreate(new Position { X = 1 });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void ForEach_WithSettler_WithNoEntities_DelegateIsNeverCalled()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void ForEach_WithNomad_WithNoMatchedArchetypes_DelegateIsNeverCalled()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void ForEach_WithSettlerAndOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.TryCreate(new Position { X = 1 });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void ForEach_WithNomadAndOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.TryCreate(new Position { X = 1 });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void ForEach_WithMultipleEntities_DelegateIsCalledForEach()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        const int count = 10;

        for (int i = 0; i < count; i++)
            assemblage.TryCreate(new Position { X = i });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void ForEach_WithSettler_CanMutateComponentData()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.TryCreate(new Position { X = 1, Y = 2 });

        query.ForEach(
            (ref Position position) =>
            {
                position.X = 99;
                position.Y = 77;
            }
        );

        Assert.Equal(99, world.Get<Position>(entity).X);
        Assert.Equal(77, world.Get<Position>(entity).Y);
    }

    [Fact]
    public void ForEach_WithNomad_CanMutateComponentData()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.TryCreate(new Position { X = 1 });

        query.ForEach((ref Position position) => position.X = 55);

        Assert.Equal(55, world.Get<Position>(entity).X);
    }

    [Fact]
    public void ForEach_MutationsArePersistedAfterIteration()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        const int count = 5;

        Entity[] entities = new Entity[count];

        for (int i = 0; i < count; i++)
            entities[i] = assemblage.TryCreate(new Position { X = i });

        query.ForEach((ref Position position) => position.X *= 2);

        for (int i = 0; i < count; i++)
            Assert.Equal(i * 2, world.Get<Position>(entities[i]).X);
    }

    [Fact]
    public void ForEach_AcrossMultipleMatchingArchetypes_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();

        Entity entityA = positionOnly.TryCreate(new Position { X = 1 });

        Assert.True(world.TryAdd<Velocity>(entityA));

        Entity entityB = positionOnly.TryCreate(new Position { X = 2 });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(2, callCount);
    }

    [Fact]
    public void ForEach_ArchetypeWithMoreComponentsThanSignature_IsStillMatched()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.TryCreate(new Position { X = 3 });

        world.TryAdd<Velocity>(entity);

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void ForEach_ArchetypeWithoutRequiredComponent_IsNotMatched()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Velocity> velocityOnly = world.CreateAssemblage<Velocity>();

        velocityOnly.TryCreate(new Velocity { X = 1 });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void ForEach_WithSettler_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.TryCreate(new Position());

        query.ForEach((ref Position _) => { });

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEach_WithNomad_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.TryCreate(new Position());

        query.ForEach((ref Position _) => { });

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEach_WithSettler_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.TryCreate(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            query.ForEach((ref Position _) => world.TryAdd<Velocity>(entity))
        );
    }

    [Fact]
    public void ForEach_WithNomad_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.TryCreate(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            query.ForEach((ref Position _) => world.TryAdd<Velocity>(entity))
        );
    }

    [Fact]
    public void ForEach_WithSettler_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.TryCreate(new Position { X = i });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void ForEach_WithNomad_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.TryCreate(new Position { X = i });

        int callCount = 0;

        query.ForEach((ref Position _) => callCount++);

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_WithNoEntities_DelegateIsNeverCalled()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int callCount = 0;

        query.ForEachEntity((Entity _, ref Position __) => callCount++);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void ForEachEntity_WithNomad_WithNoMatchedArchetypes_DelegateIsNeverCalled()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        int callCount = 0;

        query.ForEachEntity((Entity _, ref Position __) => callCount++);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_WithOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.TryCreate(new Position());

        int callCount = 0;

        query.ForEachEntity((Entity _, ref Position __) => callCount++);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_ProvidesCorrectEntityAndComponent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity created = assemblage.TryCreate(new Position { X = 5, Y = 10 });

        Entity receivedEntity = default;

        float receivedX = 0;

        query.ForEachEntity(
            (Entity entity, ref Position position) =>
            {
                receivedEntity = entity;
                receivedX = position.X;
            }
        );

        Assert.Equal(created._id, receivedEntity._id);
        Assert.Equal(5, receivedX);
    }

    [Fact]
    public void ForEachEntity_WithNomad_ProvidesCorrectEntityAndComponent()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity created = assemblage.TryCreate(new Position { X = 7, Y = 3 });

        Entity receivedEntity = default;

        float receivedX = 0;

        query.ForEachEntity(
            (Entity entity, ref Position position) =>
            {
                receivedEntity = entity;
                receivedX = position.X;
            }
        );

        Assert.Equal(created._id, receivedEntity._id);
        Assert.Equal(7, receivedX);
    }

    [Fact]
    public void ForEachEntity_WithSettler_CanMutateComponentData()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.TryCreate(new Position { X = 1 });

        query.ForEachEntity((Entity _, ref Position position) => position.X = 42);

        Assert.Equal(42, world.Get<Position>(entity).X);
    }

    [Fact]
    public void ForEachEntity_AcrossMultipleMatchingArchetypes_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();

        Entity entityA = positionOnly.TryCreate(new Position { X = 1 });

        Assert.True(world.TryAdd<Velocity>(entityA));

        Entity entityB = positionOnly.TryCreate(new Position { X = 2 });

        int callCount = 0;

        query.ForEachEntity((Entity _, ref Position __) => callCount++);

        Assert.Equal(2, callCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.TryCreate(new Position());

        query.ForEachEntity((Entity _, ref Position __) => { });

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEachEntity_WithNomad_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.TryCreate(new Position());

        query.ForEachEntity((Entity _, ref Position __) => { });

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEachEntity_WithSettler_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.TryCreate(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            query.ForEachEntity((Entity _, ref Position __) => world.TryGhoulify(entity))
        );
    }

    [Fact]
    public void ForEachEntity_WithNomad_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.TryCreate(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            query.ForEachEntity((Entity _, ref Position __) => world.TryGhoulify(entity))
        );
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredCreateInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.TryCreate(new Position());

        int countBefore = world.CountEntities();

        query.ForEachEntity(
            (Entity _, ref Position __) => assemblage.DeferredCreate(new Position { X = 1 })
        );

        world.FlushDeferred();

        Assert.Equal(countBefore + 1, world.CountEntities());
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredCreateInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.TryCreate(new Position());

        int countBefore = world.CountEntities();

        query.ForEachEntity(
            (Entity _, ref Position _2) => assemblage.DeferredCreate(new Position { X = 1 })
        );

        world.FlushDeferred();

        Assert.Equal(countBefore + 1, world.CountEntities());
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredGhoulifyInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.TryCreate(new Position());

        query.ForEachEntity((Entity e, ref Position _) => world.DeferredGhoulify(e));

        world.FlushDeferred();

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredGhoulifyInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.TryCreate(new Position());

        query.ForEachEntity((Entity e, ref Position _) => world.DeferredGhoulify(e));

        world.FlushDeferred();

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredAddInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.TryCreate(new Position());

        query.ForEachEntity((Entity e, ref Position _) => world.DeferredAdd<Velocity>(e));

        world.FlushDeferred();

        Assert.True(world.Has<Velocity>(entity));
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredAddInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.TryCreate(new Position());

        query.ForEachEntity((Entity e, ref Position _) => world.DeferredAdd<Velocity>(e));

        world.FlushDeferred();

        Assert.True(world.Has<Velocity>(entity));
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredRemoveInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.TryCreate(new Position());

        query.ForEachEntity((Entity e, ref Position _) => world.DeferredRemove<Position>(e));

        world.FlushDeferred();

        Assert.False(world.Has<Position>(entity));
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredRemoveInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.TryCreate(new Position());

        world.TryAdd<Velocity>(entity);

        query.ForEachEntity((Entity e, ref Position _) => world.DeferredRemove<Velocity>(e));

        world.FlushDeferred();

        Assert.False(world.Has<Velocity>(entity));
    }
}
