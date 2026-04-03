using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Events;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    public readonly EntityEvents EntityEvents;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnGhoulified(EntityGhoulified entityGhoulified)
    {
        EntityEvents.InvokeOnGhoulified(entityGhoulified);
        entityGhoulified
            ._belongedTo.GetMatchedAssemblage()
            ?.EntityEvents.InvokeOnGhoulified(entityGhoulified);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnComponentAdded(EntityModified entityModified)
    {
        EntityEvents.InvokeOnComponentAdded(entityModified);
        entityModified
            ._belongedTo.GetMatchedAssemblage()
            ?.EntityEvents.InvokeOnComponentAdded(entityModified);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnComponentRemoved(EntityModified entityModified)
    {
        EntityEvents.InvokeOnComponentRemoved(entityModified);
        entityModified
            ._belongedTo.GetMatchedAssemblage()
            ?.EntityEvents.InvokeOnComponentRemoved(entityModified);
    }
}
