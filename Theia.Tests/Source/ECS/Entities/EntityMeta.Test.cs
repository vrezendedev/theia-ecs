using Theia.ECS.Entities;

namespace Theia.Tests.ECS.Entities;

public class EntityMetaTests
{
    [Fact]
    public void Equals_WithSameFields_ReturnsTrue()
    {
        EntityMeta entityMetaA = new();
        EntityMeta entityMetaB = new();

        Assert.True(entityMetaA.Equals(entityMetaB));
    }

    [Fact]
    public void Equals_WithDifferentFields_ReturnsFalse()
    {
        EntityMeta entityMetaA = new()
        {
            _version = 0,
            _archetype = null!,
            _storageIndex = 2,
            _componentIndex = 0,
        };
        EntityMeta entityMetaB = new()
        {
            _version = 0,
            _archetype = null!,
            _storageIndex = 3,
            _componentIndex = 0,
        };

        Assert.False(entityMetaA.Equals(entityMetaB));
    }

    [Fact]
    public void GetHashCode_WithSameFields_ShouldBeEqual()
    {
        EntityMeta entityMetaA = new()
        {
            _version = 1,
            _archetype = null!,
            _storageIndex = 3,
            _componentIndex = 1,
        };
        EntityMeta entityMetaB = new()
        {
            _version = 1,
            _archetype = null!,
            _storageIndex = 3,
            _componentIndex = 1,
        };

        Assert.Equal(entityMetaA.GetHashCode(), entityMetaB.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentFields_ShouldBeDifferent()
    {
        EntityMeta entityMetaA = new()
        {
            _version = 0,
            _archetype = null!,
            _storageIndex = 3,
            _componentIndex = 1,
        };
        EntityMeta entityMetaB = new()
        {
            _version = 1,
            _archetype = null!,
            _storageIndex = 3,
            _componentIndex = 1,
        };

        Assert.NotEqual(entityMetaA.GetHashCode(), entityMetaB.GetHashCode());
    }
}
