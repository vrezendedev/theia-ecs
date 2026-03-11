using System;
using Theia.ECS.Archetypes;

namespace Theia.ECS.Components;

internal sealed class ComponentType<T> : ComponentType
    where T : struct
{
    internal ComponentType(Type type, int sizeOf)
        : base(type, sizeOf) { }

    internal override Storage CreateStorage(int capacity) => new Storage<T>(capacity);
}
