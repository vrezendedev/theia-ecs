using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Enums;

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
    public readonly Entity Entity;
    internal readonly Archetype _belongsTo;

    internal EntityAssembled(Entity entity, in Archetype belongsTo)
    {
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
    public readonly Entity Entity;
    public readonly Type Type;
    internal readonly Archetype _belongedTo;
    internal readonly Archetype _belongsTo;
    internal readonly int _componentId;

    internal EntityModified(
        Entity entity,
        in Type type,
        in Archetype belongedTo,
        in Archetype belongsTo,
        int componentId
    )
    {
        Entity = entity;
        Type = type;
        _belongedTo = belongedTo;
        _belongsTo = belongsTo;
        _componentId = componentId;
    }

    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum => ComponentEnum<TEnum>.FromComponent(_componentId);

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
    public readonly Entity Entity;
    internal readonly Archetype _belongedTo;

    internal EntityGhoulified(Entity entity, in Archetype belongedTo)
    {
        Entity = entity;
        _belongedTo = belongedTo;
    }

    public bool Had<TComponent>()
        where TComponent : struct => _belongedTo.Has<TComponent>();

    public bool Was<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongedTo.GetMatchedAssemblage();
}
