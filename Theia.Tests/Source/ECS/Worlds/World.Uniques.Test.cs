using Theia.ECS.Components;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

[Collection("MetaRequester")]
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

        ComponentAUpdate queryUnique = new() { A = 99 };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUnique);

        ref ComponentA c = ref world.GetUnique<ComponentA>();

        const int targetA = 12;

        c.A = targetA;

        Assert.Equal(targetA, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void GetUnique_ReadsCurrentValue()
    {
        World world = new();

        ComponentAUpdate queryUnique = new() { A = 42 };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUnique);

        Assert.Equal(42, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void UpdateUnique_DefaultsToDefault_WhenNeverSet()
    {
        World world = new();

        ComponentAUpdate queryUnique = new() { };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUnique);

        Assert.Equal(default, queryUnique.ComponentA);
    }

    [Fact]
    public void UpdateUnique_MutationAffectsStoredValue()
    {
        World world = new();

        ComponentAUpdate queryUnique = new() { A = 99 };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUnique);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void UpdateUnique_ReadsCurrentValue()
    {
        World world = new();

        ComponentAUpdate queryUnique1 = new() { A = 42 };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUnique1);

        ComponentAUpdate queryUnique2 = new() { };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUnique2);

        Assert.Equal(42, queryUnique2.ComponentA.A);
    }

    [Fact]
    public void UpdateUnique_OverwritesPreviousValue()
    {
        World world = new();

        ref ComponentA a = ref world.GetUnique<ComponentA>();

        a.A += 10;

        ComponentAUpdate queryUnique = new() { A = 99 };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUnique);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
    }

    [Fact]
    public void UpdateUnique_DoesNotAffectOtherTypes()
    {
        World world = new();

        ComponentAUpdate queryUniqueA = new() { A = 99 };

        world.QueryUnique<ComponentA, ComponentAUpdate>(ref queryUniqueA);

        ComponentBUpdate queryUniqueB = new() { B = 2 };

        world.QueryUnique<ComponentB, ComponentBUpdate>(ref queryUniqueB);

        Assert.Equal(99, world.ReadUnique<ComponentA>().A);
        Assert.Equal(2, world.ReadUnique<ComponentB>().B);
    }
}

file ref struct ComponentAUpdate : IUniqueQuery<ComponentA>
{
    public int A;
    public ComponentA ComponentA;

    public void Execute(ref ComponentA component)
    {
        if (A > 0)
        {
            component.A = A;
        }

        ComponentA = component;
    }
}

file ref struct ComponentBUpdate : IUniqueQuery<ComponentB>
{
    public int B;

    public void Execute(ref ComponentB component)
    {
        component.B = B;
    }
}
