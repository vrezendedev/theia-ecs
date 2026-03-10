using Theia.ECS.Entities;

namespace Theia.Tests.ECS.Entities;

public class EntityTests
{
    [Fact]
    public void Equals_WithSameId_ReturnsTrue()
    {
        Entity entityA = new() { _id = 0 };
        Entity entityB = new() { _id = 0 };

        Assert.True(entityA.Equals(entityB));
    }

    [Fact]
    public void Equals_WithDifferentId_ReturnsFalse()
    {
        Entity entityA = new() { _id = 0 };
        Entity entityB = new() { _id = 1 };

        Assert.False(entityA.Equals(entityB));
    }

    [Fact]
    public void Equals_WithSameIdAndSameVersion_ReturnsTrue()
    {
        Entity entityA = new() { _id = 0, _version = 1 };
        Entity entityB = new() { _id = 0, _version = 1 };

        Assert.True(entityA.Equals(entityB));
    }

    [Fact]
    public void Equals_WithSameIdAndDifferentVersion_ReturnsFalse()
    {
        Entity entityA = new() { _id = 0 };
        Entity entityB = new() { _id = 0, _version = 1 };

        Assert.False(entityA.Equals(entityB));
    }

    [Fact]
    public void GetHashCode_WhenIdsAreEqual_ShouldBeEqual()
    {
        Entity entityA = new() { _id = 0 };
        Entity entityB = new() { _id = 0 };

        Assert.Equal(entityA.GetHashCode(), entityB.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WhenIdsDiffer_ShouldBeDifferent()
    {
        Entity entityA = new() { _id = 0 };
        Entity entityB = new() { _id = 1 };

        Assert.NotEqual(entityA.GetHashCode(), entityB.GetHashCode());
    }
}
