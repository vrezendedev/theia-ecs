using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

public abstract class Assemblage
{
    internal readonly World _world;
    internal readonly Signature _signature;
    internal readonly Archetype _archetype;
    internal int[] _componentStorageMapping;

    internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
    {
        _world = world;
        _signature = archetype._signature;
        _archetype = archetype;
        _componentStorageMapping = componentStorageMapping.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGhoulify(Entity entity) => _world.TryGhoulify(entity, _archetype);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeferredGhoulify(Entity entity) =>
        _world.Defer(() => _world.TryGhoulify(entity, _archetype));

    internal ReadOnlySpan<int> GetComponentStorageMapping() => _componentStorageMapping.AsSpan();
}
