using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

public sealed class WorldArchetypeTests
{
    [Fact]
    public void FindOrCreateArchetype_NewSignature_CreatesArchetype()
    {
        World world = new();

        Archetype archetype = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );

        Assert.NotNull(archetype);
    }

    [Fact]
    public void FindOrCreateArchetype_SameSignatureCalledTwice_ReturnsSameArchetype()
    {
        World world = new();

        Archetype first = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );
        Archetype second = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );

        Assert.Same(first, second);
    }

    [Fact]
    public void FindOrCreateArchetype_DifferentSignatures_ReturnDifferentArchetypes()
    {
        World world = new();

        Archetype positionArchetype = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );
        Archetype velocityArchetype = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Velocity>.s_id }
        );

        Assert.NotSame(positionArchetype, velocityArchetype);
    }

    [Fact]
    public void FindOrCreateArchetype_MultiComponentSignature_ArchetypeHasAllComponents()
    {
        World world = new();

        Archetype archetype = world.FindOrCreateArchetype(
            stackalloc int[2] { ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id }
        );

        Assert.True(archetype.Has<Position>());
        Assert.True(archetype.Has<Velocity>());
    }

    [Fact]
    public void GetArchetype_ReturnsCorrectArchetype()
    {
        World world = new();

        Archetype created = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );

        Archetype retrieved = world.GetArchetype(created._archetypeId);

        Assert.Same(created, retrieved);
    }
}
