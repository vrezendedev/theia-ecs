using System;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Events;

public sealed class EntityEventsTests
{
    [Fact]
    public void OnEntityCreated_WorldEvent_FiresWhenEntityCreated()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool fired = false;

        world.Events.OnEntityCreated += (_) => fired = true;

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(fired);
    }

    [Fact]
    public void OnEntityCreated_AssemblageEvent_FiresWhenEntityCreated()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool fired = false;

        assemblage.Events.OnEntityCreated += (_) => fired = true;

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(fired);
    }

    [Fact]
    public void OnEntityCreated_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity createdEntity = default;

        world.Events.OnEntityCreated += (data) => createdEntity = data.Entity;

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Assert.Equal(entity, createdEntity);
    }

    [Fact]
    public void OnEntityCreated_WorldEvent_HasComponentReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool hasComponent = false;

        world.Events.OnEntityCreated += (data) => hasComponent = data.Has<ComponentA>();

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(hasComponent);
    }

    [Fact]
    public void OnEntityCreated_WorldEvent_IsAssemblageReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool isAssemblage = false;

        world.Events.OnEntityCreated += (data) => isAssemblage = data.Is(assemblage);

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(isAssemblage);
    }

    [Fact]
    public void OnEntityGhoulified_WorldEvent_FiresWhenEntityGhoulified()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.Events.OnEntityGhoulified += (_) => fired = true;

        world.TryGhoulify(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnEntityGhoulified_AssemblageEvent_FiresWhenEntityBelongedToAssemblage()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        assemblage.Events.OnEntityGhoulified += (_) => fired = true;

        world.TryGhoulify(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnEntityGhoulified_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Entity ghoulifiedEntity = default;

        world.Events.OnEntityGhoulified += (data) => ghoulifiedEntity = data.Entity;

        world.TryGhoulify(entity);

        Assert.Equal(entity, ghoulifiedEntity);
    }

    [Fact]
    public void OnEntityGhoulified_WorldEvent_HadComponentReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool hadComponent = false;

        world.Events.OnEntityGhoulified += (data) => hadComponent = data.Had<ComponentA>();

        world.TryGhoulify(entity);

        Assert.True(hadComponent);
    }

    [Fact]
    public void OnEntityGhoulified_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.Events.OnEntityGhoulified += (_) => fired = true;

        world.TryGhoulify(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnEntityGhoulified_WorldEvent_WasAssemblageReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool wasAssemblage = false;

        world.Events.OnEntityGhoulified += (data) => wasAssemblage = data.Was(assemblage);

        world.TryGhoulify(entity);

        Assert.True(wasAssemblage);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_FiresWhenComponentAdded()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.Events.OnComponentAdded += (_) => fired = true;

        world.TryAdd<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnComponentAdded_AssemblageEvent_FiresWhenEntityBelongedToAssemblage()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        assemblage.Events.OnComponentAdded += (_) => fired = true;

        world.TryAdd<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Entity modifiedEntity = default;

        world.Events.OnComponentAdded += (data) => modifiedEntity = data.Entity;

        world.TryAdd<ComponentB>(entity);

        Assert.Equal(entity, modifiedEntity);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Type? type = null;

        world.Events.OnComponentAdded += (data) => type = data.Type;

        world.TryAdd<ComponentB>(entity);

        Assert.Equal(typeof(ComponentB), type);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.Events.OnComponentAdded += (_) => fired = true;

        world.TryAdd<ComponentB>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_DoesNotFireWhenEntityAlreadyHasComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.Events.OnComponentAdded += (_) => fired = true;

        world.TryAdd<ComponentA>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_HasReturnsTrueForNewComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool has = false;

        world.Events.OnComponentAdded += (data) => has = data.Has<ComponentB>();

        world.TryAdd<ComponentB>(entity);

        Assert.True(has);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_HadReturnsTrueForPreviousComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool had = false;

        world.Events.OnComponentAdded += (data) => had = data.Had<ComponentA>();

        world.TryAdd<ComponentB>(entity);

        Assert.True(had);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_HadReturnsFalseForNewComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool had = false;

        world.Events.OnComponentAdded += (data) => had = data.Had<ComponentB>();

        world.TryAdd<ComponentB>(entity);

        Assert.False(had);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_IsAssemblageReturnsFalseAfterTransition()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool is_ = false;

        world.Events.OnComponentAdded += (data) => is_ = data.Is(assemblage);

        world.TryAdd<ComponentB>(entity);

        Assert.False(is_);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_WasAssemblageReturnsTrueBeforeTransition()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool was = false;

        world.Events.OnComponentAdded += (data) => was = data.Was(assemblage);

        world.TryAdd<ComponentB>(entity);

        Assert.True(was);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_FiresWhenComponentRemoved()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<ComponentB>(entity);

        bool fired = false;

        world.Events.OnComponentRemoved += (_) => fired = true;

        world.TryRemove<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<ComponentB>(entity);

        Entity modifiedEntity = default;

        world.Events.OnComponentRemoved += (data) => modifiedEntity = data.Entity;

        world.TryRemove<ComponentB>(entity);

        Assert.Equal(entity, modifiedEntity);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<ComponentB>(entity);

        Type? type = null;

        world.Events.OnComponentRemoved += (data) => type = data.Type;

        world.TryRemove<ComponentB>(entity);

        Assert.Equal(typeof(ComponentB), type);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.Events.OnComponentRemoved += (_) => fired = true;

        world.TryRemove<ComponentA>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_DoesNotFireWhenEntityMissingComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.Events.OnComponentRemoved += (_) => fired = true;

        world.TryRemove<ComponentB>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_HadReturnsTrueForRemovedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<ComponentB>(entity);

        bool had = false;

        world.Events.OnComponentRemoved += (data) => had = data.Had<ComponentB>();

        world.TryRemove<ComponentB>(entity);

        Assert.True(had);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_HasReturnsFalseForRemovedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<ComponentB>(entity);

        bool has = false;

        world.Events.OnComponentRemoved += (data) => has = data.Has<ComponentB>();

        world.TryRemove<ComponentB>(entity);

        Assert.False(has);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_HasReturnsTrueForRemainingComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<ComponentB>(entity);

        bool has = false;

        world.Events.OnComponentRemoved += (data) => has = data.Has<ComponentA>();

        world.TryRemove<ComponentB>(entity);

        Assert.True(has);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_AsReturnsCorrectMatchesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        MatchesEnum result = default;

        world.Events.OnComponentAdded += (data) => result = data.As<MatchesEnum>();

        world.TryAdd<MatchesComponentA>(entity);

        Assert.Equal(MatchesEnum.ComponentA, result);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_AsReturnsCorrectIncludesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        IncludesEnum result = default;

        world.Events.OnComponentAdded += (data) => result = data.As<IncludesEnum>();

        world.TryAdd<IncludesComponentA>(entity);

        Assert.Equal(IncludesEnum.Group, result);
    }

    [Fact]
    public void OnComponentAdded_WorldEvent_AsReturnsDefaultForUnmappedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        MatchesEnum result = default;

        world.Events.OnComponentAdded += (data) => result = data.As<MatchesEnum>();

        world.TryAdd<UnmappedComponent>(entity);

        Assert.Equal(default, result);
    }

    [Fact]
    public void OnComponentRemoved_WorldEvent_AsReturnsCorrectMatchesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<MatchesComponentA>(entity);

        MatchesEnum result = default;

        world.Events.OnComponentRemoved += (data) => result = data.As<MatchesEnum>();

        world.TryRemove<MatchesComponentA>(entity);

        Assert.Equal(MatchesEnum.ComponentA, result);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsAllSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        int firedCount = 0;

        world.Events.OnEntityCreated += (_) => firedCount++;
        world.Events.OnEntityGhoulified += (_) => firedCount++;
        world.Events.OnComponentAdded += (_) => firedCount++;
        world.Events.OnComponentRemoved += (_) => firedCount++;

        world.Events.Reset();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAdd<ComponentB>(entity);

        world.TryRemove<ComponentB>(entity);

        world.TryGhoulify(entity);

        Assert.Equal(0, firedCount);
    }
}
