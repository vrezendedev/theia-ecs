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
    public void GetUnique_DefaultsToDefault_WhenNeverSet()
    {
        World world = new();

        Assert.Equal(default, world.GetUnique<ComponentA>());
    }

    [Fact]
    public void GetUnique_MutationAffectsStoredValue()
    {
        World world = new();

        ref ComponentA c = ref world.GetUnique<ComponentA>();

        const int targetA = 10;
        c.A = targetA;

        Assert.Equal(targetA, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void GetUnique_OverwritesPreviousValue()
    {
        World world = new();

        world.UpdateUnique(static (ref ComponentA c) => c.A = 99);
        ref ComponentA c = ref world.GetUnique<ComponentA>();

        const int targetA = 12;
        c.A = targetA;

        Assert.Equal(targetA, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void GetUnique_ReadsCurrentValue()
    {
        World world = new();

        world.UpdateUnique(static (ref ComponentA c) => c.A = 42);

        Assert.Equal(42, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_DefaultsToDefault_WhenNeverSet()
    {
        World world = new();

        world.UpdateUnique(static (ref ComponentA c) => Assert.Equal(default, c));
    }

    [Fact]
    public void SetUnique_MutationAffectsStoredValue()
    {
        World world = new();

        world.UpdateUnique(static (ref ComponentA c) => c.A = 99);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_ReadsCurrentValue()
    {
        World world = new();

        world.UpdateUnique(static (ref ComponentA c) => c.A = 42);

        world.UpdateUnique((ref ComponentA c) => Assert.Equal(42, c.A));
    }

    [Fact]
    public void SetUnique_OverwritesPreviousValue()
    {
        World world = new();

        world.ReadUnique<ComponentA>();
        world.UpdateUnique(static (ref ComponentA c) => c.A = 99);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_DoesNotAffectOtherTypes()
    {
        World world = new();

        world.UpdateUnique(static (ref ComponentA c) => c.A = 99);
        world.UpdateUnique(static (ref ComponentB c) => c.B = 2);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
        Assert.Equal(2, world.ReadUnique<ComponentB>().B);
    }
}
