using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Events;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    /// <summary>
    /// Per-world hub for entity-lifecycle and component-mutation events. Handlers attached here
    /// fire for every entity in the world; for assemblage-scoped events, see the per-assemblage
    /// <see cref="Assemblages.Assemblage.EntityEvents"/>.
    /// </summary>
    public readonly EntitiesEvents EntitiesEvents;

    /// <summary>Per-world hub for relation-lifecycle events. Handlers fire for every relation add or remove in the world.</summary>
    public readonly RelationsEvents RelationsEvents;

    /// <summary>
    /// Fires <c>OnGhoulified</c> on the world hub, then on the matched assemblage's hub if the
    /// entity's <see cref="EntityGhoulified._belongedTo"/> archetype had one. The assemblage
    /// receives the event because the <b>entity's final composition matched its archetype</b>:
    /// "an instance of <i>this</i> assemblage was just destroyed."
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnGhoulified(EntityGhoulified entityGhoulified)
    {
        EntitiesEvents.InvokeOnGhoulified(entityGhoulified);
        entityGhoulified
            ._belongedTo.GetMatchedAssemblage()
            ?.EntityEvents.InvokeOnGhoulified(entityGhoulified);
    }

    /// <summary>
    /// Fires <c>OnComponentAdded</c> on the world hub, then on the matched assemblage's hub if
    /// the entity's previous archetype had one. The assemblage receives the event because <b>the
    /// entity <i>was</i> an instance of this assemblage's composition immediately before the
    /// addition transitioned it out</b>: "an instance of <i>this</i> assemblage just grew
    /// beyond its composition."
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnComponentAdded(EntityModified entityModified)
    {
        EntitiesEvents.InvokeOnComponentAdded(entityModified);
        entityModified
            ._belongedTo.GetMatchedAssemblage()
            ?.EntityEvents.InvokeOnComponentAdded(entityModified);
    }

    /// <summary>
    /// Fires <c>OnComponentRemoved</c> on the world hub, then on the matched assemblage's hub
    /// if the entity's previous archetype had one. The assemblage receives the event because
    /// <b>the entity <i>was</i> an instance of this assemblage's composition immediately before
    /// the removal transitioned it out</b>: "an instance of <i>this</i> assemblage just
    /// lost part of its composition."
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnComponentRemoved(EntityModified entityModified)
    {
        EntitiesEvents.InvokeOnComponentRemoved(entityModified);
        entityModified
            ._belongedTo.GetMatchedAssemblage()
            ?.EntityEvents.InvokeOnComponentRemoved(entityModified);
    }
}
