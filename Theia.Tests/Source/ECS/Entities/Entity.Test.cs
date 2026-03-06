namespace Theia.Tests.ECS.Entities;

public class EntityTest
{
    [Fact]
    public void Equals_WithSameId_ReturnsTrue()
    {
        Theia.ECS.Entities.Entity entityA = new() { _id = 0 };
        Theia.ECS.Entities.Entity entityB = new() { _id = 0 };

        Assert.True(entityA.Equals(entityB));
    }

    [Fact]
    public void Equals_WithDifferentId_ReturnsFalse()
    {
        Theia.ECS.Entities.Entity entityA = new() { _id = 0 };
        Theia.ECS.Entities.Entity entityB = new() { _id = 1 };

        Assert.False(entityA.Equals(entityB));
    }

    [Fact]
    public void Equals_WithSameIdAndSameVersion_ReturnsTrue()
    {
        Theia.ECS.Entities.Entity entityA = new() { _id = 0, _version = 1 };
        Theia.ECS.Entities.Entity entityB = new() { _id = 0, _version = 1 };

        Assert.True(entityA.Equals(entityB));
    }

    [Fact]
    public void Equals_WithSameIdAndDifferentVersion_ReturnsFalse()
    {
        Theia.ECS.Entities.Entity entityA = new() { _id = 0 };
        Theia.ECS.Entities.Entity entityB = new() { _id = 0, _version = 1 };

        Assert.False(entityA.Equals(entityB));
    }

    [Fact]
    public void GetHashCode_WhenIdsAreEqual_ShouldBeEqual()
    {
        Theia.ECS.Entities.Entity entityA = new() { _id = 0 };
        Theia.ECS.Entities.Entity entityB = new() { _id = 0 };

        Assert.Equal(entityA.GetHashCode(), entityB.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WhenIdsDiffer_ShouldBeDifferent()
    {
        Theia.ECS.Entities.Entity entityA = new() { _id = 0 };
        Theia.ECS.Entities.Entity entityB = new() { _id = 1 };

        Assert.NotEqual(entityA.GetHashCode(), entityB.GetHashCode());
    }
}
