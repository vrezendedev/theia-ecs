using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Events;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

public abstract class Assemblage
{
    public readonly EntityEvents Events;

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

    internal abstract void DeferredCreate();
}
