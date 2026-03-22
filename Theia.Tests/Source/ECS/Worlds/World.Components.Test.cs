using System;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldComponentsTests
{
    [Fact]
    public void RegisterUnique_ValidComponent_DoesNotThrow()
    {
        World world = new();

        world.RegisterUnique<ComponentA>();
    }

    [Fact]
    public void RegisterUnique_ReturnsWorld_ForFluentChaining()
    {
        World world = new();

        World returned = world.RegisterUnique<ComponentA>();

        Assert.Same(world, returned);
    }

    [Fact]
    public void RegisterUnique_FluentChaining_RegistersMultipleUniques()
    {
        World world = new();

        world
            .RegisterUnique(new ComponentA() { A = 1 })
            .RegisterUnique(new ComponentB() { B = 1 })
            .RegisterUnique(new ComponentC() { C = 1 });

        Assert.Equal(1, world.GetUnique<ComponentA>().A);
        Assert.Equal(1, world.GetUnique<ComponentB>().B);
        Assert.Equal(1, world.GetUnique<ComponentC>().C);
    }

    [Fact]
    public void RegisterUnique_WithInitialValue_SetsValue()
    {
        World world = new();

        world.RegisterUnique(new ComponentA { A = 42 });

        Assert.Equal(42, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void RegisterUnique_WithoutInitialValue_DefaultsToDefault()
    {
        World world = new();

        world.RegisterUnique<ComponentA>();

        Assert.Equal(default, world.GetUnique<ComponentA>());
    }

    [Fact]
    public void RegisterUnique_AlreadyRegistered_ThrowsInvalidOperationException()
    {
        World world = new();

        world.RegisterUnique<ComponentA>();

        Assert.Throws<InvalidOperationException>(() => world.RegisterUnique<ComponentA>());
    }

    [Fact]
    public void GetUnique_RegisteredComponent_ReturnsValue()
    {
        World world = new();

        world.RegisterUnique(new ComponentA { A = 42 });

        Assert.Equal(42, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void GetUnique_UnregisteredComponent_ThrowsInvalidOperationException()
    {
        World world = new();

        Assert.Throws<InvalidOperationException>(() => world.GetUnique<ComponentA>());
    }

    [Fact]
    public void GetUnique_ReturnsRef_MutationAffectsStoredValue()
    {
        World world = new();

        world.RegisterUnique<ComponentA>();

        ref ComponentA unique = ref world.GetUnique<ComponentA>();

        unique.A = 99;

        Assert.Equal(99, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_RegisteredComponent_SetsValue()
    {
        World world = new();

        world.RegisterUnique<ComponentA>();

        world.SetUnique(new ComponentA { A = 42 });

        Assert.Equal(42, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_OverwritesPreviousValue()
    {
        World world = new();

        world.RegisterUnique(new ComponentA { A = 1 });

        world.SetUnique(new ComponentA { A = 99 });

        Assert.Equal(99, world.GetUnique<ComponentA>().A);
    }

    [Fact]
    public void SetUnique_UnregisteredComponent_ThrowsInvalidOperationException()
    {
        World world = new();

        Assert.Throws<InvalidOperationException>(() => world.SetUnique(new ComponentA { A = 42 }));
    }
}
