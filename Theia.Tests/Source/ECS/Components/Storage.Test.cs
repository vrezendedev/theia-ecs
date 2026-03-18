using System;
using Theia.ECS.Components;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Components;

public sealed class StorageTests
{
    [Fact]
    public void Values_WithAllComponentsSet_ReturnsMatchingSpan()
    {
        int length = 10;

        Storage<ComponentA> storage = new Storage<ComponentA>(length);

        for (int i = 0; i < length; i++)
            storage.Set(i, new ComponentA() { AField = i });

        Span<ComponentA> components = storage.GetValues(length);

        for (int i = 0; i < length; i++)
            Assert.Equal(i, components[i].AField);
    }

    [Fact]
    public void SetAndGet_WithValidIndex_ReturnsSetComponent()
    {
        int length = 1;
        Storage<ComponentA> storage = new Storage<ComponentA>(length);

        int targetIndex = 0;
        int x = 10;
        ComponentA component = new ComponentA() { AField = x };

        storage.Set(targetIndex, component);

        Assert.Equal(x, storage.Get(targetIndex).AField);
    }

    [Fact]
    public void Move_WithValidIndexes_CopiesValueToTarget()
    {
        int lengthA = 10;

        Storage<ComponentA> storage = new Storage<ComponentA>(lengthA);

        int from = 0;
        int value = 5;

        storage.Set(from, new ComponentA() { AField = value });

        int to = 5;

        storage.Move(from, to);

        Assert.Equal(storage.Get(from).AField, storage.Get(to).AField);
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

        storageA.Set(oldIndex, new ComponentA() { AField = value });

        storageA.Transfer(oldIndex, newIndex, storageB);

        Assert.Equal(
            storageA.Get(oldIndex).AField,
            ((Storage<ComponentA>)storageB).Get(newIndex).AField
        );
    }
}
