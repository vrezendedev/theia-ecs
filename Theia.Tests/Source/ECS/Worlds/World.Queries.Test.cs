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

        assemblage.Create(new Position { X = 1 });

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(1, forEachPosition.CallCount);
    }

    [Fact]
    public void CreateNomadQuery_PicksUpArchetypesCreatedAfterRegistration()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position { X = 1 });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(1, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithSettler_WithNoEntities_DelegateIsNeverCalled()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(0, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithNomad_WithNoMatchedArchetypes_DelegateIsNeverCalled()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(0, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithSettlerAndOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position { X = 1 });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(1, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithNomadAndOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position { X = 1 });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(1, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithMultipleEntities_DelegateIsCalledForEach()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        const int count = 10;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position { X = i });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(count, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithSettler_CanMutateComponentData()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position { X = 1, Y = 2 });

        ForEachPosition forEachPosition = new() { X = 99, Y = 77 };

        query.ForEach(ref forEachPosition);

        Assert.Equal(99, world.Get<Position>(entity).X);
        Assert.Equal(77, world.Get<Position>(entity).Y);
    }

    [Fact]
    public void ForEach_WithNomad_CanMutateComponentData()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position { X = 1 });

        ForEachPosition forEachPosition = new() { X = 55 };

        query.ForEach(ref forEachPosition);

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
            entities[i] = assemblage.Create(new Position { X = i });

        ForEachPositionMultiply forEachPositionMultiply = new() { XMultiplier = 2 };

        query.ForEach(ref forEachPositionMultiply);

        for (int i = 0; i < count; i++)
            Assert.Equal(i * 2, world.Get<Position>(entities[i]).X);
    }

    [Fact]
    public void ForEach_AcrossMultipleMatchingArchetypes_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();

        Entity entityA = positionOnly.Create(new Position { X = 1 });

        Assert.True(world.TryAddComponent<Velocity>(entityA));

        positionOnly.Create(new Position { X = 2 });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(2, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_ArchetypeWithMoreComponentsThanSignature_IsStillMatched()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position { X = 3 });

        world.TryAddComponent<Velocity>(entity);

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(1, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_ArchetypeWithoutRequiredComponent_IsNotMatched()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Velocity> velocityOnly = world.CreateAssemblage<Velocity>();

        velocityOnly.Create(new Velocity { X = 1 });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(0, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithSettler_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position());

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEach_WithNomad_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position());

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEach_WithSettler_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() =>
        {
            AttemptAddForEachEntityPosition attemptAddForEachEntityPosition = new()
            {
                World = world,
            };

            query.ForEachEntity(ref attemptAddForEachEntityPosition);
        });
    }

    [Fact]
    public void ForEach_WithNomad_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        AttemptAddForEachEntityPosition attemptAddForEachEntityPosition = new() { World = world };

        Assert.Throws<InvalidOperationException>(() =>
        {
            AttemptAddForEachEntityPosition attemptAddForEachEntityPosition = new()
            {
                World = world,
            };
            query.ForEachEntity(ref attemptAddForEachEntityPosition);
        });
    }

    [Fact]
    public void ForEach_WithSettler_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position { X = i });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(count, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEach_WithNomad_BeyondInitialChunkCapacity_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int count = assemblage._archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position { X = i });

        ForEachPosition forEachPosition = new();

        query.ForEach(ref forEachPosition);

        Assert.Equal(count, forEachPosition.CallCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_WithNoEntities_DelegateIsNeverCalled()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        ForEachEntityPosition forEachEntityPosition = new();

        query.ForEachEntity(ref forEachEntityPosition);

        Assert.Equal(0, forEachEntityPosition.CallCount);
    }

    [Fact]
    public void ForEachEntity_WithNomad_WithNoMatchedArchetypes_DelegateIsNeverCalled()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        ForEachEntityPosition forEachEntityPosition = new();

        query.ForEachEntity(ref forEachEntityPosition);

        Assert.Equal(0, forEachEntityPosition.CallCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_WithOneEntity_DelegateIsCalledOnce()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position());

        ForEachEntityPosition forEachEntityPosition = new();

        query.ForEachEntity(ref forEachEntityPosition);

        Assert.Equal(1, forEachEntityPosition.CallCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_ProvidesCorrectEntityAndComponent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity created = assemblage.Create(new Position { X = 5, Y = 10 });

        CaptureEntityForEachEntityPosition captureEntityForEachEntityPosition = new();

        query.ForEachEntity(ref captureEntityForEachEntityPosition);

        Assert.Equal(created._id, captureEntityForEachEntityPosition.Entity._id);
        Assert.Equal(5, captureEntityForEachEntityPosition.Position.X);
    }

    [Fact]
    public void ForEachEntity_WithNomad_ProvidesCorrectEntityAndComponent()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity created = assemblage.Create(new Position { X = 7, Y = 3 });

        CaptureEntityForEachEntityPosition captureEntityForEachEntityPosition = new();

        query.ForEachEntity(ref captureEntityForEachEntityPosition);

        Assert.Equal(created._id, captureEntityForEachEntityPosition.Entity._id);
        Assert.Equal(7, captureEntityForEachEntityPosition.Position.X);
    }

    [Fact]
    public void ForEachEntity_WithSettler_CanMutateComponentData()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position { X = 1 });

        ForEachEntityPosition forEachPosition = new() { X = 42 };

        query.ForEachEntity(ref forEachPosition);

        Assert.Equal(42, world.Get<Position>(entity).X);
    }

    [Fact]
    public void ForEachEntity_AcrossMultipleMatchingArchetypes_AllEntitiesAreVisited()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();

        Entity entityA = positionOnly.Create(new Position { X = 1 });

        Assert.True(world.TryAddComponent<Velocity>(entityA));

        positionOnly.Create(new Position { X = 2 });

        ForEachEntityPosition forEachEntityPosition = new();

        query.ForEachEntity(ref forEachEntityPosition);

        Assert.Equal(2, forEachEntityPosition.CallCount);
    }

    [Fact]
    public void ForEachEntity_WithSettler_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position());

        ForEachEntityPosition forEachEntityPosition = new();

        query.ForEachEntity(ref forEachEntityPosition);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEachEntity_WithNomad_DecrementsQueryCountAfterExecution()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position());

        ForEachEntityPosition forEachEntityPosition = new();

        query.ForEachEntity(ref forEachEntityPosition);

        Assert.False(world.AreThereAnyQueriesBeingExecuted());
    }

    [Fact]
    public void ForEachEntity_WithSettler_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() =>
        {
            AttemptGhoulifyForEachEntityPosition attemptGhoulify = new()
            {
                World = world,
                Target = entity,
            };

            query.ForEachEntity(ref attemptGhoulify);
        });
    }

    [Fact]
    public void ForEachEntity_WithNomad_StructuralMutationDuringExecution_Throws()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() =>
        {
            AttemptGhoulifyForEachEntityPosition attemptGhoulify = new()
            {
                World = world,
                Target = entity,
            };
            query.ForEachEntity(ref attemptGhoulify);
        });
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredCreateInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position());

        int countBefore = world.CountEntities();

        DeferredCreateForEachEntityPosition deferredCreate = new() { Assemblage = assemblage };

        query.ForEachEntity(ref deferredCreate);

        world.FlushDeferred();

        Assert.Equal(countBefore + 1, world.CountEntities());
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredCreateInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        assemblage.Create(new Position());

        int countBefore = world.CountEntities();

        DeferredCreateForEachEntityPosition deferredCreate = new() { Assemblage = assemblage };

        query.ForEachEntity(ref deferredCreate);

        world.FlushDeferred();

        Assert.Equal(countBefore + 1, world.CountEntities());
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredGhoulifyInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position());

        DeferredGhoulifyForEachEntityPosition deferredGhoulify = new() { World = world };

        query.ForEachEntity(ref deferredGhoulify);

        world.FlushDeferred();

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredGhoulifyInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        DeferredGhoulifyForEachEntityPosition deferredGhoulify = new() { World = world };

        query.ForEachEntity(ref deferredGhoulify);

        world.FlushDeferred();

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredAddInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position());

        DeferredAddVelocityForEachEntityPosition deferredAdd = new() { World = world };

        query.ForEachEntity(ref deferredAdd);

        world.FlushDeferred();

        Assert.True(world.Has<Velocity>(entity));
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredAddInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        DeferredAddVelocityForEachEntityPosition deferredAdd = new() { World = world };

        query.ForEachEntity(ref deferredAdd);

        world.FlushDeferred();

        Assert.True(world.Has<Velocity>(entity));
    }

    [Fact]
    public void ForEachEntity_WithSettler_DeferredRemoveInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(new Position());

        DeferredRemovePositionForEachEntityPosition deferredRemove = new() { World = world };

        query.ForEachEntity(ref deferredRemove);

        world.FlushDeferred();

        Assert.False(world.Has<Position>(entity));
    }

    [Fact]
    public void ForEachEntity_WithNomad_DeferredRemoveInsideQuery_AppliedAfterFlush()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        world.TryAddComponent<Velocity>(entity);

        DeferredRemoveVelocityForEachEntityPosition deferredRemove = new() { World = world };

        query.ForEachEntity(ref deferredRemove);

        world.FlushDeferred();

        Assert.False(world.Has<Velocity>(entity));
    }
}

