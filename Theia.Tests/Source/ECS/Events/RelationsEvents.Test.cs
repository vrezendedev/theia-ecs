using System;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Events;

[Collection("MetaRequester")]
public sealed class RelationsEventsTests
{
    [Fact]
    public void OnAnyRelationAdded_WorldEvent_FiresWhenRelationAdded()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        bool fired = false;

        world.RelationsEvents.OnAnyRelationAdded += (_) => fired = true;

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.True(fired);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_OwnerIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        Entity capturedOwner = default;

        world.RelationsEvents.OnAnyRelationAdded += (data) => capturedOwner = data.Owner;

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(owner, capturedOwner);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_TargetIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        Entity capturedTarget = default;

        world.RelationsEvents.OnAnyRelationAdded += (data) => capturedTarget = data.Target;

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(target, capturedTarget);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        Type? type = null;

        world.RelationsEvents.OnAnyRelationAdded += (data) => type = data.Type;

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(typeof(Friend), type);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_DoesNotFireWhenRelationAlreadyExists()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        bool fired = false;

        world.RelationsEvents.OnAnyRelationAdded += (_) => fired = true;

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_DoesNotFireWhenOwnerNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryGhoulify(owner);

        bool fired = false;

        world.RelationsEvents.OnAnyRelationAdded += (_) => fired = true;

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_DoesNotFireWhenTargetNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryGhoulify(target);

        bool fired = false;

        world.RelationsEvents.OnAnyRelationAdded += (_) => fired = true;

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_FiresForEvaluatedRelation()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        bool fired = false;

        world.RelationsEvents.OnAnyRelationAdded += (_) => fired = true;

        world.TryAddEvaluatedRelation<Damage>(owner, target);

        Assert.True(fired);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_FiresWhenRelationRemoved()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        bool fired = false;

        world.RelationsEvents.OnAnyRelationRemoved += (_) => fired = true;

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.True(fired);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_OwnerIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        Entity capturedOwner = default;

        world.RelationsEvents.OnAnyRelationRemoved += (data) => capturedOwner = data.Owner;

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(owner, capturedOwner);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_TargetIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        Entity capturedTarget = default;

        world.RelationsEvents.OnAnyRelationRemoved += (data) => capturedTarget = data.Target;

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(target, capturedTarget);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        Type? type = null;

        world.RelationsEvents.OnAnyRelationRemoved += (data) => type = data.Type;

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(typeof(Friend), type);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_DoesNotFireWhenRelationMissing()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        bool fired = false;

        world.RelationsEvents.OnAnyRelationRemoved += (_) => fired = true;

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_DoesNotFireWhenOwnerNotAlive()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        world.TryGhoulify(owner);

        bool fired = false;

        world.RelationsEvents.OnAnyRelationRemoved += (_) => fired = true;

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.False(fired);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_FiresForEachTargetWhenRemovedByOwner()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity targetA = assemblage.Create(new ComponentA { A = 2 });
        Entity targetB = assemblage.Create(new ComponentA { A = 3 });
        Entity targetC = assemblage.Create(new ComponentA { A = 4 });

        world.TryAddTagRelation<Friend>(owner, targetA);
        world.TryAddTagRelation<Friend>(owner, targetB);
        world.TryAddTagRelation<Friend>(owner, targetC);

        int firedCount = 0;

        world.RelationsEvents.OnAnyRelationRemoved += (_) => firedCount++;

        world.TryRemoveRelation<Friend>(owner);

