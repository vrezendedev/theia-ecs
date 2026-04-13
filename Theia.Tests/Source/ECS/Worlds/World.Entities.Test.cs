using System;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldEntityLifecycleTests
{
    [Fact]
    public void IsAlive_DefaultEntity_ReturnsFalse() => Assert.False(new World().IsAlive(default));

    [Fact]
    public void IsAlive_AfterCreate_ReturnsTrue()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void IsAlive_AfterGhoulify_ReturnsFalse()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void IsAlive_WithStaleVersion_ReturnsFalse()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        Entity stale = new Entity() { _id = entity._id, _version = entity._version };

        Assert.False(world.IsAlive(stale));
    }

    [Fact]
    public void CountEntities_AfterCreate_IncreasesByOne()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int before = world.CountEntitiesAlive();

        assemblage.Create(new Position());

        Assert.Equal(before + 1, world.CountEntitiesAlive());
    }

    [Fact]
    public void CountEntities_AfterGhoulify_DecreasesByOne()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        int before = world.CountEntitiesAlive();

        world.TryGhoulify(entity);

        Assert.Equal(before - 1, world.CountEntitiesAlive());
    }

    [Fact]
    public void CountGhouls_AfterGhoulify_IncreasesByOne()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        int before = world.CountGhouls();

        world.TryGhoulify(entity);

        Assert.Equal(before + 1, world.CountGhouls());
    }

    [Fact]
    public void CountGhouls_AfterGhoulifyThenRecycle_DecreasesOnRecycle()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        world.TryGhoulify(entity);

        int ghoulsBefore = world.CountGhouls();

        assemblage.Create(new Position());

        Assert.Equal(ghoulsBefore - 1, world.CountGhouls());
    }

    [Fact]
    public void TryGhoulify_AliveEntity_ReturnsTrue()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.True(world.TryGhoulify(entity));
    }

    [Fact]
    public void TryGhoulify_WithGhoul_ReturnsFalse()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        Assert.False(world.TryGhoulify(entity));
    }

    [Fact]
    public void TryGhoulify_DefaultEntity_ReturnsFalse()
    {
        World world = new();

        Assert.False(world.TryGhoulify(default));
    }

    [Fact]
    public void TryGhoulify_IsRecycledOnNextCreate()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity original = assemblage.Create(new Position());

        world.TryGhoulify(original);

        Entity recycled = assemblage.Create(new Position());

        Assert.Equal(original._id, recycled._id);
    }

    [Fact]
    public void TryGhoulify_RecycledEntity_HasIncrementedVersion()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity original = assemblage.Create(new Position());

        world.TryGhoulify(original);

        Entity recycled = assemblage.Create(new Position());

        Assert.True(recycled._version > original._version);
    }

    [Fact]
    public void TryGhoulify_MultipleEntities_OnlyTargetBecomesInvalid()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entityA = assemblage.Create(new Position());
        Entity entityB = assemblage.Create(new Position());

        world.TryGhoulify(entityA);

        Assert.False(world.IsAlive(entityA));
        Assert.True(world.IsAlive(entityB));
    }

    [Fact]
    public void TryAdd_ToAliveEntityWithoutComponent_ReturnsTrue()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.True(world.TryAddComponent<Velocity>(entity));
    }

    [Fact]
    public void TryAdd_WithGhoul_ReturnsFalse()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        Assert.False(world.TryAddComponent<Velocity>(entity));
    }

    [Fact]
    public void TryAdd_ComponentAlreadyPresent_ReturnsFalse()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.False(world.TryAddComponent<Position>(entity));
    }

    [Fact]
    public void TryAdd_ComponentIsStoredCorrectly()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryAddComponent(entity, new Velocity { X = 3, Y = 7 });

        ref Velocity velocity = ref world.Get<Velocity>(entity);

        Assert.Equal(3, velocity.X);
        Assert.Equal(7, velocity.Y);
    }

    [Fact]
    public void TryAdd_EntityRemainsAlive()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryAddComponent<Velocity>(entity);

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void TryAdd_PreservesExistingComponentData()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position { X = 42, Y = 13 });

        world.TryAddComponent<Velocity>(entity);

        ref Position position = ref world.Get<Position>(entity);

        Assert.Equal(42, position.X);
        Assert.Equal(13, position.Y);
    }

    [Fact]
    public void TryAdd_CalledTwiceWithDifferentComponents_EntityHasAll()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryAddComponent<Velocity>(entity);
        world.TryAddComponent<Rotation>(entity);

        Assert.True(world.TryAddComponent<Health>(entity));

        Assert.True(world.Has<Position>(entity));
        Assert.True(world.Has<Velocity>(entity));
        Assert.True(world.Has<Rotation>(entity));
        Assert.True(world.Has<Health>(entity));
    }

    [Fact]
    public void TryRemove_ComponentPresent_ReturnsTrue()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryAddComponent<Velocity>(entity);

        Assert.True(world.TryRemoveComponent<Velocity>(entity));
    }

    [Fact]
    public void TryRemove_ComponentNotPresent_ReturnsFalse()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.False(world.TryRemoveComponent<Velocity>(entity));
    }

    [Fact]
    public void TryRemove_WithGhoul_ReturnsFalse()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());
        world.TryGhoulify(entity);

        Assert.False(world.TryRemoveComponent<Position>(entity));
    }

    [Fact]
    public void TryRemove_AfterRemove_ComponentIsNoLongerAccessible()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryAddComponent<Velocity>(entity);
        world.TryRemoveComponent<Velocity>(entity);

        Assert.Throws<InvalidOperationException>(() => world.Get<Velocity>(entity));
    }

    [Fact]
    public void TryRemove_LastComponent_GhoulifiesEntity()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryRemoveComponent<Position>(entity);

        Assert.False(world.IsAlive(entity));
    }

    [Fact]
    public void TryRemove_LastComponent_ReturnsTrue()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.True(world.TryRemoveComponent<Position>(entity));
    }

    [Fact]
    public void TryRemove_PreservesRemainingComponentData()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position { X = 5, Y = 9 });

        world.TryAddComponent<Velocity>(entity);
        world.TryRemoveComponent<Velocity>(entity);

        ref Position position = ref world.Get<Position>(entity);

        Assert.Equal(5, position.X);
        Assert.Equal(9, position.Y);
    }

    [Fact]
    public void Get_WithValidEntityAndComponent_ReturnsStoredValue()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position { X = 3, Y = 7 });

        ref Position position = ref world.Get<Position>(entity);

        Assert.Equal(3, position.X);
        Assert.Equal(7, position.Y);
    }

    [Fact]
    public void Get_ReturnsRefToLiveData_MutationIsPersisted()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position { X = 1, Y = 1 });

        world.Get<Position>(entity).X = 99;

        Assert.Equal(99, world.Get<Position>(entity).X);
    }

    [Fact]
    public void Get_WithGhoul_Throws()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        Assert.Throws<InvalidOperationException>(() => world.Get<Position>(entity));
    }

    [Fact]
    public void Get_WithDefaultEntity_Throws()
    {
        World world = new();

        Assert.Throws<InvalidOperationException>(() => world.Get<Position>(default));
    }

    [Fact]
    public void Get_WithMissingComponent_Throws()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.Throws<InvalidOperationException>(() => world.Get<Velocity>(entity));
    }

    [Fact]
    public void Set_WithValidEntityAndComponent_StoresValue()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.Set(entity, new Position { X = 5, Y = 8 });

        Assert.Equal(5, world.Get<Position>(entity).X);
        Assert.Equal(8, world.Get<Position>(entity).Y);
    }

    [Fact]
    public void Set_CalledMultipleTimes_UpdatesAppropriately()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.Set(entity, new Position { X = 1 });
        world.Set(entity, new Position { X = 2 });
        world.Set(entity, new Position { X = 3 });

        Assert.Equal(3, world.Get<Position>(entity).X);
    }

    [Fact]
    public void Set_WithGhoul_Throws()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        world.TryGhoulify(entity);

        Assert.Throws<InvalidOperationException>(() => world.Set(entity, new Position()));
    }

    [Fact]
    public void Set_WithMissingComponent_Throws()
    {
        World world = new();

        Entity entity = world.CreateAssemblage<Position>().Create(new Position());

        Assert.Throws<InvalidOperationException>(() => world.Set(entity, new Velocity()));
    }
}
