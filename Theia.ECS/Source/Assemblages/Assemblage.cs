using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Events;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

/// <summary>
/// Non-generic base for an entity factory bound to a specific <see cref="Archetype"/>. Holds the
/// shared archetype binding, the per-assemblage events hub, and the deferred-create contract;
/// concrete typed factories derive from this.
/// </summary>
/// <remarks>
/// <para>
/// <b>An assemblage is the fast path for creating entities with a fixed component composition</b>.
/// Because the archetype is known up-front, creation skips the structural-change cost an entity
/// would otherwise pay walking the archetype graph component-by-component: <b>the entity is placed
/// directly into the bound archetype with its components populated in one pass</b>.
/// </para>
/// <para>
/// Each assemblage owns its own <see cref="EntityEvents"/> hub. Handlers attached here fire
/// only for entities created through this assemblage, in addition to the world-level events on
/// <see cref="World.EntitiesEvents"/>. This lets gameplay wire factory-specific behavior: on every
/// entity spawned with this set of component do that; without polluting the world-level
/// event bus or filtering by component composition at the handler.
/// </para>
/// <para>
/// Creations issued during systems and queries execution <b>must be deferred</b> and materialized by
/// <see cref="DeferredCreate"/> at the next safe point in the tick.
/// </para>
/// </remarks>
public abstract class Assemblage
{
    /// <summary>
    /// Per-assemblage events hub in addition to the matching handlers on <see cref="World.EntitiesEvents"/>.
    /// </summary>
    public readonly EntitiesEvents EntityEvents;

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
        EntityEvents = new();
        _world = world;
        _signature = archetype._signature;
        _archetype = archetype;
        _componentStorageMapping = componentStorageMapping.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<int> GetComponentStorageMapping() => _componentStorageMapping.AsSpan();

    /// <summary>
    /// Fires both the world-level and assemblage-level <c>OnCreated</c> events for
    /// <paramref name="entityAssembled"/>. Concrete derivatives call this once per entity
    /// during their flush in <see cref="DeferredCreate"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void InvokeOnEntityCreated(EntityAssembled entityAssembled)
    {
        _world.EntitiesEvents.InvokeOnCreated(entityAssembled);
        EntityEvents.InvokeOnCreated(entityAssembled);
    }

    /// <summary>
    /// Materializes every entity queued on this assemblage since the last flush, placing them
    /// into the bound <see cref="_archetype"/> and firing <see cref="EntitiesEvents.OnCreated">OnCreated</see>
    /// for each on both the world-level and assemblage-level hubs.
    /// </summary>
    internal abstract void DeferredCreate();
}
