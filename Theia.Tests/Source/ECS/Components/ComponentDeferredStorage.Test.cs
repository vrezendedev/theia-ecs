using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Components;

public sealed class ComponentDeferredStorageTests
{
    private readonly Signature _signature = new Signature(
        [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
    );

    private (Archetype archetype, EntityMeta meta) CreateEntityInArchetype()
    {
        Archetype archetype = new Archetype(1, _signature);

        EntityAccounted accounted = archetype.Add(new Entity() { _id = 1 });
        EntityMeta meta = new EntityMeta(
            0,
            accounted._archetypeIndex,
            accounted._storageIndex,
            accounted._componentIndex
        );

        return (archetype, meta);
    }

    [Fact]
    public void EnqueueDeferred_SingleValue_DoesNotThrow()
    {
        ComponentDeferredStorage<Position> storage = new ComponentDeferredStorage<Position>(4);

        storage.EnqueueDeferred(new Position { X = 1, Y = 2 });
    }

    [Fact]
    public void EnqueueDeferred_MultipleValues_DoesNotThrow()
    {
        ComponentDeferredStorage<Position> storage = new ComponentDeferredStorage<Position>(4);

        storage.EnqueueDeferred(new Position { X = 1 });
        storage.EnqueueDeferred(new Position { X = 2 });
        storage.EnqueueDeferred(new Position { X = 3 });
    }

    [Fact]
    public void SetWithNext_AfterEnqueue_WritesComponentToArchetype()
    {
        (Archetype archetype, EntityMeta meta) = CreateEntityInArchetype();

        ComponentDeferredStorage<Position> storage = new ComponentDeferredStorage<Position>(4);

        storage.EnqueueDeferred(new Position { X = 5, Y = 10 });

        storage.SetWithNext(in meta, archetype);

        ref Position position = ref archetype.Get<Position>(in meta);

        Assert.Equal(5, position.X);
        Assert.Equal(10, position.Y);
    }

    [Fact]
    public void SetWithNext_ConsumesValueInFifoOrder()
    {
        (Archetype archetype, EntityMeta metaOne) = CreateEntityInArchetype();

        EntityAccounted accountedTwo = archetype.Add(new Entity() { _id = 2 });

        EntityMeta metaTwo = new EntityMeta(
            0,
            accountedTwo._archetypeIndex,
            accountedTwo._storageIndex,
            accountedTwo._componentIndex
        );

        ComponentDeferredStorage<Position> storage = new ComponentDeferredStorage<Position>(4);

        storage.EnqueueDeferred(new Position { X = 1 });
        storage.EnqueueDeferred(new Position { X = 2 });

        storage.SetWithNext(in metaOne, archetype);
        storage.SetWithNext(in metaTwo, archetype);

        Assert.Equal(1, archetype.Get<Position>(in metaOne).X);
        Assert.Equal(2, archetype.Get<Position>(in metaTwo).X);
    }

    [Fact]
    public void SetWithNext_CalledTwice_EachEntityReceivesCorrectValue()
    {
        (Archetype archetype, EntityMeta metaOne) = CreateEntityInArchetype();

        EntityAccounted accountedTwo = archetype.Add(new Entity() { _id = 2 });
        EntityMeta metaTwo = new EntityMeta(
            0,
            accountedTwo._archetypeIndex,
            accountedTwo._storageIndex,
            accountedTwo._componentIndex
        );

        ComponentDeferredStorage<Position> storage = new ComponentDeferredStorage<Position>(4);

        storage.EnqueueDeferred(new Position { X = 10, Y = 20 });
        storage.EnqueueDeferred(new Position { X = 30, Y = 40 });

        storage.SetWithNext(in metaOne, archetype);
        storage.SetWithNext(in metaTwo, archetype);

        Assert.Equal(10, archetype.Get<Position>(in metaOne).X);
        Assert.Equal(20, archetype.Get<Position>(in metaOne).Y);
        Assert.Equal(30, archetype.Get<Position>(in metaTwo).X);
        Assert.Equal(40, archetype.Get<Position>(in metaTwo).Y);
    }

    [Fact]
    public void DiscardNext_AfterEnqueue_NextSetWithNextUsesFollowingValue()
    {
        (Archetype archetype, EntityMeta meta) = CreateEntityInArchetype();

        ComponentDeferredStorage<Position> storage = new ComponentDeferredStorage<Position>(4);

        storage.EnqueueDeferred(new Position { X = 99 });
        storage.EnqueueDeferred(new Position { X = 42 });

        storage.DiscardNext();
        storage.SetWithNext(in meta, archetype);

        Assert.Equal(42, archetype.Get<Position>(in meta).X);
    }

    [Fact]
    public void DiscardNext_MultipleDiscards_RemainingValuesPreserveFifoOrder()
    {
        (Archetype archetype, EntityMeta meta) = CreateEntityInArchetype();

        ComponentDeferredStorage<Position> storage = new ComponentDeferredStorage<Position>(4);

        storage.EnqueueDeferred(new Position { X = 1 });
        storage.EnqueueDeferred(new Position { X = 2 });
        storage.EnqueueDeferred(new Position { X = 3 });

        storage.DiscardNext();
        storage.DiscardNext();

        storage.SetWithNext(in meta, archetype);

        Assert.Equal(3, archetype.Get<Position>(in meta).X);
    }
}
