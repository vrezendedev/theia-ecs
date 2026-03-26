using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed partial class WorldUniqueTests
{
    [Fact]
    public void ReadUnique_DefaultsToDefault_WhenNeverSet()
    {
        World world = new();

        Assert.Equal(default, world.ReadUnique<ComponentA>());
    }

    [Fact]
    public void SetUnique_DefaultsToDefault_WhenNeverSet()
    {
        World world = new();

        world.SetUnique((ref ComponentA c) => Assert.Equal(default, c));
    }

    [Fact]
    public void SetUnique_MutationAffectsStoredValue()
    {
        World world = new();

        world.SetUnique((ref ComponentA c) => c.A = 99);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_ReadsCurrentValue()
    {
        World world = new();

        world.SetUnique((ref ComponentA c) => c.A = 42);

        world.SetUnique((ref ComponentA c) => Assert.Equal(42, c.A));
    }

    [Fact]
    public void SetUnique_OverwritesPreviousValue()
    {
        World world = new();

        world.ReadUnique<ComponentA>();
        world.SetUnique((ref ComponentA c) => c.A = 99);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_DoesNotAffectOtherTypes()
    {
        World world = new();

        world.SetUnique((ref ComponentA c) => c.A = 99);
        world.SetUnique((ref ComponentB c) => c.B = 2);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
        Assert.Equal(2, world.ReadUnique<ComponentB>().B);
    }
}
