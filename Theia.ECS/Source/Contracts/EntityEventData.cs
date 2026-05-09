using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Assemblages;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Enums;
using Theia.ECS.Worlds;

namespace Theia.ECS.Contracts;

/// <summary>
/// Contract for event payloads that <b>describe an entity's current composition</b>. Provides
/// component-membership and assemblage-binding queries against the archetype the entity belongs
/// to at the moment the event fired.
/// </summary>
internal interface IEntitySet
{
    /// <summary>Returns <see langword="true"/> if the entity currently has <typeparamref name="TComponent"/>.</summary>
    public bool Has<TComponent>()
        where TComponent : struct;

    /// <summary>Returns <see langword="true"/> if the entity currently belongs to an archetype bound to <paramref name="assemblage"/>.</summary>
    public bool Is<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage;

    /// <summary>Returns <see langword="true"/> if the entity component composition currently satisfies to <paramref name="assemblage"/>.</summary>
    public bool Satisfies<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage;
}

/// <summary>
/// Contract for event payloads that describe a single component-set change, exposing a way to
/// classify the affected component type via a user-defined <see cref="Enum"/> mapped through
/// <see cref="StructEnum{TEnum, TTypeMeta}"/>.
/// </summary>
internal interface IComponentSetChanged
{
    /// <summary>
    /// Returns the <typeparamref name="TEnum"/> value that classifies the affected component
    /// type, as declared via <c>Includes&lt;T&gt;</c> or <c>Matches&lt;T&gt;</c> on the enum.
    /// Returns <see langword="default"/> when the component has no mapping.
    /// </summary>
    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum;
}

/// <summary>
/// Contract for event payloads that <b>describe an entity's previous composition</b>. Mirrors
/// <see cref="IEntitySet"/> but in past tense, for events fired after a transition where the
/// "before" state is what listeners care about.
/// </summary>
internal interface IEntityTransformed
{
    /// <summary>Returns <see langword="true"/> if the entity had <typeparamref name="TComponent"/> before the transition.</summary>
    public bool Had<TComponent>()
        where TComponent : struct;

    /// <summary>Returns <see langword="true"/> if the entity belonged to an archetype bound to <paramref name="assemblage"/> before the transition.</summary>
    public bool Was<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage;
}

/// <summary>
/// Event payload for <see cref="Events.EntitiesEvents.OnCreated">OnCreated</see>. Carries the freshly created
/// entity along with its assembled archetype so listeners can inspect the initial composition.
/// </summary>
public readonly ref struct EntityAssembled : IEntitySet
{
    /// <summary>The world the entity was created in.</summary>
    public readonly World World;

    /// <summary>The entity that was just created.</summary>
    public readonly Entity Entity;
    internal readonly Archetype _belongsTo;

    internal EntityAssembled(World world, Entity entity, in Archetype belongsTo)
    {
        World = world;
        Entity = entity;
        _belongsTo = belongsTo;
    }

    /// <inheritdoc/>
    public bool Has<TComponent>()
        where TComponent : struct => _belongsTo.Has<TComponent>();

    /// <inheritdoc/>
    public bool Is<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongsTo.GetMatchedAssemblage();

    /// <inheritdoc/>
    public bool Satisfies<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage =>
        assemblage._signature.IsSatisfiedBy(_belongsTo._signature);
}

/// <summary>
/// Event payload for <see cref="Events.EntitiesEvents.OnAnyComponentAdded">OnAnyComponentAdded</see> and the matching
/// removal events. Carries both archetypes, the one the entity belonged to before the change
/// and the one it belongs to after, so listeners can answer membership questions in either
/// direction without re-walking world state.
/// </summary>
/// <remarks>
/// The implemented contracts overlap deliberately: <see cref="IEntitySet"/> answers questions
/// about the post-change composition, <see cref="IEntityTransformed"/> answers them about the
/// pre-change composition, and <see cref="IComponentSetChanged"/> classifies the specific
/// component that changed.
/// </remarks>
public readonly ref struct EntityModified : IComponentSetChanged, IEntitySet, IEntityTransformed
{
    /// <summary>The world the modification occurred in.</summary>
    public readonly World World;

    /// <summary>The entity whose composition changed.</summary>
    public readonly Entity Entity;

    /// <summary>The runtime <see cref="System.Type"/> of the component that was added or removed.</summary>
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

    /// <inheritdoc/>
    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum => StructEnum<TEnum, ComponentType>.FromStruct(_componentId);

    /// <inheritdoc/>
    public bool Has<TComponent>()
        where TComponent : struct => _belongsTo.Has<TComponent>();

    /// <inheritdoc/>
    public bool Had<TComponent>()
        where TComponent : struct => _belongedTo.Has<TComponent>();

    /// <inheritdoc/>
    public bool Is<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongsTo.GetMatchedAssemblage();

    /// <inheritdoc/>
    public bool Satisfies<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage =>
        assemblage._signature.IsSatisfiedBy(_belongsTo._signature);

    /// <inheritdoc/>
    public bool Was<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongedTo.GetMatchedAssemblage();
}

/// <summary>
/// Event payload for <see cref="Events.EntitiesEvents.OnGhoulified">OnGhoulified</see>. Carries the now-recyclable
/// entity along with the archetype it belonged to before ghoulification, so listeners can inspect what
/// the entity <b>was</b> in their cleanup logic.
/// </summary>
/// <remarks>
/// By the time this event fires the entity's components and relations have already been cleaned
/// up; the only state still meaningfully readable through the payload is the past-tense
/// composition exposed via <see cref="IEntityTransformed"/>.
/// </remarks>
public readonly ref struct EntityGhoulified : IEntityTransformed
{
    /// <summary>The world the entity was destroyed in.</summary>
    public readonly World World;

    /// <summary>The entity whose slot is now eligible for reuse.</summary>
    public readonly Entity Entity;
    internal readonly Archetype _belongedTo;

    internal EntityGhoulified(World world, Entity entity, in Archetype belongedTo)
    {
        World = world;
        Entity = entity;
        _belongedTo = belongedTo;
    }

    /// <inheritdoc/>
    public bool Had<TComponent>()
        where TComponent : struct => _belongedTo.Has<TComponent>();

    /// <inheritdoc/>
    public bool Was<TAssemblage>(TAssemblage assemblage)
        where TAssemblage : Assemblage => assemblage == _belongedTo.GetMatchedAssemblage();
}
