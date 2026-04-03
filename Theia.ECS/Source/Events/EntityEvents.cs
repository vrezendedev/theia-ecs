using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;

namespace Theia.ECS.Events;

public sealed class EntityEvents
{
    public event Action<EntityAssembled>? OnCreated;
    public event Action<EntityGhoulified>? OnGhoulified;
    public event Action<EntityModified>? OnComponentAdded;
    public event Action<EntityModified>? OnComponentRemoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnCreated(EntityAssembled entityAssembled) =>
        OnCreated?.Invoke(entityAssembled);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnGhoulified(EntityGhoulified entityGhoulified) =>
        OnGhoulified?.Invoke(entityGhoulified);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentAdded(EntityModified entityModified) =>
        OnComponentAdded?.Invoke(entityModified);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentRemoved(EntityModified entityModified) =>
        OnComponentRemoved?.Invoke(entityModified);

    public void Reset()
    {
        OnCreated = null;
        OnGhoulified = null;
        OnComponentAdded = null;
        OnComponentRemoved = null;
    }
}
