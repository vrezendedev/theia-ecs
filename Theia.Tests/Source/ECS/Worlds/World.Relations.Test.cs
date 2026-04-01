using System;
using System.Collections.Generic;
using Theia.ECS.Assemblages;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Relations;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldRelationTests
{
    [Fact]
    public void TryAddRelation_WithAliveEntities_ReturnsTrue()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.True(world.TryAddRelation<Friend>(owner, target));
    }

    [Fact]
    public void TryAddRelation_WithDeadOwner_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryGhoulify(owner);

        Assert.False(world.TryAddRelation<Friend>(owner, target));
    }

    [Fact]
    public void TryAddRelation_WithDeadTarget_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryGhoulify(target);

        Assert.False(world.TryAddRelation<Friend>(owner, target));
    }

    [Fact]
    public void TryAddRelation_WithDefaultOwner_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity target = assemblage.Create(new Position());

        Assert.False(world.TryAddRelation<Friend>(default, target));
    }

    [Fact]
    public void TryAddRelation_WithDefaultTarget_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Assert.False(world.TryAddRelation<Friend>(owner, default));
    }

    [Fact]
    public void TryAddRelation_IncreasesCountRelations()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        int before = world.CountRelations<Friend>(owner);

        world.TryAddRelation<Friend>(owner, target);

        Assert.Equal(before + 1, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void TryAddRelation_TargetGainsExternalLink()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        Assert.True(world.HasExternalLinks<Friend>(target));
    }

    [Fact]
    public void TryAddRelation_MultipleTargets_AllCountedCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());
        Entity targetC = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);
        world.TryAddRelation<Friend>(owner, targetC);

        Assert.Equal(3, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void TryAddRelation_MultipleOwners_TargetCountsAllLinks()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity ownerA = assemblage.Create(new Position());
        Entity ownerB = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(ownerA, target);
        world.TryAddRelation<Friend>(ownerB, target);

        Assert.Equal(2, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void TryAddRelation_WithEvaluatedRelationType_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() => world.TryAddRelation<Damage>(owner, target));
    }

    [Fact]
    public void TryAddEvaluatedRelation_WithAliveEntities_ReturnsTrue()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.True(world.TryAddEvaluatedRelation<Damage>(owner, target));
    }

    [Fact]
    public void TryAddEvaluatedRelation_WithDeadOwner_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryGhoulify(owner);

        Assert.False(world.TryAddEvaluatedRelation<Damage>(owner, target));
    }

    [Fact]
    public void TryAddEvaluatedRelation_WithDeadTarget_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryGhoulify(target);

        Assert.False(world.TryAddEvaluatedRelation<Damage>(owner, target));
    }

    [Fact]
    public void TryAddEvaluatedRelation_WithTagRelationType_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            world.TryAddEvaluatedRelation<Friend>(owner, target)
        );
    }

    [Fact]
    public void TryAddEvaluatedRelation_StoresSuppliedValue()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, target, new Damage { Value = 42f });

        Assert.Equal(42f, world.GetEvaluatedRelation<Damage>(owner, target).Value);
    }

    [Fact]
    public void TryAddEvaluatedRelation_WithoutValue_StoresDefault()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation<Damage>(owner, target);

        Assert.Equal(
            default(Damage).Value,
            world.GetEvaluatedRelation<Damage>(owner, target).Value
        );
    }

    [Fact]
    public void TryAddRelation_WithRelatedEntity_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation<Damage>(owner, target);

        Assert.False(world.TryAddEvaluatedRelation<Damage>(owner, target));
    }

    [Fact]
    public void HasRelation_AfterAdd_ReturnsTrue()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        Assert.True(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void HasRelation_WithNoRelation_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Assert.False(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void HasRelation_WithDeadEntity_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        world.TryGhoulify(owner);

        Assert.False(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void IsRelatedTo_AfterAdd_ReturnsTrue()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        Assert.True(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public void IsRelatedTo_WithNoRelation_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.False(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public void IsRelatedTo_UnrelatedPair_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);

        Assert.False(world.IsRelatedTo<Friend>(owner, targetB));
    }

    [Fact]
    public void IsRelatedTo_WithDeadOwner_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        world.TryGhoulify(owner);

        Assert.False(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public void IsRelatedTo_WithDeadTarget_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        Assert.True(world.IsRelatedTo<Friend>(owner, target));

        world.TryGhoulify(target);

        Assert.False(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public void CountRelations_WithNoRelation_ReturnsZero()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Assert.Equal(0, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void CountRelations_WithDeadEntity_ReturnsMinusOne()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        world.TryGhoulify(owner);

        Assert.Equal(-1, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void CountExternalLinks_WithNoLinks_ReturnsZero()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        Assert.Equal(0, world.CountExternalLinks<Friend>(entity));
    }

    [Fact]
    public void CountExternalLinks_WithDeadEntity_ReturnsMinusOne()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        world.TryGhoulify(entity);

        Assert.Equal(-1, world.CountExternalLinks<Friend>(entity));
    }

    [Fact]
    public void TryRemoveRelation_ByOwner_WhenRelationExists_ReturnsTrue()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        Assert.True(world.TryRemoveRelation<Friend>(owner));
    }

    [Fact]
    public void TryRemoveRelation_ByOwner_WhenNoRelation_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Assert.False(world.TryRemoveRelation<Friend>(owner));
    }

    [Fact]
    public void TryRemoveRelation_ByOwner_WithDeadOwner_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        world.TryGhoulify(owner);

        Assert.False(world.TryRemoveRelation<Friend>(owner));
    }

    [Fact]
    public void TryRemoveRelation_ByOwner_CountRelationsBecomesZero()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        world.TryRemoveRelation<Friend>(owner);

        Assert.Equal(0, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void TryRemoveRelation_ByOwner_AllTargetsLoseExternalLink()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        world.TryRemoveRelation<Friend>(owner);

        Assert.Equal(0, world.CountExternalLinks<Friend>(targetA));
        Assert.Equal(0, world.CountExternalLinks<Friend>(targetB));
    }

    [Fact]
    public void TryRemoveRelation_ByOwner_HasRelationReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        world.TryRemoveRelation<Friend>(owner);

        Assert.False(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_WhenRelationExists_ReturnsTrue()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        Assert.True(world.TryRemoveRelation<Friend>(owner, target));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_WhenNoRelation_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.False(world.TryRemoveRelation<Friend>(owner, target));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_WithDeadOwner_ReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);
        world.TryGhoulify(owner);

        Assert.False(world.TryRemoveRelation<Friend>(owner, target));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_DecrementsCountRelations()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        world.TryRemoveRelation<Friend>(owner, targetA);

        Assert.Equal(1, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_IsRelatedToReturnsFalse()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);
        world.TryRemoveRelation<Friend>(owner, target);

        Assert.False(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_OtherTargetRemainsRelated()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        world.TryRemoveRelation<Friend>(owner, targetA);

        Assert.True(world.IsRelatedTo<Friend>(owner, targetB));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_RemovedTargetLosesExternalLink()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        world.TryRemoveRelation<Friend>(owner, target);

        Assert.Equal(0, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void TryRemoveRelation_ByOwnerAndTarget_SurvivorCompositeKeyIsCorrectAfterSwap()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        world.TryRemoveRelation<Friend>(owner, targetA);

        Assert.True(world.TryRemoveRelation<Friend>(owner, targetB));

        Assert.Equal(0, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void GetRelations_AfterAddingTargets_LengthMatchesCount()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        ReadOnlySpan<Entity> relations = world.GetRelations<Friend>(owner);

        Assert.Equal(2, relations.Length);
    }

    [Fact]
    public void GetRelations_WithDeadOwner_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);
        world.TryGhoulify(owner);

        Assert.Throws<InvalidOperationException>(() => world.GetRelations<Friend>(owner));
    }

    [Fact]
    public void GetRelations_WithNoRelation_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() => world.GetRelations<Friend>(owner));
    }

    [Fact]
    public void GetEvaluatedRelation_ReturnsRefToLiveData_MutationPersists()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, target, new Damage { Value = 1f });

        world.GetEvaluatedRelation<Damage>(owner, target).Value = 99f;

        Assert.Equal(99f, world.GetEvaluatedRelation<Damage>(owner, target).Value);
    }

    [Fact]
    public void GetEvaluatedRelation_WithDeadOwner_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, target, new Damage { Value = 5f });
        world.TryGhoulify(owner);

        Assert.Throws<InvalidOperationException>(() =>
            world.GetEvaluatedRelation<Damage>(owner, target)
        );
    }

    [Fact]
    public void GetEvaluatedRelation_WithNoRelation_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            world.GetEvaluatedRelation<Damage>(owner, target)
        );
    }

    [Fact]
    public void GetEvaluatedRelations_LengthMatchesAddedTargets()
    {
        World world = new();
        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, targetA, new Damage { Value = 10f });
        world.TryAddEvaluatedRelation(owner, targetB, new Damage { Value = 20f });

        EntityEvaluatedRelations<Damage> result = world.GetEvaluatedRelations<Damage>(owner);

        Assert.Equal(2, result.Entities.Length);
        Assert.Equal(2, result.Data.Length);
    }

    [Fact]
    public void GetEvaluatedRelations_DataAlignedWithEntities()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, targetA, new Damage { Value = 10f });
        world.TryAddEvaluatedRelation(owner, targetB, new Damage { Value = 20f });

        EntityEvaluatedRelations<Damage> result = world.GetEvaluatedRelations<Damage>(owner);

        for (int i = 0; i < result.Entities.Length; i++)
        {
            float expected = world.GetEvaluatedRelation<Damage>(owner, result.Entities[i]).Value;
            Assert.Equal(expected, result.Data[i].Value);
        }
    }

    [Fact]
    public void QueryRelation_CallsCallbackForEachTarget()
    {
        World world = new();
        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());
        Entity targetC = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);
        world.TryAddRelation<Friend>(owner, targetC);

        List<Entity> visited = new();

        world.QueryRelation<Friend>(owner, e => visited.Add(e));

        Assert.Equal(3, visited.Count);
    }

    [Fact]
    public void QueryRelation_WithZeroTargets_CallbackNeverCalled()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);
        world.TryRemoveRelation<Friend>(owner, target);

        int callCount = 0;
        world.QueryRelation<Friend>(owner, _ => callCount++);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void QueryRelation_WithDeadOwner_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);
        world.TryGhoulify(owner);

        Assert.Throws<InvalidOperationException>(() =>
            world.QueryRelation<Friend>(owner, _ => { })
        );
    }

    [Fact]
    public void QueryRelation_AddRelationInsideCallback_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);

        Assert.Throws<InvalidOperationException>(() =>
            world.QueryRelation<Friend>(owner, _ => world.TryAddRelation<Friend>(owner, targetB))
        );
    }

    [Fact]
    public void QueryEvaluatedRelation_CallsCallbackWithCorrectData()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, targetA, new Damage { Value = 10f });
        world.TryAddEvaluatedRelation(owner, targetB, new Damage { Value = 20f });

        float total = 0f;

        world.QueryEvaluatedRelation(owner, (Entity _, ref Damage d) => total += d.Value);

        Assert.Equal(30f, total);
    }

    [Fact]
    public void QueryEvaluatedRelation_MutationInsideCallbackPersists()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddEvaluatedRelation(owner, target, new Damage { Value = 5f });

        world.QueryEvaluatedRelation(owner, (Entity _, ref Damage d) => d.Value = 99f);

        Assert.Equal(99f, world.GetEvaluatedRelation<Damage>(owner, target).Value);
    }

    [Fact]
    public void QueryEvaluatedRelation_WithTagRelationType_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        Assert.Throws<InvalidOperationException>(() =>
            world.QueryEvaluatedRelation(owner, (Entity _, ref Friend _) => { })
        );
    }

    [Fact]
    public void GetExternalLinks_AfterAddingOwners_LengthMatchesCount()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity ownerA = assemblage.Create(new Position());
        Entity ownerB = assemblage.Create(new Position());

        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(ownerA, target);
        world.TryAddRelation<Friend>(ownerB, target);

        ReadOnlySpan<ExternalLink> links = world.GetExternalLinks<Friend>(target);

        Assert.Equal(2, links.Length);
    }

    [Fact]
    public void GetExternalLinks_WithDeadEntity_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity target = assemblage.Create(new Position());

        world.TryGhoulify(target);

        Assert.Throws<InvalidOperationException>(() => world.GetExternalLinks<Friend>(target));
    }

    [Fact]
    public void GetExternalLinks_WithNoLinks_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity entity = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() => world.GetExternalLinks<Friend>(entity));
    }

    [Fact]
    public void GhoulifyOwner_AllTargetsLoseExternalLink()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        world.TryGhoulify(owner);

        Assert.Equal(0, world.CountExternalLinks<Friend>(targetA));
        Assert.Equal(0, world.CountExternalLinks<Friend>(targetB));
    }

    [Fact]
    public void GhoulifyOwner_OtherOwnerRelationsUnaffected()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity ownerA = assemblage.Create(new Position());
        Entity ownerB = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(ownerA, target);
        world.TryAddRelation<Friend>(ownerB, target);

        world.TryGhoulify(ownerA);

        Assert.Equal(1, world.CountExternalLinks<Friend>(target));

        Assert.True(world.IsRelatedTo<Friend>(ownerB, target));
    }

    [Fact]
    public void GhoulifyTarget_OwnerLosesRelationToTarget()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, target);

        world.TryGhoulify(target);

        Assert.Equal(0, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void GhoulifyTarget_MultipleOwners_AllLoseRelationToTarget()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity ownerA = assemblage.Create(new Position());
        Entity ownerB = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(ownerA, target);
        world.TryAddRelation<Friend>(ownerB, target);

        world.TryGhoulify(target);

        Assert.Equal(0, world.CountRelations<Friend>(ownerA));
        Assert.Equal(0, world.CountRelations<Friend>(ownerB));
    }

    [Fact]
    public void GhoulifyTarget_OtherOwnerRelationsUnaffected()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddRelation<Friend>(owner, targetA);
        world.TryAddRelation<Friend>(owner, targetB);

        world.TryGhoulify(targetA);

        Assert.Equal(1, world.CountRelations<Friend>(owner));
        Assert.True(world.IsRelatedTo<Friend>(owner, targetB));
    }
}
