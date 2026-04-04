using System;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Events;

public sealed class EntityEventsTests
{
    [Fact]
    public void OnCreated_WorldEvent_FiresWhenEntityCreated()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool fired = false;

        world.EntityEvents.OnCreated += (_) => fired = true;

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(fired);
    }

    [Fact]
    public void OnCreated_AssemblageEvent_FiresWhenEntityCreated()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool fired = false;

        assemblage.EntityEvents.OnCreated += (_) => fired = true;

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(fired);
    }

    [Fact]
    public void OnCreated_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity createdEntity = default;

        world.EntityEvents.OnCreated += (data) => createdEntity = data.Entity;

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Assert.Equal(entity, createdEntity);
    }

    [Fact]
    public void OnCreated_WorldEvent_HasComponentReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool hasComponent = false;

        world.EntityEvents.OnCreated += (data) => hasComponent = data.Has<ComponentA>();

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(hasComponent);
    }

    [Fact]
    public void OnCreated_WorldEvent_IsAssemblageReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        bool isAssemblage = false;

        world.EntityEvents.OnCreated += (data) => isAssemblage = data.Is(assemblage);

        assemblage.Create(new ComponentA { A = 1 });

        Assert.True(isAssemblage);
    }

    [Fact]
    public void OnGhoulified_WorldEvent_FiresWhenEntityGhoulified()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.OnGhoulified += (_) => fired = true;

        world.TryGhoulify(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnGhoulified_AssemblageEvent_FiresWhenEntityBelongedToAssemblage()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        assemblage.EntityEvents.OnGhoulified += (_) => fired = true;

        world.TryGhoulify(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnGhoulified_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Entity ghoulifiedEntity = default;

        world.EntityEvents.OnGhoulified += (data) => ghoulifiedEntity = data.Entity;

        world.TryGhoulify(entity);

        Assert.Equal(entity, ghoulifiedEntity);
    }

    [Fact]
    public void OnGhoulified_WorldEvent_HadComponentReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool hadComponent = false;

        world.EntityEvents.OnGhoulified += (data) => hadComponent = data.Had<ComponentA>();

        world.TryGhoulify(entity);

        Assert.True(hadComponent);
    }

    [Fact]
    public void OnGhoulified_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.EntityEvents.OnGhoulified += (_) => fired = true;

        world.TryGhoulify(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnGhoulified_WorldEvent_WasAssemblageReturnsTrue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool wasAssemblage = false;

        world.EntityEvents.OnGhoulified += (data) => wasAssemblage = data.Was(assemblage);

        world.TryGhoulify(entity);

        Assert.True(wasAssemblage);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_FiresWhenComponentAdded()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.OnAnyComponentAdded += (_) => fired = true;

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnAnyComponentAdded_AssemblageEvent_FiresWhenEntityBelongedToAssemblage()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        assemblage.EntityEvents.OnAnyComponentAdded += (_) => fired = true;

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Entity modifiedEntity = default;

        world.EntityEvents.OnAnyComponentAdded += (data) => modifiedEntity = data.Entity;

        world.TryAddComponent<ComponentB>(entity);

        Assert.Equal(entity, modifiedEntity);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Type? type = null;

        world.EntityEvents.OnAnyComponentAdded += (data) => type = data.Type;

        world.TryAddComponent<ComponentB>(entity);

        Assert.Equal(typeof(ComponentB), type);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.EntityEvents.OnAnyComponentAdded += (_) => fired = true;

        world.TryAddComponent<ComponentB>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_DoesNotFireWhenEntityAlreadyHasComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.OnAnyComponentAdded += (_) => fired = true;

        world.TryAddComponent<ComponentA>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_HasReturnsTrueForNewComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool has = false;

        world.EntityEvents.OnAnyComponentAdded += (data) => has = data.Has<ComponentB>();

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(has);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_HadReturnsTrueForPreviousComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool had = false;

        world.EntityEvents.OnAnyComponentAdded += (data) => had = data.Had<ComponentA>();

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(had);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_HadReturnsFalseForNewComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool had = false;

        world.EntityEvents.OnAnyComponentAdded += (data) => had = data.Had<ComponentB>();

        world.TryAddComponent<ComponentB>(entity);

        Assert.False(had);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_IsAssemblageReturnsFalseAfterTransition()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool is_ = false;

        world.EntityEvents.OnAnyComponentAdded += (data) => is_ = data.Is(assemblage);

        world.TryAddComponent<ComponentB>(entity);

        Assert.False(is_);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_WasAssemblageReturnsTrueBeforeTransition()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool was = false;

        world.EntityEvents.OnAnyComponentAdded += (data) => was = data.Was(assemblage);

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(was);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_FiresWhenComponentRemoved()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool fired = false;

        world.EntityEvents.OnAnyComponentRemoved += (_) => fired = true;

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        Entity modifiedEntity = default;

        world.EntityEvents.OnAnyComponentRemoved += (data) => modifiedEntity = data.Entity;

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.Equal(entity, modifiedEntity);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        Type? type = null;

        world.EntityEvents.OnAnyComponentRemoved += (data) => type = data.Type;

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.Equal(typeof(ComponentB), type);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.EntityEvents.OnAnyComponentRemoved += (_) => fired = true;

        world.TryRemoveComponent<ComponentA>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_DoesNotFireWhenEntityMissingComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.OnAnyComponentRemoved += (_) => fired = true;

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_HadReturnsTrueForRemovedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool had = false;

        world.EntityEvents.OnAnyComponentRemoved += (data) => had = data.Had<ComponentB>();

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.True(had);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_HasReturnsFalseForRemovedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool has = false;

        world.EntityEvents.OnAnyComponentRemoved += (data) => has = data.Has<ComponentB>();

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.False(has);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_HasReturnsTrueForRemainingComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool has = false;

        world.EntityEvents.OnAnyComponentRemoved += (data) => has = data.Has<ComponentA>();

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.True(has);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_AsReturnsCorrectMatchesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        MatchesEnum result = default;

        world.EntityEvents.OnAnyComponentAdded += (data) => result = data.As<MatchesEnum>();

        world.TryAddComponent<MatchesComponentA>(entity);

        Assert.Equal(MatchesEnum.ComponentA, result);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_AsReturnsCorrectIncludesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        IncludesEnum result = default;

        world.EntityEvents.OnAnyComponentAdded += (data) => result = data.As<IncludesEnum>();

        world.TryAddComponent<IncludesComponentA>(entity);

        Assert.Equal(IncludesEnum.Group, result);
    }

    [Fact]
    public void OnAnyComponentAdded_WorldEvent_AsReturnsDefaultForUnmappedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        MatchesEnum result = default;

        world.EntityEvents.OnAnyComponentAdded += (data) => result = data.As<MatchesEnum>();

        world.TryAddComponent<UnmappedComponent>(entity);

        Assert.Equal(default, result);
    }

    [Fact]
    public void OnAnyComponentRemoved_WorldEvent_AsReturnsCorrectMatchesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<MatchesComponentA>(entity);

        MatchesEnum result = default;

        world.EntityEvents.OnAnyComponentRemoved += (data) => result = data.As<MatchesEnum>();

        world.TryRemoveComponent<MatchesComponentA>(entity);

        Assert.Equal(MatchesEnum.ComponentA, result);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsAllSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        int firedCount = 0;

        world.EntityEvents.OnCreated += (_) => firedCount++;
        world.EntityEvents.OnGhoulified += (_) => firedCount++;
        world.EntityEvents.OnAnyComponentAdded += (_) => firedCount++;
        world.EntityEvents.OnAnyComponentRemoved += (_) => firedCount++;

        world.EntityEvents.Reset();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        world.TryRemoveComponent<ComponentB>(entity);

        world.TryGhoulify(entity);

        Assert.Equal(0, firedCount);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_FiresWhenSpecificComponentAdded()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => fired = true);

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_DoesNotFireForDifferentComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => fired = true);

        world.TryAddComponent<ComponentC>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Entity modifiedEntity = default;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(data =>
            modifiedEntity = data.Entity
        );

        world.TryAddComponent<ComponentB>(entity);

        Assert.Equal(entity, modifiedEntity);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        Type? type = null;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(data => type = data.Type);

        world.TryAddComponent<ComponentB>(entity);

        Assert.Equal(typeof(ComponentB), type);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_HasReturnsTrueForSubscribedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool has = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(data =>
            has = data.Has<ComponentB>()
        );

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(has);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_HadReturnsFalseForSubscribedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool had = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(data =>
            had = data.Had<ComponentB>()
        );

        world.TryAddComponent<ComponentB>(entity);

        Assert.False(had);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_HadReturnsTrueForPreviousComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool had = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(data =>
            had = data.Had<ComponentA>()
        );

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(had);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => fired = true);

        world.TryAddComponent<ComponentB>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_DoesNotFireWhenEntityAlreadyHasComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentA>(_ => fired = true);

        world.TryAddComponent<ComponentA>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_WasAssemblageReturnsTrueBeforeTransition()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool was = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(data =>
            was = data.Was(assemblage)
        );

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(was);
    }

    [Fact]
    public void SubscribeOnComponentAdded_WorldEvent_IsAssemblageReturnsFalseAfterTransition()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool belongs = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(data =>
            belongs = data.Is(assemblage)
        );

        world.TryAddComponent<ComponentB>(entity);

        Assert.False(belongs);
    }

    [Fact]
    public void SubscribeOnComponentAdded_AssemblageEvent_FiresWhenEntityBelongedToAssemblage()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        assemblage.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => fired = true);

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void SubscribeOnComponentAdded_AssemblageEvent_DoesNotFireForUnrelatedAssemblage()
    {
        World world = new();

        Assemblage<ComponentA> assemblageA = world.CreateAssemblage<ComponentA>();
        Assemblage<ComponentC> assemblageC = world.CreateAssemblage<ComponentC>();

        Entity entityA = assemblageA.Create(new ComponentA { A = 1 });

        bool fired = false;

        assemblageC.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => fired = true);

        world.TryAddComponent<ComponentB>(entityA);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_FiresWhenSpecificComponentRemoved()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(_ => fired = true);

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.True(fired);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_DoesNotFireForDifferentComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);
        world.TryAddComponent<ComponentC>(entity);

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(_ => fired = true);

        world.TryRemoveComponent<ComponentC>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_EntityDataIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        Entity modifiedEntity = default;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(data =>
            modifiedEntity = data.Entity
        );

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.Equal(entity, modifiedEntity);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        Type? type = null;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(data => type = data.Type);

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.Equal(typeof(ComponentB), type);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_HadReturnsTrueForRemovedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool had = false;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(data =>
            had = data.Had<ComponentB>()
        );

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.True(had);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_HasReturnsFalseForRemovedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool has = false;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(data =>
            has = data.Has<ComponentB>()
        );

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.False(has);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_HasReturnsTrueForRemainingComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool has = false;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(data =>
            has = data.Has<ComponentA>()
        );

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.True(has);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_DoesNotFireWhenEntityNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryGhoulify(entity);

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentA>(_ => fired = true);

        world.TryRemoveComponent<ComponentA>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_WorldEvent_DoesNotFireWhenEntityMissingComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(_ => fired = true);

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnComponentAdded_BothSpecificAndBroadEventsFire()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool broadFired = false;
        bool specificFired = false;

        world.EntityEvents.OnAnyComponentAdded += _ => broadFired = true;
        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => specificFired = true);

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(broadFired);
        Assert.True(specificFired);
    }

    [Fact]
    public void SubscribeOnComponentRemoved_BothSpecificAndBroadEventsFire()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        bool broadFired = false;
        bool specificFired = false;

        world.EntityEvents.OnAnyComponentRemoved += _ => broadFired = true;
        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(_ => specificFired = true);

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.True(broadFired);
        Assert.True(specificFired);
    }

    [Fact]
    public void SubscribeOnComponentAdded_SpecificEventDoesNotFireWhenBroadWould()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        bool broadFired = false;
        bool specificFired = false;

        world.EntityEvents.OnAnyComponentAdded += _ => broadFired = true;
        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => specificFired = true);

        world.TryAddComponent<ComponentC>(entity);

        Assert.True(broadFired);
        Assert.False(specificFired);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsPerComponentAddedSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        int firedCount = 0;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => firedCount++);

        world.EntityEvents.Reset();

        world.TryAddComponent<ComponentB>(entity);

        Assert.Equal(0, firedCount);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsPerComponentRemovedSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddComponent<ComponentB>(entity);

        int firedCount = 0;

        world.EntityEvents.SubscribeOnComponentRemoved<ComponentB>(_ => firedCount++);

        world.EntityEvents.Reset();

        world.TryRemoveComponent<ComponentB>(entity);

        Assert.Equal(0, firedCount);
    }

    [Fact]
    public void Reset_WorldEvents_PerComponentSubscribersCanBeReregisteredAfterReset()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => { });

        world.EntityEvents.Reset();

        bool fired = false;

        world.EntityEvents.SubscribeOnComponentAdded<ComponentB>(_ => fired = true);

        world.TryAddComponent<ComponentB>(entity);

        Assert.True(fired);
    }
}
