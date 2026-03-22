using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Events;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    public readonly EntityEvents Events;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnEntityGhoulified(EntityGhoulified entityGhoulified)
    {
        Events.InvokeOnEntityGhoulified(entityGhoulified);
        entityGhoulified
            ._belongedTo.GetMatchedAssemblage()
            ?.Events.InvokeOnEntityGhoulified(entityGhoulified);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnComponentAdded(EntityModified entityModified)
    {
        Events.InvokeOnComponentAdded(entityModified);
        entityModified
            ._belongedTo.GetMatchedAssemblage()
            ?.Events.InvokeOnComponentAdded(entityModified);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InvokeOnComponentRemoved(EntityModified entityModified)
    {
        Events.InvokeOnComponentRemoved(entityModified);
        entityModified
            ._belongedTo.GetMatchedAssemblage()
            ?.Events.InvokeOnComponentRemoved(entityModified);
    }
}
