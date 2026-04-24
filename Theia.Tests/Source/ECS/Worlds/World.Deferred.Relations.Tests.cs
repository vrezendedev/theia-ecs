using System;
using Theia.ECS.Assemblages;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

[Collection("MetaRequester")]
public sealed class WorldDeferredRelationsTests
{
    [Fact]
    public void DeferredAddRelation_AfterFlush_RelationIsEstablished()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddRelation<Friend>(owner, target);

        Assert.False(world.HasRelation<Friend>(owner));

        world.FlushDeferred();

        Assert.True(world.HasRelation<Friend>(owner));
        Assert.True(world.IsRelatedTo<Friend>(owner, target));
    }

    [Fact]
    public void DeferredAddRelation_AfterFlush_TargetGainsExternalLink()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddRelation<Friend>(owner, target);

        world.FlushDeferred();

        Assert.Equal(1, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void DeferredAddRelation_WithEvaluatedRelationType_ThrowsImmediately()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            world.DeferredAddRelation<Damage>(owner, target)
        );
    }

    [Fact]
    public void DeferredAddRelation_OwnerDeadAtFlushTime_Discarded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddRelation<Friend>(owner, target);

        world.TryGhoulify(owner);

        world.FlushDeferred();

        Assert.Equal(0, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void DeferredAddRelation_TargetDeadAtFlushTime_Discarded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddRelation<Friend>(owner, target);

        world.TryGhoulify(target);

        world.FlushDeferred();

        Assert.False(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void DeferredAddRelation_EnqueuedMultipleTimes_AllEstablishedOnFlush()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());
        Entity targetC = assemblage.Create(new Position());

        world.DeferredAddRelation<Friend>(owner, targetA);
        world.DeferredAddRelation<Friend>(owner, targetB);
        world.DeferredAddRelation<Friend>(owner, targetC);

        world.FlushDeferred();

        Assert.Equal(3, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void DeferredAddRelation_FlushTwice_SecondFlushIsIdempotent()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddRelation<Friend>(owner, target);

        world.FlushDeferred();
        world.FlushDeferred();

        Assert.Equal(1, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void DeferredAddEvaluatedRelation_AfterFlush_RelationIsEstablished()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddEvaluatedRelation(owner, target, new Damage { Value = 10f });

        Assert.False(world.HasRelation<Damage>(owner));

        world.FlushDeferred();

        Assert.True(world.HasRelation<Damage>(owner));
    }

    [Fact]
    public void DeferredAddEvaluatedRelation_AfterFlush_ValueIsStoredCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddEvaluatedRelation(owner, target, new Damage { Value = 42f });

        world.FlushDeferred();

        Assert.Equal(42f, world.GetEvaluatedRelation<Damage>(owner, target).Value);
    }

    [Fact]
    public void DeferredAddEvaluatedRelation_WithTagRelationType_ThrowsImmediately()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        Assert.Throws<InvalidOperationException>(() =>
            world.DeferredAddEvaluatedRelation(owner, target, new Friend())
        );
    }

    [Fact]
    public void DeferredAddEvaluatedRelation_OwnerDeadAtFlushTime_Discarded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddEvaluatedRelation(owner, target, new Damage { Value = 5f });

        world.TryGhoulify(owner);

        world.FlushDeferred();

        Assert.Equal(0, world.CountExternalLinks<Damage>(target));
    }

    [Fact]
    public void DeferredAddEvaluatedRelation_MultipleTargets_EachValueStoredCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.DeferredAddEvaluatedRelation(owner, targetA, new Damage { Value = 10f });
        world.DeferredAddEvaluatedRelation(owner, targetB, new Damage { Value = 20f });

        world.FlushDeferred();

        Assert.Equal(10f, world.GetEvaluatedRelation<Damage>(owner, targetA).Value);
        Assert.Equal(20f, world.GetEvaluatedRelation<Damage>(owner, targetB).Value);
    }

    [Fact]
    public void DeferredRemoveRelation_AfterFlush_RelationIsRemoved()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, target);

        world.DeferredRemoveRelation<Friend>(owner, target);

        Assert.True(world.HasRelation<Friend>(owner));

        world.FlushDeferred();

        Assert.False(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void DeferredRemoveRelation_AfterFlush_TargetLosesExternalLink()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, target);

        world.DeferredRemoveRelation<Friend>(owner, target);

        world.FlushDeferred();

        Assert.Equal(0, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void DeferredRemoveRelation_WithNoExistingRelation_DiscardedSilently()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredRemoveRelation<Friend>(owner, target);

        Assert.False(world.HasRelation<Friend>(owner));

        world.FlushDeferred();

        Assert.False(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void DeferredRemoveRelation_OwnerDeadAtFlushTime_DiscardedSilently()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, target);

        world.DeferredRemoveRelation<Friend>(owner, target);

        world.TryGhoulify(owner);

        world.FlushDeferred();

        Assert.Equal(0, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void DeferredRemoveRelation_OtherRelationsUnaffected()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, targetA);
        world.TryAddTagRelation<Friend>(owner, targetB);

        world.DeferredRemoveRelation<Friend>(owner, targetA);

        world.FlushDeferred();

        Assert.True(world.IsRelatedTo<Friend>(owner, targetB));
        Assert.Equal(1, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void FlushDeferred_AddThenRemove_InSameFlush_NetResultIsNoRelation()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredAddRelation<Friend>(owner, target);
        world.DeferredRemoveRelation<Friend>(owner, target);

        world.FlushDeferred();

        Assert.False(world.HasRelation<Friend>(owner));
        Assert.Equal(0, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void FlushDeferred_GhoulifyOwnerThenAddRelation_InSameFlush_RelationDiscarded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredGhoulify(owner);
        world.DeferredAddRelation<Friend>(owner, target);

        world.FlushDeferred();

        Assert.Equal(0, world.CountExternalLinks<Friend>(target));
    }

    [Fact]
    public void FlushDeferred_GhoulifyTargetThenAddRelation_InSameFlush_RelationDiscarded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.DeferredGhoulify(target);
        world.DeferredAddRelation<Friend>(owner, target);

        world.FlushDeferred();

        Assert.False(world.HasRelation<Friend>(owner));
    }

    [Fact]
    public void FlushDeferred_MultipleAddsAndRemoves_AllProcessedInEnqueueOrder()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity targetA = assemblage.Create(new Position());
        Entity targetB = assemblage.Create(new Position());
        Entity targetC = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, targetA);

        world.DeferredRemoveRelation<Friend>(owner, targetA);
        world.DeferredAddRelation<Friend>(owner, targetB);
        world.DeferredAddRelation<Friend>(owner, targetC);

        world.FlushDeferred();

        Assert.False(world.IsRelatedTo<Friend>(owner, targetA));
        Assert.True(world.IsRelatedTo<Friend>(owner, targetB));
        Assert.True(world.IsRelatedTo<Friend>(owner, targetC));
        Assert.Equal(2, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void DeferredAddRelation_DuringFlush_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        assemblage.EntityEvents.OnCreated += _ => world.DeferredAddRelation<Friend>(owner, target);

        assemblage.DeferredCreate(new Position());

        Assert.Throws<InvalidOperationException>(() => world.FlushDeferred());
    }

    [Fact]
    public void DeferredRemoveRelation_DuringFlush_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        world.TryAddTagRelation<Friend>(owner, target);

        assemblage.EntityEvents.OnCreated += _ =>
            world.DeferredRemoveRelation<Friend>(owner, target);

        assemblage.DeferredCreate(new Position());

        Assert.Throws<InvalidOperationException>(() => world.FlushDeferred());
    }

    [Fact]
    public void DeferredAddEvaluatedRelation_DuringFlush_Throws()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());
        Entity target = assemblage.Create(new Position());

        assemblage.EntityEvents.OnCreated += _ =>
            world.DeferredAddEvaluatedRelation(owner, target, new Damage { Value = 1f });

        assemblage.DeferredCreate(new Position());

        Assert.Throws<InvalidOperationException>(() => world.FlushDeferred());
    }

    [Fact]
    public void DeferredCreate_WithTagRelation_OwnerIsRelatedToCreatedEntity()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Friend> { Owner = owner }
        );
        world.FlushDeferred();

        Assert.Equal(1, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void DeferredCreate_WithTagRelation_CreatedEntityHasExternalLink()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity createdEntity = default;

        assemblage.EntityEvents.OnCreated += e => createdEntity = e.Entity;

        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Friend> { Owner = owner }
        );

        world.FlushDeferred();

        Assert.Equal(1, world.CountExternalLinks<Friend>(createdEntity));
    }

    [Fact]
    public void DeferredCreate_WithTagRelation_MultipleTimes_OwnerRelatedToAll()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Friend> { Owner = owner }
        );
        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Friend> { Owner = owner }
        );
        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Friend> { Owner = owner }
        );

        world.FlushDeferred();

        Assert.Equal(3, world.CountRelations<Friend>(owner));
    }

    [Fact]
    public void DeferredCreate_WithTagRelation_OwnerDeadAtFlushTime_EntityCreatedButRelationDiscarded()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        int entitiesCreated = 0;

        assemblage.EntityEvents.OnCreated += _ => entitiesCreated++;

        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Friend> { Owner = owner }
        );

        world.TryGhoulify(owner);

        world.FlushDeferred();

        Assert.Equal(1, entitiesCreated);
    }

    [Fact]
    public void DeferredCreate_WithoutRelation_EntityCreatedNormally()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        int entitiesCreated = 0;

        assemblage.EntityEvents.OnCreated += _ => entitiesCreated++;

        assemblage.DeferredCreate(new Position());

        world.FlushDeferred();

        Assert.Equal(1, entitiesCreated);
    }

    [Fact]
    public void DeferredCreate_WithEvaluatedRelation_OwnerIsRelatedToCreatedEntity()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Damage>
            {
                Owner = owner,
                Relation = new Damage { Value = 7f },
            }
        );

        world.FlushDeferred();

        Assert.Equal(1, world.CountRelations<Damage>(owner));
    }

    [Fact]
    public void DeferredCreate_WithEvaluatedRelation_ValueIsStoredCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        Entity createdEntity = default;

        assemblage.EntityEvents.OnCreated += e => createdEntity = e.Entity;

        assemblage.DeferredCreate(
            new Position(),
            new DeferredRelationOnCreate<Damage>
            {
                Owner = owner,
                Relation = new Damage { Value = 99f },
            }
        );

        world.FlushDeferred();

        Assert.Equal(99f, world.GetEvaluatedRelation<Damage>(owner, createdEntity).Value);
    }

    [Fact]
    public void DeferredCreate_WithEvaluatedRelation_MultipleTimes_EachValueStoredCorrectly()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        Entity owner = assemblage.Create(new Position());

        int times = 512;
        float sum = 0;

        for (int i = 0; i < times; i++)
        {
            assemblage.DeferredCreate(
                new Position(),
                new DeferredRelationOnCreate<Damage>
                {
                    Owner = owner,
                    Relation = new Damage { Value = i },
                }
            );

            sum += i;
        }

        world.FlushDeferred();

        EntityEvaluatedRelations<Damage> relations = world.GetEvaluatedRelations<Damage>(owner);

        Assert.Equal(times, relations.Entities.Length);

        float dataSum = 0;

        for (int i = 0; i < relations.Data.Length; i++)
            dataSum += relations.Data[i].Value;

        Assert.Equal(dataSum, sum);

        for (int i = 0; i < times; i++)
        {
            assemblage.DeferredCreate(
                new Position(),
                new DeferredRelationOnCreate<Damage>
                {
                    Owner = owner,
                    Relation = new Damage { Value = i },
                }
            );

            sum += i;
        }

        world.FlushDeferred();

        relations = world.GetEvaluatedRelations<Damage>(owner);

        Assert.Equal(times * 2, relations.Entities.Length);

        dataSum = 0;

        for (int i = 0; i < relations.Data.Length; i++)
            dataSum += relations.Data[i].Value;

        Assert.Equal(dataSum, sum);
    }
}
