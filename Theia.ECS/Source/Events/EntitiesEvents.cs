using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Components;
using Theia.ECS.Contracts;

namespace Theia.ECS.Events;

/// <summary>
/// Per-<see cref="Worlds.World">World</see> hub for entity-lifecycle and component-mutation events.
/// Exposes both broad subscriptions ("any component added to any entity") and per-component-type
/// subscriptions, so listeners can choose the granularity that matches their cost tolerance.
/// </summary>
/// <remarks>
/// <para>
/// Invoking the event is a single bounds check plus a null check on the bucket, so the cost of subscribing
/// to one component does not compound across all others.
/// </para>
/// <para>
/// <b>Events are attachment points for fixed behavior, not temporary subscriptions.</b> Wire
/// handlers once at world construction to declare "this is what happens when X occurs"; the
/// only teardown mechanism is <see cref="Reset"/>, which clears the entire wiring at once.
/// </para>
/// <para>
/// Events fire after the underlying mutation has been applied to world state. <b>Subscribers run
/// synchronously on the calling thread</b>.
/// </para>
/// </remarks>
public sealed class EntitiesEvents
{
    /// <summary>Fires when an entity is <b>created via the <see cref="Assemblages.Assemblage">Assemblage</see> path</b>, with its initial component composition already in place.</summary>
    public event Action<EntityAssembled>? OnCreated;

    /// <summary>Fires when <b>any component is added to any entity</b>. <br/>Use the typed <see cref="SubscribeOnComponentAdded"/> for per-component-type filtering instead.</summary>
    public event Action<EntityModified>? OnAnyComponentAdded;

    /// <summary>
    /// Fires when <b>any component is removed from any entity</b>, except when the removal would
    /// leave the entity with no components: in that case the entity is ghoulified instead, and
    /// <see cref="OnGhoulified"/> fires in this event's place.
    /// <br/>
    /// Use the typed <see cref="SubscribeOnComponentRemoved"/> for per-component-type filtering.
    /// </summary>
    public event Action<EntityModified>? OnAnyComponentRemoved;

    /// <summary>
    /// Fires when an entity has been destroyed and its slot is now eligible for reuse. <b>The
    /// entity's components and relations have already been cleaned up</b>; this event marks the
    /// transition into the recyclable state, not the start of teardown.
    /// </summary>
    public event Action<EntityGhoulified>? OnGhoulified;
    private ComponentEvents[] _componentEvents = Array.Empty<ComponentEvents>();

    /// <summary>
    /// Subscribes <paramref name="onComponentAdded"/> to additions of <typeparamref name="TComponent"/>
    /// specifically, regardless of which entity is affected.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubscribeOnComponentAdded<TComponent>(Action<EntityModified> onComponentAdded)
        where TComponent : struct =>
        GetOrCreateComponentEvents<TComponent>()._onComponentAdded += onComponentAdded;

    /// <summary>
    /// Subscribes <paramref name="onComponentRemoved"/> to removals of <typeparamref name="TComponent"/>
    /// specifically, regardless of which entity is affected.
    /// </summary>
    /// <remarks>
    /// Does not fire when the removal of <typeparamref name="TComponent"/> would leave the entity
    /// with no components: in that case the entity is ghoulified instead, and
    /// <see cref="OnGhoulified"/> fires in this subscription's place.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubscribeOnComponentRemoved<TComponent>(Action<EntityModified> onComponentRemoved)
        where TComponent : struct =>
        GetOrCreateComponentEvents<TComponent>()._onComponentRemoved += onComponentRemoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnCreated(EntityAssembled entityAssembled) =>
        OnCreated?.Invoke(entityAssembled);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnGhoulified(EntityGhoulified entityGhoulified) =>
        OnGhoulified?.Invoke(entityGhoulified);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentAdded(EntityModified entityModified)
    {
        OnAnyComponentAdded?.Invoke(entityModified);

        if (HasEvents(entityModified._componentId))
            _componentEvents[entityModified._componentId].InvokeOnComponentAdded(entityModified);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentRemoved(EntityModified entityModified)
    {
        OnAnyComponentRemoved?.Invoke(entityModified);

        if (HasEvents(entityModified._componentId))
            _componentEvents[entityModified._componentId].InvokeOnComponentRemoved(entityModified);
    }

    private ComponentEvents GetOrCreateComponentEvents<TComponent>()
        where TComponent : struct
    {
        int componentId = ComponentMeta<TComponent>.s_id;

        if (componentId >= _componentEvents.Length)
            Array.Resize(ref _componentEvents, componentId + 1);

        if (_componentEvents[componentId] is null)
            _componentEvents[componentId] = new ComponentEvents();

        return _componentEvents[componentId];
    }

    private bool HasEvents(int componentId) =>
        componentId < _componentEvents.Length && _componentEvents[componentId] is not null;

    /// <summary>
    /// Detaches every subscriber from every event on this hub, including all per-component
    /// buckets.
    /// </summary>
    public void Reset()
    {
        OnCreated = null;
        OnAnyComponentAdded = null;
        OnAnyComponentRemoved = null;
        OnGhoulified = null;

        foreach (ComponentEvents componentEvents in _componentEvents)
            componentEvents?.Reset();
    }
}

/// <summary>
/// Per-component-type bucket holding the typed add and remove subscriptions. Created lazily by
/// <see cref="EntitiesEvents"/> on first subscription for that component.
/// </summary>
internal sealed class ComponentEvents
{
    internal event Action<EntityModified>? _onComponentAdded;
    internal event Action<EntityModified>? _onComponentRemoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentAdded(EntityModified entityModified) =>
        _onComponentAdded?.Invoke(entityModified);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentRemoved(EntityModified entityModified) =>
        _onComponentRemoved?.Invoke(entityModified);

    internal void Reset()
    {
        _onComponentAdded = null;
        _onComponentRemoved = null;
    }
}
