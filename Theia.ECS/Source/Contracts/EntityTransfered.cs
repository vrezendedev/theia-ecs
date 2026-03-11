using System;

namespace Theia.ECS.Contracts;

internal readonly ref struct EntityTransferred : IEquatable<EntityTransferred>
{
    internal readonly EntityAccounted _entityAccounted;
    internal readonly EntitySwapped _entitySwapped;

    internal EntityTransferred(EntityAccounted entityAccounted, EntitySwapped entitySwapped)
    {
        _entityAccounted = entityAccounted;
        _entitySwapped = entitySwapped;
    }

    public bool Equals(EntityTransferred other) =>
        _entityAccounted.Equals(other._entityAccounted)
        && _entitySwapped.Equals(other._entitySwapped);
}