        Assert.Equal(3, firedCount);
    }

    [Fact]
    public void SubscribeOnRelationAdded_WorldEvent_FiresForMatchingRelation()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        bool fired = false;

        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(_ => fired = true);

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.True(fired);
    }

    [Fact]
    public void SubscribeOnRelationAdded_WorldEvent_DoesNotFireForDifferentRelation()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        bool fired = false;

        world.RelationsEvents.SubscribeOnRelationAdded<Damage>(_ => fired = true);

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnRelationAdded_WorldEvent_OwnerIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        Entity capturedOwner = default;

        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(data => capturedOwner = data.Owner);

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(owner, capturedOwner);
    }

    [Fact]
    public void SubscribeOnRelationAdded_WorldEvent_TargetIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        Entity capturedTarget = default;

        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(data =>
            capturedTarget = data.Target
        );

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(target, capturedTarget);
    }

    [Fact]
    public void SubscribeOnRelationAdded_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        Type? type = null;

        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(data => type = data.Type);

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(typeof(Friend), type);
    }

    [Fact]
    public void SubscribeOnRelationAdded_BothBroadAndSpecificFire()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        bool broadFired = false;
        bool specificFired = false;

        world.RelationsEvents.OnAnyRelationAdded += _ => broadFired = true;
        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(_ => specificFired = true);

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.True(broadFired);
        Assert.True(specificFired);
    }

    [Fact]
    public void SubscribeOnRelationAdded_SpecificDoesNotFireWhenBroadWould()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        bool broadFired = false;
        bool specificFired = false;

        world.RelationsEvents.OnAnyRelationAdded += _ => broadFired = true;
        world.RelationsEvents.SubscribeOnRelationAdded<Damage>(_ => specificFired = true);

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.True(broadFired);
        Assert.False(specificFired);
    }

    [Fact]
    public void SubscribeOnRelationRemoved_WorldEvent_FiresForMatchingRelation()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        bool fired = false;

        world.RelationsEvents.SubscribeOnRelationRemoved<Friend>(_ => fired = true);

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.True(fired);
    }

    [Fact]
    public void SubscribeOnRelationRemoved_WorldEvent_DoesNotFireForDifferentRelation()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);
        world.TryAddEvaluatedRelation<Damage>(owner, target);

        bool fired = false;

        world.RelationsEvents.SubscribeOnRelationRemoved<Damage>(_ => fired = true);

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.False(fired);
    }

    [Fact]
    public void SubscribeOnRelationRemoved_WorldEvent_OwnerIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        Entity capturedOwner = default;

        world.RelationsEvents.SubscribeOnRelationRemoved<Friend>(data =>
            capturedOwner = data.Owner
        );

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(owner, capturedOwner);
    }

    [Fact]
    public void SubscribeOnRelationRemoved_WorldEvent_TargetIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        Entity capturedTarget = default;

        world.RelationsEvents.SubscribeOnRelationRemoved<Friend>(data =>
            capturedTarget = data.Target
        );

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(target, capturedTarget);
    }

    [Fact]
    public void SubscribeOnRelationRemoved_WorldEvent_TypeIsCorrect()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        Type? type = null;

        world.RelationsEvents.SubscribeOnRelationRemoved<Friend>(data => type = data.Type);

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(typeof(Friend), type);
    }

    [Fact]
    public void SubscribeOnRelationRemoved_BothBroadAndSpecificFire()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        bool broadFired = false;
        bool specificFired = false;

        world.RelationsEvents.OnAnyRelationRemoved += _ => broadFired = true;
        world.RelationsEvents.SubscribeOnRelationRemoved<Friend>(_ => specificFired = true);

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.True(broadFired);
        Assert.True(specificFired);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsPerRelationAddedSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        int firedCount = 0;

        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(_ => firedCount++);

        world.RelationsEvents.Reset();

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(0, firedCount);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsPerRelationRemovedSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        int firedCount = 0;

        world.RelationsEvents.SubscribeOnRelationRemoved<Friend>(_ => firedCount++);

        world.RelationsEvents.Reset();

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(0, firedCount);
    }

    [Fact]
    public void Reset_WorldEvents_PerRelationSubscribersCanBeReregisteredAfterReset()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(_ => { });

        world.RelationsEvents.Reset();

        bool fired = false;

        world.RelationsEvents.SubscribeOnRelationAdded<Friend>(_ => fired = true);

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.True(fired);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsBroadAddedSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        int firedCount = 0;

        world.RelationsEvents.OnAnyRelationAdded += _ => firedCount++;

        world.RelationsEvents.Reset();

        world.TryAddTagRelation<Friend>(owner, target);

        Assert.Equal(0, firedCount);
    }

    [Fact]
    public void Reset_WorldEvents_ClearsBroadRemovedSubscribers()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity owner = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 2 });

        world.TryAddTagRelation<Friend>(owner, target);

        int firedCount = 0;

        world.RelationsEvents.OnAnyRelationRemoved += _ => firedCount++;

        world.RelationsEvents.Reset();

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(0, firedCount);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_AsReturnsCorrectMatchesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 1 });

        MatchesRelationEnum result = default;

        world.RelationsEvents.OnAnyRelationAdded += (data) =>
            result = data.As<MatchesRelationEnum>();

        world.TryAddTagRelation<Friend>(entity, target);

        Assert.Equal(MatchesRelationEnum.Friend, result);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_AsReturnsCorrectIncludesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 1 });

        IncludesRelationOnlyEnum result = default;

        world.RelationsEvents.OnAnyRelationAdded += (data) =>
            result = data.As<IncludesRelationOnlyEnum>();

        world.TryAddTagRelation<Friend>(entity, target);

        Assert.Equal(IncludesRelationOnlyEnum.Group, result);
    }

    [Fact]
    public void OnAnyRelationAdded_WorldEvent_AsReturnsDefaultForUnmappedComponent()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 1 });

        MatchesRelationEnum result = default;

        world.RelationsEvents.OnAnyRelationAdded += (data) =>
            result = data.As<MatchesRelationEnum>();

        world.TryAddEvaluatedRelation<Damage>(entity, target);

        Assert.Equal(default, result);
    }

    [Fact]
    public void OnAnyRelationRemoved_WorldEvent_AsReturnsCorrectMatchesEnumValue()
    {
        World world = new();

        Assemblage<ComponentA> assemblage = world.CreateAssemblage<ComponentA>();

        Entity entity = assemblage.Create(new ComponentA { A = 1 });
        Entity target = assemblage.Create(new ComponentA { A = 1 });

        world.TryAddEvaluatedRelation<Damage>(entity, target);

        MatchesMultipleRelationEnum result = default;

        world.RelationsEvents.OnAnyRelationRemoved += (data) =>
            result = data.As<MatchesMultipleRelationEnum>();

        world.TryRemoveRelation<Damage>(entity);

        Assert.Equal(MatchesMultipleRelationEnum.Damage, result);
    }
}
