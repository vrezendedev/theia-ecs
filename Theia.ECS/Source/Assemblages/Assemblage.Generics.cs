using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

public class Assemblage<T> : Assemblage
    where T : struct
{
    internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
        : base(world, archetype, componentStorageMapping) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity Create(in T component)
    {
        EntityCreated entityCreated = _world.CreateEntity(_archetype);

        _archetype.Set(_componentStorageMapping[0], in entityCreated._entityMeta, in component);

        return entityCreated._entity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeferredCreate(T component) => _world.Defer(() => Create(component));
}
