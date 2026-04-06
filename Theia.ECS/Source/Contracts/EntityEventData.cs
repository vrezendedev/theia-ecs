using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Enums;
using Theia.ECS.Worlds;

namespace Theia.ECS.Contracts;

internal interface IEntitySet
{
    public bool Has<TComponent>()
        where TComponent : struct;
    public bool Is<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage;
}

internal interface IComponentSetChanged
{
    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum;
}

internal interface IEntityTransformed
{
    public bool Had<TComponent>()
        where TComponent : struct;
    public bool Was<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage;
}

public readonly ref struct EntityAssembled : IEntitySet
{
    public readonly World World;
    public readonly Entity Entity;
    internal readonly Archetype _belongsTo;

    internal EntityAssembled(World world, Entity entity, in Archetype belongsTo)
    {
        World = world;
        Entity = entity;
        _belongsTo = belongsTo;
    }

    public bool Has<TComponent>()
        where TComponent : struct => _belongsTo.Has<TComponent>();

    public bool Is<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongsTo.GetMatchedAssemblage();
}

public readonly ref struct EntityModified : IComponentSetChanged, IEntitySet, IEntityTransformed
{
    public readonly World World;
    public readonly Entity Entity;
    public readonly Type Type;
    internal readonly Archetype _belongedTo;
    internal readonly Archetype _belongsTo;
    internal readonly int _componentId;

    internal EntityModified(
        World world,
        Entity entity,
        in Type type,
        in Archetype belongedTo,
        in Archetype belongsTo,
        int componentId
    )
    {
        World = world;
        Entity = entity;
        Type = type;
        _belongedTo = belongedTo;
        _belongsTo = belongsTo;
        _componentId = componentId;
    }

    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum => StructEnum<TEnum, ComponentType>.FromStruct(_componentId);

    public bool Has<TComponent>()
        where TComponent : struct => _belongsTo.Has<TComponent>();

    public bool Had<TComponent>()
        where TComponent : struct => _belongedTo.Has<TComponent>();

    public bool Is<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongsTo.GetMatchedAssemblage();

    public bool Was<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongedTo.GetMatchedAssemblage();
}

public readonly ref struct EntityGhoulified : IEntityTransformed
{
    public readonly World World;
    public readonly Entity Entity;
    internal readonly Archetype _belongedTo;

    internal EntityGhoulified(World world, Entity entity, in Archetype belongedTo)
    {
        World = world;
        Entity = entity;
        _belongedTo = belongedTo;
    }

    public bool Had<TComponent>()
        where TComponent : struct => _belongedTo.Has<TComponent>();

    public bool Was<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongedTo.GetMatchedAssemblage();
}
