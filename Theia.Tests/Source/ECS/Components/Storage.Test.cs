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
            storage.Set(i, new ComponentA() { A = i });

        Span<ComponentA> components = storage.GetValues(length);

        for (int i = 0; i < length; i++)
            Assert.Equal(i, components[i].A);
    }

    [Fact]
    public void SetAndGet_WithValidIndex_ReturnsSetComponent()
    {
        int length = 1;
        Storage<ComponentA> storage = new Storage<ComponentA>(length);

        int targetIndex = 0;
        int x = 10;
        ComponentA component = new ComponentA() { A = x };

        storage.Set(targetIndex, component);

        Assert.Equal(x, storage.Get(targetIndex).A);
    }

    [Fact]
    public void Move_WithValidIndexes_CopiesValueToTarget()
    {
        int lengthA = 10;

        Storage<ComponentA> storage = new Storage<ComponentA>(lengthA);

        int from = 0;
        int value = 5;

        storage.Set(from, new ComponentA() { A = value });

        int to = 5;

        storage.Move(from, to);

        Assert.Equal(storage.Get(from).A, storage.Get(to).A);
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

        storageA.Set(oldIndex, new ComponentA() { A = value });

        storageA.Transfer(oldIndex, newIndex, storageB);

        Assert.Equal(storageA.Get(oldIndex).A, ((Storage<ComponentA>)storageB).Get(newIndex).A);
    }
}
