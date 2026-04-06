using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Components;
using Theia.ECS.Contracts;

namespace Theia.ECS.Events;

public sealed class EntitiesEvents
{
    public event Action<EntityAssembled>? OnCreated;
    public event Action<EntityModified>? OnAnyComponentAdded;
    public event Action<EntityModified>? OnAnyComponentRemoved;
    public event Action<EntityGhoulified>? OnGhoulified;
    private ComponentEvents[] _componentEvents = Array.Empty<ComponentEvents>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubscribeOnComponentAdded<TComponent>(Action<EntityModified> onComponentAdded)
        where TComponent : struct =>
        GetOrCreateComponentEvents<TComponent>()._onComponentAdded += onComponentAdded;

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
