using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed partial class WorldUniqueTests
{
    [Fact]
    public void GetUnique_ReturnsRef_MutationAffectsStoredValue()
    {
        World world = new();

        ref ComponentA unique = ref world.GetUnique<ComponentA>();
        unique.A = 99;

        Assert.Equal(99, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void GetUnique_DefaultsToDefault_WhenNeverSet()
    {
        World world = new();

        Assert.Equal(default, world.GetUnique<ComponentA>());
    }

    [Fact]
    public void GetUnique_MultipleTypes_AreIndependent()
    {
        World world = new();

        world.SetUnique(new ComponentA { A = 1 });
        world.SetUnique(new ComponentB { B = 2 });
        world.SetUnique(new ComponentC { C = 3 });

        Assert.Equal(1, world.GetUnique<ComponentA>().A);
        Assert.Equal(2, world.GetUnique<ComponentB>().B);
        Assert.Equal(3, world.GetUnique<ComponentC>().C);
    }

    [Fact]
    public void ReadUnique_ReturnsValue_WhenSet()
    {
        World world = new();

        world.SetUnique(new ComponentA { A = 42 });

        Assert.Equal(42, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void ReadUnique_ReturnsCopy_MutationDoesNotAffectStoredValue()
    {
        World world = new();

        world.SetUnique(new ComponentA { A = 42 });

        ComponentA copy = world.ReadUnique<ComponentA>();
        copy.A = 99;

        Assert.Equal(42, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void ReadUnique_DefaultsToDefault_WhenNeverSet()
    {
        World world = new();

        Assert.Equal(default, world.ReadUnique<ComponentA>());
    }

    [Fact]
    public void SetUnique_SetsValue()
    {
        World world = new();

        world.SetUnique(new ComponentA { A = 42 });

        Assert.Equal(42, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_OverwritesPreviousValue()
    {
        World world = new();

        world.SetUnique(new ComponentA { A = 1 });
        world.SetUnique(new ComponentA { A = 99 });

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_DoesNotAffectOtherTypes()
    {
        World world = new();

        world.SetUnique(new ComponentA { A = 1 });
        world.SetUnique(new ComponentB { B = 2 });

        world.SetUnique(new ComponentA { A = 99 });

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
        Assert.Equal(2, world.ReadUnique<ComponentB>().B);
    }
}
