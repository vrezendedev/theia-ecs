using System;

namespace Theia.ECS.Components;

internal sealed class ComponentType<TComponent> : ComponentType
    where TComponent : struct
{
    internal ComponentType(Type type, int sizeOf)
        : base(type, sizeOf) { }

    internal override Storage CreateStorage(int capacity) => new Storage<TComponent>(capacity);
}
