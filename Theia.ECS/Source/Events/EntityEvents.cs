using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;

namespace Theia.ECS.Events;

public sealed class EntityEvents
{
    public event Action<EntityAssembled>? OnEntityCreated;
    public event Action<EntityGhoulified>? OnEntityGhoulified;
    public event Action<EntityModified>? OnComponentAdded;
    public event Action<EntityModified>? OnComponentRemoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnEntityCreated(EntityAssembled entityAssembled) =>
        OnEntityCreated?.Invoke(entityAssembled);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnEntityGhoulified(EntityGhoulified entityGhoulified) =>
        OnEntityGhoulified?.Invoke(entityGhoulified);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentAdded(EntityModified entityModified) =>
        OnComponentAdded?.Invoke(entityModified);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnComponentRemoved(EntityModified entityModified) =>
        OnComponentRemoved?.Invoke(entityModified);

    public void Reset()
    {
        OnEntityCreated = null;
        OnEntityGhoulified = null;
        OnComponentAdded = null;
        OnComponentRemoved = null;
    }
}
