using Theia.ECS.Archetypes;

namespace Theia.ECS.Components;

internal abstract class ComponentType
{
    internal readonly int _sizeOf;

    internal ComponentType(int sizeOf) => _sizeOf = sizeOf;

    internal abstract Storage CreateStorage(int capacity);
}
