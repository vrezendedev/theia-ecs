namespace Theia.Tests.ECS.Entities;

public class EntityMetaTest
{
    [Fact]
    public void Equals_WithSameFields_ReturnsTrue()
    {
        Theia.ECS.Entities.EntityMeta entityMetaA = new();
        Theia.ECS.Entities.EntityMeta entityMetaB = new();

        Assert.True(entityMetaA.Equals(entityMetaB));
    }

    [Fact]
    public void Equals_WithDifferentFields_ReturnsFalse()
    {
        Theia.ECS.Entities.EntityMeta entityMetaA = new()
        {
            _version = 0,
            _archetypeIndex = 1,
            _componentStorageIndex = 2,
            _componentIndex = 0,
        };
        Theia.ECS.Entities.EntityMeta entityMetaB = new()
        {
            _version = 0,
            _archetypeIndex = 1,
            _componentStorageIndex = 3,
            _componentIndex = 0,
        };

        Assert.False(entityMetaA.Equals(entityMetaB));
    }

    [Fact]
    public void GetHashCode_WithSameFields_ShouldBeEqual()
    {
        Theia.ECS.Entities.EntityMeta entityMetaA = new()
        {
            _version = 1,
            _archetypeIndex = 2,
            _componentStorageIndex = 3,
            _componentIndex = 1,
        };
        Theia.ECS.Entities.EntityMeta entityMetaB = new()
        {
            _version = 1,
            _archetypeIndex = 2,
            _componentStorageIndex = 3,
            _componentIndex = 1,
        };

        Assert.Equal(entityMetaA.GetHashCode(), entityMetaB.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentFields_ShouldBeDifferent()
    {
        Theia.ECS.Entities.EntityMeta entityMetaA = new()
        {
            _version = 0,
            _archetypeIndex = 2,
            _componentStorageIndex = 3,
            _componentIndex = 1,
        };
        Theia.ECS.Entities.EntityMeta entityMetaB = new()
        {
            _version = 1,
            _archetypeIndex = 2,
            _componentStorageIndex = 3,
            _componentIndex = 1,
        };

        Assert.NotEqual(entityMetaA.GetHashCode(), entityMetaB.GetHashCode());
    }
}