file ref struct ForEachPosition : IForEach<Position>
{
    public int CallCount;
    public int X;
    public int Y;

    public void Execute(ref Position c1)
    {
        CallCount++;
        c1.X = X;
        c1.Y = Y;
    }
}

file ref struct ForEachPositionMultiply : IForEach<Position>
{
    public int XMultiplier;

    public void Execute(ref Position c1)
    {
        c1.X *= XMultiplier;
    }
}

file ref struct ForEachEntityPosition : IForEachEntity<Position>
{
    public int CallCount;
    public int X;

    public void Execute(Entity entity, ref Position c1)
    {
        CallCount++;
        c1.X = X;
    }
}

file ref struct AttemptAddForEachEntityPosition : IForEachEntity<Position>
{
    public World World;

    public void Execute(Entity entity, ref Position c1)
    {
        World.TryAddComponent(entity, new Velocity());
    }
}

file ref struct AttemptGhoulifyForEachEntityPosition : IForEachEntity<Position>
{
    public World World;
    public Entity Target;

    public void Execute(Entity entity, ref Position c1)
    {
        World.TryGhoulify(Target);
    }
}

file ref struct CaptureEntityForEachEntityPosition : IForEachEntity<Position>
{
    public Entity Entity;
    public Position Position;

    public void Execute(Entity entity, ref Position c1)
    {
        Entity = entity;
        Position = c1;
    }
}

file ref struct DeferredCreateForEachEntityPosition : IForEachEntity<Position>
{
    public Assemblage<Position> Assemblage;

    public void Execute(Entity entity, ref Position c1)
    {
        Assemblage.DeferredCreate(new Position { X = 1 });
    }
}

file ref struct DeferredGhoulifyForEachEntityPosition : IForEachEntity<Position>
{
    public World World;

    public void Execute(Entity entity, ref Position c1)
    {
        World.DeferredGhoulify(entity);
    }
}

file ref struct DeferredAddVelocityForEachEntityPosition : IForEachEntity<Position>
{
    public World World;

    public void Execute(Entity entity, ref Position c1)
    {
        World.DeferredAddComponent<Velocity>(entity);
    }
}

file ref struct DeferredRemovePositionForEachEntityPosition : IForEachEntity<Position>
{
    public World World;

    public void Execute(Entity entity, ref Position c1)
    {
        World.DeferredRemoveComponent<Position>(entity);
    }
}

file ref struct DeferredRemoveVelocityForEachEntityPosition : IForEachEntity<Position>
{
    public World World;

    public void Execute(Entity entity, ref Position c1)
    {
        World.DeferredRemoveComponent<Velocity>(entity);
    }
}
