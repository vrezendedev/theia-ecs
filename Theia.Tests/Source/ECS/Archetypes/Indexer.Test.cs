using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.Tests.ECS.Archetypes;

public sealed class IndexerTests
{
    [Fact]
    public void Count_WithEmptyIndexer_ReturnsZero()
    {
        Indexer indexer = new Indexer(10);

        Assert.Equal(0, indexer.Count());
    }

    [Fact]
    public void Count_WithHalfFullIndexer_ReturnsHalf()
    {
        int capacity = 10;

        int half = capacity / 2;

        Indexer indexer = new Indexer(capacity);

        for (int i = 0; i < half; i++)
            indexer.Add();

        Assert.Equal(half, indexer.Count());
    }

    [Fact]
    public void IsFull_WithFilledIndexer_ReturnsTrue()
    {
        int capacity = 10;

        Indexer indexer = new Indexer(capacity);

        for (int i = 0; i < capacity; i++)
            indexer.Add();

        Assert.True(indexer.IsFull());
    }

    [Fact]
    public void IsFull_WithEmptyIndexer_ReturnsFalse()
    {
        int capacity = 10;

        Indexer indexer = new Indexer(capacity);

        Assert.False(indexer.IsFull());
    }

    [Fact]
    public void IsFull_WithHalfFullIndexer_ReturnsFalse()
    {
        int capacity = 10;

        int half = capacity / 2;

        Indexer indexer = new Indexer(capacity);

        for (int i = 0; i < half; i++)
            indexer.Add();

        Assert.False(indexer.IsFull());
    }

    [Fact]
    public void Values_WithAllEntitiesSet_ReturnsMatchingSpan()
    {
        int length = 10;

        Indexer indexer = new Indexer(length);

        for (int i = 0; i < length; i++)
        {
            int index = indexer.Add();
            indexer.Set(index, new Entity() { _id = i });
        }

        Span<Entity> entities = indexer.GetValues();

        for (int i = 0; i < length; i++)
            Assert.Equal(i, entities[i]._id);
    }

    [Fact]
    public void SetAndGet_WithValidIndex_ReturnsSetEntity()
    {
        int length = 1;
        Indexer indexer = new Indexer(length);

        int targetIndex = 0;
        int id = 10;
        Entity entity = new Entity() { _id = id };

        indexer.Set(targetIndex, entity);

        Assert.Equal(id, indexer.Get(targetIndex)._id);
    }

    [Fact]
    public void Add_WithNonFullIndexer_ReturnsNextIndexAndIncrementsCount()
    {
        int length = 10;

        Indexer indexer = new Indexer(length);

        int nextValidIndex = indexer.Count();

        int validIndex = indexer.Add();

        Assert.Equal(nextValidIndex, validIndex);
        Assert.Equal(nextValidIndex + 1, indexer.Count());
    }

    [Fact]
    public void Remove_WithNonLastIndex_ReturnsSwappedIndexAndDecrementsCount()
    {
        int length = 10;

        Indexer indexer = new Indexer(length);

        for (int i = 0; i < length; i++)
        {
            int index = indexer.Add();
            indexer.Set(index, new Entity() { _id = i });
        }

        int lastID = 9;
        int removeIndex = 3;

        int swapped = indexer.Remove(removeIndex);

        Assert.Equal(lastID, swapped);
        Assert.Equal(lastID, indexer.Get(removeIndex)._id);
    }

    [Fact]
    public void Transfer_WithPartiallyFilledTarget_ReturnsNextAvailableIndex()
    {
        int lengthA = 10;
        int lengthB = 10;
        int lengthBHalf = lengthB / 2;

        Indexer indexerA = new Indexer(lengthA);
        Indexer indexerB = new Indexer(lengthB);

        for (int i = 0; i < lengthA; i++)
        {
            int index = indexerA.Add();
            indexerA.Set(index, new Entity() { _id = i });
        }

        for (int i = 0; i < lengthBHalf; i++)
        {
            int index = indexerB.Add();
            indexerB.Set(index, new Entity() { _id = lengthA + i });
        }

        int targetIndex = 8;

        int newIndex = indexerA.Transfer(targetIndex, indexerB);

        Assert.Equal(newIndex, lengthBHalf);
    }
}
