using System;

namespace Theia.ECS.Components;

internal abstract class ComponentType
{
    internal readonly Type _type;
    internal readonly int _sizeOf;

    internal ComponentType(Type type, int sizeOf)
    {
        _type = type;
        _sizeOf = sizeOf;
    }

    internal abstract Storage CreateStorage(int capacity);
}
