using System;
using Theia.ECS.Contracts;

namespace Theia.ECS.Events;

public sealed class EntityEvents
{
    public event Action<EntityAssembled>? OnEntityCreated;
    public event Action<EntityGhoulified>? OnEntityGhoulified;
    public event Action<EntityModified>? OnComponentAdded;
    public event Action<EntityModified>? OnComponentRemoved;

    public void Reset()
    {
        OnEntityCreated = null;
        OnEntityGhoulified = null;
        OnComponentAdded = null;
        OnComponentRemoved = null;
    }
}
