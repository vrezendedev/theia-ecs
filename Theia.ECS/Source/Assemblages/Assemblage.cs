using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Events;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

public abstract class Assemblage
{
    public readonly EntityEvents Events;

    protected readonly Lock _deferredCreateLock = new();

    internal readonly World _world;
    internal readonly Signature _signature;
    internal readonly Archetype _archetype;
    internal readonly int[] _componentStorageMapping;

    internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
    {
        Events = new();
        _world = world;
        _signature = archetype._signature;
        _archetype = archetype;
        _componentStorageMapping = componentStorageMapping.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> GetComponentStorageMapping() => _componentStorageMapping.AsSpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void InvokeOnEntityCreated(EntityAssembled entityAssembled)
    {
        _world.Events.InvokeOnEntityCreated(entityAssembled);
        Events.InvokeOnEntityCreated(entityAssembled);
    }

    internal abstract void DeferredCreate();
}
