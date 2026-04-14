using System;
using Theia.ECS.Assemblages;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldConstructorTests
{
    [Fact]
    public void Constructor_WithDefaultCapacity_DoesNotThrow() => Assert.NotNull(new World());

    [Fact]
    public void Constructor_WithValidCapacity_DoesNotThrow() => Assert.NotNull(new World(1024));

    [Fact]
    public void Constructor_WithZeroCapacity_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new World(0));

    [Fact]
    public void Constructor_WithNegativeCapacity_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new World(-1));

    [Fact]
    public void Constructor_WithCapacityExceedingMax_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new World((1 << 30) + 1));

    [Fact]
    public void Constructor_WithNonPowerOfTwoCapacity_RoundsUpToPowerOfTwo()
    {
        World world = new(1000);

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        for (int i = 0; i < 1024; i++)
            assemblage.Create(new Position { X = i });
    }

    [Fact]
    public void Constructor_MultipleWorlds_WorldIdsAreDistinct()
    {
        World worldA = new();
        World worldB = new();

        Assert.NotEqual(worldA._worldId, worldB._worldId);
    }

    [Fact]
    public void Constructor_EntityCountStartsAtZero() =>
        Assert.Equal(0, new World().CountEntities());

    [Fact]
    public void Constructor_GhoulCountStartsAtZero() => Assert.Equal(0, new World().CountGhouls());
}
