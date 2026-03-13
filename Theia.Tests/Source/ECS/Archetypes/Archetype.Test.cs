using System;
using System.Collections.Generic;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Archetypes;

public sealed class ArchetypeTests
{
    private Signature _twoComponentSignature = new Signature(
        [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
    );

    private Signature _fourComponentSignature = new Signature(
        [
            ComponentMeta<Position>.s_id,
            ComponentMeta<Velocity>.s_id,
            ComponentMeta<Rotation>.s_id,
            ComponentMeta<Health>.s_id,
        ]
    );

    [Fact]
    public void Constructor_WithValidArchetypeId_StoresAssignedArchetypeId()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        Assert.Equal(1, archetype._archetypeId);
    }

    [Fact]
    public void Constructor_WithValidSignature_StoresAssignedSignature()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        Assert.Equal(_fourComponentSignature, archetype._signature);
    }

    [Fact]
    public void GetIndexers_BeforeAdd_IsEmpty()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        Assert.Equal(0, archetype.GetIndexers().Length);
    }

    [Fact]
    public void GetIndexers_AfterAdd_ReturnsOne()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        Entity entity = new Entity() { _id = 0 };
        archetype.Add(entity);

        Assert.Equal(1, archetype.GetIndexers().Length);
    }

    [Fact]
    public void Add_FirstEntity_ComponentIndexIsZero()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });

        Assert.Equal(0, accounted._componentIndex);
    }

    [Fact]
    public void Add_FirstEntity_ReturnsCorrectEntityAccounted()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });

        Assert.Equal(archetype._archetypeId, accounted._archetypeIndex);
        Assert.Equal(accounted._storageIndex, archetype.GetIndexers().Length - 1);
    }

    [Fact]
    public void Add_SecondEntity_ComponentIndexIsOne()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        Entity entityA = new Entity() { _id = 1 };
        Entity entityB = new Entity() { _id = 2 };

        archetype.Add(entityA);
        EntityAccounted accounted = archetype.Add(entityB);

        Assert.Equal(1, accounted._componentIndex);
    }

    [Fact]
    public void Add_MultipleEntities_AllStoredInIndexer()
    {
        const int count = 512;
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        for (int i = 0; i < count; i++)
            archetype.Add(new Entity() { _id = i });

        Span<Indexer> indexers = archetype.GetIndexers();
        int total = 0;

        foreach (Indexer indexer in indexers)
            total += indexer.Count();

        Assert.Equal(count, total);
    }

    [Fact]
    public void Remove_OnlyEntity_ReturnsNone()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });
        EntityMeta meta = new EntityMeta(0, 1, accounted._storageIndex, accounted._componentIndex);

        EntitySwapped swapped = archetype.Remove(meta);

        Assert.Equal(EntitySwapped.InvalidEntitySwappedIndexes, swapped._entityID);
    }

    [Fact]
    public void Remove_NonLastEntity_ReturnsSwappedEntityId()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accountedOne = archetype.Add(new Entity() { _id = 1 });
        archetype.Add(new Entity() { _id = 2 });

        EntityMeta metaOne = new EntityMeta(
            0,
            1,
            accountedOne._storageIndex,
            accountedOne._componentIndex
        );

        EntitySwapped swapped = archetype.Remove(metaOne);

        Assert.Equal(2, swapped._entityID);
        Assert.Equal(0, swapped._componentIndex);
    }

    [Fact]
    public void Remove_NonLastEntity_SwapsComponents()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accountedOne = archetype.Add(new Entity() { _id = 1 });
        EntityAccounted accountedTwo = archetype.Add(new Entity() { _id = 2 });

        EntityMeta metaOne = new EntityMeta(
            0,
            accountedOne._archetypeIndex,
            accountedOne._storageIndex,
            accountedOne._componentIndex
        );

        EntityMeta metaTwo = new EntityMeta(
            0,
            accountedTwo._archetypeIndex,
            accountedTwo._storageIndex,
            accountedTwo._componentIndex
        );

        archetype.Set(in metaTwo, new Position() { X = 10, Y = 15 });

        EntitySwapped swapped = archetype.Remove(metaOne);

        EntityMeta swappedMeta = new EntityMeta(
            0,
            1,
            metaTwo._storageIndex,
            swapped._componentIndex
        );

        ref Position position = ref archetype.Get<Position>(in swappedMeta);

        Assert.Equal(10, position.X);
        Assert.Equal(15, position.Y);
    }

    [Fact]
    public void Remove_LastEntity_DecrementsIndexerCount()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accountOne = archetype.Add(new Entity() { _id = 1 });
        archetype.Add(new Entity() { _id = 2 });

        EntityMeta metaOne = new EntityMeta(
            0,
            accountOne._archetypeIndex,
            accountOne._storageIndex,
            accountOne._componentIndex
        );

        archetype.Remove(metaOne);

        Span<Indexer> indexers = archetype.GetIndexers();

        int total = 0;

        foreach (Indexer idx in indexers)
            total += idx.Count();

        Assert.Equal(1, total);
    }

    [Fact]
    public void Remove_EntityAndReAdd_SlotIsReused()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });
        EntityMeta meta = new EntityMeta(
            0,
            accounted._archetypeIndex,
            accounted._storageIndex,
            accounted._componentIndex
        );

        archetype.Remove(meta);

        EntityAccounted accountedTwo = archetype.Add(new Entity() { _id = 2 });

        Assert.Equal(accounted._storageIndex, accountedTwo._storageIndex);
        Assert.Equal(accounted._componentIndex, accountedTwo._componentIndex);
    }

    [Fact]
    public void Set_ThenGet_ReturnsStoredValue()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });
        EntityMeta meta = new EntityMeta(
            0,
            accounted._archetypeIndex,
            accounted._storageIndex,
            accounted._componentIndex
        );

        archetype.Set(in meta, new Position { X = 3, Y = 7 });

        ref Position pos = ref archetype.Get<Position>(in meta);

        Assert.Equal(3, pos.X);
        Assert.Equal(7, pos.Y);
    }

    [Fact]
    public void Get_ReturnsRefToLiveData_MutationIsPersisted()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });
        EntityMeta meta = new EntityMeta(
            0,
            accounted._archetypeIndex,
            accounted._storageIndex,
            accounted._componentIndex
        );

        archetype.Set(in meta, new Position { X = 1, Y = 1 });
        archetype.Get<Position>(meta).X = 99;

        Assert.Equal(99, archetype.Get<Position>(in meta).X);
    }

    [Fact]
    public void Set_MultipleEntities_DataIsIndependent()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accountedOne = archetype.Add(new Entity() { _id = 1 });
        EntityAccounted accountedTwo = archetype.Add(new Entity() { _id = 2 });
        EntityMeta metaOne = new EntityMeta(
            0,
            accountedOne._archetypeIndex,
            accountedOne._storageIndex,
            accountedOne._componentIndex
        );
        EntityMeta metaTwo = new EntityMeta(
            0,
            accountedTwo._archetypeIndex,
            accountedTwo._storageIndex,
            accountedTwo._componentIndex
        );

        archetype.Set(in metaOne, new Position { X = 10 });
        archetype.Set(in metaTwo, new Position { X = 20 });

        Assert.Equal(10f, archetype.Get<Position>(in metaOne).X);
        Assert.Equal(20f, archetype.Get<Position>(in metaTwo).X);
    }

    [Fact]
    public void Set_MultipleComponents_EachReadBackCorrectly()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });
        EntityMeta meta = new EntityMeta(
            0,
            accounted._archetypeIndex,
            accounted._storageIndex,
            accounted._componentIndex
        );

        archetype.Set(in meta, new Position { X = 5, Y = 5 });
        archetype.Set(in meta, new Velocity { X = 1, Y = -1 });

        Assert.Equal(5f, archetype.Get<Position>(in meta).X);
        Assert.Equal(1f, archetype.Get<Velocity>(in meta).X);
        Assert.Equal(-1f, archetype.Get<Velocity>(in meta).Y);
    }

    [Fact]
    public void Transfer_Entity_RemovesFromSourceArchetype()
    {
        Archetype from = new Archetype(1, _twoComponentSignature);
        Archetype to = new Archetype(2, _fourComponentSignature);

        Entity entity = new Entity() { _id = 1 };
        EntityAccounted accounted = from.Add(entity);
        EntityMeta meta = new EntityMeta(
            0,
            accounted._archetypeIndex,
            accounted._storageIndex,
            accounted._componentIndex
        );

        from.Transfer(entity, meta, to);

        Span<Indexer> fromIndexers = from.GetIndexers();

        int total = 0;

        foreach (Indexer idx in fromIndexers)
            total += idx.Count();

        Assert.Equal(0, total);
    }

    [Fact]
    public void Transfer_Entity_AppearsInDestinationArchetype()
    {
        Archetype from = new Archetype(1, _twoComponentSignature);
        Archetype to = new Archetype(2, _fourComponentSignature);

        Entity entity = new Entity() { _id = 1 };
        EntityAccounted accounted = from.Add(entity);
        EntityMeta meta = new EntityMeta(
            0,
            accounted._archetypeIndex,
            accounted._storageIndex,
            accounted._componentIndex
        );

        from.Transfer(entity, meta, to);

        Span<Indexer> toIndexers = to.GetIndexers();

        int total = 0;

        foreach (Indexer idx in toIndexers)
            total += idx.Count();

        Assert.Equal(1, total);
    }

    [Fact]
    public void Transfer_Entity_PreservesSharedComponentData()
    {
        Archetype from = new Archetype(1, _twoComponentSignature);
        Archetype to = new Archetype(2, _fourComponentSignature);

        Entity entity = new Entity() { _id = 1 };
        EntityAccounted fromAccounted = from.Add(entity);
        EntityMeta fromMeta = new EntityMeta(
            0,
            fromAccounted._archetypeIndex,
            fromAccounted._storageIndex,
            fromAccounted._componentIndex
        );

        from.Set(in fromMeta, new Position { X = 42, Y = 13 });

        EntityTransferred transferred = from.Transfer(entity, fromMeta, to);
        EntityAccounted toAccounted = transferred._entityAccounted;
        EntityMeta toMeta = new EntityMeta(
            0,
            toAccounted._archetypeIndex,
            toAccounted._storageIndex,
            toAccounted._componentIndex
        );

        Assert.Equal(42, to.Get<Position>(in toMeta).X);
        Assert.Equal(13, to.Get<Position>(in toMeta).Y);
    }

    [Fact]
    public void Transfer_WhenSourceHadMultipleEntities_ReturnsCorrectSwappedInfo()
    {
        Archetype from = new Archetype(1, _twoComponentSignature);
        Archetype to = new Archetype(2, _fourComponentSignature);

        Entity entity = new Entity() { _id = 1 };
        EntityAccounted accountedOne = from.Add(entity);
        from.Add(new Entity() { _id = 2 });

        EntityMeta metaOne = new EntityMeta(
            0,
            accountedOne._archetypeIndex,
            accountedOne._storageIndex,
            accountedOne._componentIndex
        );

        EntityTransferred transferred = from.Transfer(entity, metaOne, to);

        Assert.Equal(2, transferred._entitySwapped._entityID);
    }

    [Fact]
    public void Transfer_WhenSourceHadMultipleEntities_SwappedEntityShouldPreserveComponentData()
    {
        Archetype from = new Archetype(1, _twoComponentSignature);
        Archetype to = new Archetype(2, _fourComponentSignature);

        EntityAccounted accountedOne = from.Add(new Entity() { _id = 1 });
        EntityAccounted accountedTwo = from.Add(new Entity() { _id = 2 });

        EntityMeta metaOne = new EntityMeta(
            0,
            accountedOne._archetypeIndex,
            accountedOne._storageIndex,
            accountedOne._componentIndex
        );
        EntityMeta metaTwo = new EntityMeta(
            0,
            accountedTwo._archetypeIndex,
            accountedTwo._storageIndex,
            accountedTwo._componentIndex
        );

        from.Set(in metaTwo, new Position { X = 99, Y = 42 });

        EntityTransferred transferred = from.Transfer(new Entity() { _id = 1 }, metaOne, to);

        EntityMeta swappedMeta = new EntityMeta(
            0,
            from._archetypeId,
            accountedOne._storageIndex,
            transferred._entitySwapped._componentIndex
        );

        Assert.Equal(99, from.Get<Position>(in swappedMeta).X);
        Assert.Equal(42, from.Get<Position>(in swappedMeta).Y);
    }

    [Fact]
    public void Add_BeyondInitialChunkCapacity_AllEntitiesAreAccessible()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);
        int numberOfChunks = 2;
        int count = archetype._capacity * numberOfChunks;

        List<EntityMeta> entityMetas = new List<EntityMeta>();

        for (int i = 0; i < count; i++)
        {
            EntityAccounted accounted = archetype.Add(new Entity() { _id = i });
            EntityMeta meta = new EntityMeta(
                0,
                accounted._archetypeIndex,
                accounted._storageIndex,
                accounted._componentIndex
            );
            archetype.Set(in meta, new Position { X = i, Y = -i });
            entityMetas.Add(meta);
        }

        for (int i = 0; i < count; i++)
        {
            ref Position pos = ref archetype.Get<Position>(entityMetas[i]);
            Assert.Equal(i, pos.X);
            Assert.Equal(-i, pos.Y);
        }

        Assert.Equal(count, entityMetas.Count);
        Assert.Equal(numberOfChunks, archetype.GetIndexers().Length);
    }

    [Fact]
    public void Add_BeyondInitialChunkCapacity_IndexerCountMatchesEntityCount()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);
        int count = archetype._capacity * 2;

        for (int i = 0; i < count; i++)
            archetype.Add(new Entity() { _id = i });

        Span<Indexer> indexers = archetype.GetIndexers();
        int total = 0;

        foreach (Indexer idx in indexers)
            total += idx.Count();

        Assert.Equal(count, total);
    }

    [Fact]
    public void GetStorages_BeforeAnyAdd_ShouldBeEmpty()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);

        Assert.Equal(0, archetype.GetStorages<Position>().Length);
    }

    [Fact]
    public void GetStorages_AfterAdd_ShouldHaveOneEntry()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);
        archetype.Add(new Entity() { _id = 1 });

        Assert.Equal(1, archetype.GetStorages<Position>().Length);
    }

    [Fact]
    public void GetStorages_AfterMultipleAdd_CountShouldMatchInitialisedChunkCount()
    {
        Archetype archetype = new Archetype(1, _fourComponentSignature);
        int numberOfChunks = 4;
        int count = archetype._capacity * numberOfChunks;

        for (int i = 0; i < count; i++)
            archetype.Add(new Entity() { _id = i });

        int storageChunks = archetype.GetStorages<Position>().Length;
        int indexerChunks = archetype.GetIndexers().Length;

        Assert.Equal(indexerChunks, storageChunks);
        Assert.Equal(numberOfChunks, indexerChunks);
        Assert.Equal(numberOfChunks, storageChunks);
    }
}
