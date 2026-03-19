using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Queries;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Queries;

public sealed class QueryTests
{
    [Fact]
    public void SettlerQuery_ArchetypeMatchesAssemblageArchetype()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        Assert.Same(assemblage._archetype, query._archetype);
    }

    [Fact]
    public void SettlerQuery_ComponentStorageMappingMatchesAssemblageMapping()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        ReadOnlySpan<int> queryMapping = query.GetComponentStorageMapping();
        ReadOnlySpan<int> assemblageMapping = assemblage.GetComponentStorageMapping();

        Assert.Equal(assemblageMapping.Length, queryMapping.Length);
        Assert.Equal(assemblageMapping[0], queryMapping[0]);
    }

    [Fact]
    public void NomadQuery_InitialMatchedArchetypeCountIsZero()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Assert.Equal(0, query.GetMatchedArchetypes().Length);
    }

    [Fact]
    public void NomadQuery_Add_IncrementsMatchedArchetypeCount()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Archetype archetype = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );

        query.Add(archetype);

        Assert.Equal(2, query.GetMatchedArchetypes().Length);
    }

    [Fact]
    public void NomadQuery_AddMultipleArchetypes_CountMatchesAdded()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Archetype archetypeA = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );

        Archetype archetypeB = world.FindOrCreateArchetype(
            stackalloc int[2] { ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id }
        );

        Assert.Equal(2, query.GetMatchedArchetypes().Length);
    }

    [Fact]
    public void NomadQuery_GetMatchedArchetypes_ReturnsAddedArchetypes()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Archetype archetypeA = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );
        Archetype archetypeB = world.FindOrCreateArchetype(
            stackalloc int[2] { ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id }
        );

        ReadOnlySpan<Archetype> matched = query.GetMatchedArchetypes();

        Assert.Equal(2, matched.Length);
        Assert.True(matched.Contains(archetypeA));
        Assert.True(matched.Contains(archetypeB));
    }

    [Fact]
    public void NomadQuery_SignatureReflectsRequestedComponents()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Archetype positionArchetype = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Position>.s_id }
        );

        Assert.True(query._signature.IsSatisfiedBy(positionArchetype._signature));
    }

    [Fact]
    public void NomadQuery_SignatureNotSatisfiedByUnrelatedArchetype()
    {
        World world = new();

        NomadQuery<Position> query = world.CreateNomadQuery<Position>();

        Archetype velocityOnly = world.FindOrCreateArchetype(
            stackalloc int[1] { ComponentMeta<Velocity>.s_id }
        );

        Assert.False(query._signature.IsSatisfiedBy(velocityOnly._signature));
        Assert.Equal(0, query.GetMatchedArchetypes().Length);
    }
}
