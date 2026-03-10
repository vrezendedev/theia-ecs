using Theia.ECS.Archetypes;

namespace Theia.ECS.Components;

internal sealed class ComponentType<T> : ComponentType
    where T : struct
{
    internal ComponentType(int sizeOf)
        : base(sizeOf) { }

    internal override Storage CreateStorage(int capacity) => new Storage<T>(capacity);
}
