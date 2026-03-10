using System;
using Theia.ECS.Archetypes;

namespace Theia.Tests.ECS.Archetypes;

public class StorageTests
{
    private struct ComponentA
    {
        public int X;
    }

    [Fact]
    public void Values_WithAllComponentsSet_ReturnsMatchingSpan()
    {
        int length = 10;

        Storage<ComponentA> storage = new Storage<ComponentA>(length);

        for (int i = 0; i < length; i++)
            storage.Set(i, new ComponentA() { X = i });

        Span<ComponentA> components = storage.Values(length);

        for (int i = 0; i < length; i++)
            Assert.Equal(i, components[i].X);
    }

    [Fact]
    public void SetAndGet_WithValidIndex_ReturnsSetComponent()
    {
        int length = 1;
        Storage<ComponentA> storage = new Storage<ComponentA>(length);

        int targetIndex = 0;
        int x = 10;
        ComponentA component = new ComponentA() { X = x };

        storage.Set(targetIndex, component);

        Assert.Equal(x, storage.Get(targetIndex).X);
    }

    [Fact]
    public void Move_WithValidIndexes_CopiesValueToTarget()
    {
        int lengthA = 10;

        Storage<ComponentA> storage = new Storage<ComponentA>(lengthA);

        int from = 0;
        int value = 5;

        storage.Set(from, new ComponentA() { X = value });

        int to = 5;

        storage.Move(from, to);

        Assert.Equal(storage.Get(from).X, storage.Get(to).X);
    }

    [Fact]
    public void Transfer_WithValidIndexes_CopiesValueToTargetStorage()
    {
        int lengthA = 10;
        int lengthB = 20;
        int value = 42;

        Storage<ComponentA> storageA = new Storage<ComponentA>(lengthA);
        Storage storageB = new Storage<ComponentA>(lengthB);

        int oldIndex = 9;
        int newIndex = 15;

        storageA.Set(oldIndex, new ComponentA() { X = value });

        storageA.Transfer(oldIndex, newIndex, storageB);

        Assert.Equal(storageA.Get(oldIndex).X, ((Storage<ComponentA>)storageB).Get(newIndex).X);
    }
}
