using System;
using Theia.ECS.Reflection;

namespace Theia.ECS.Components;

internal abstract class ComponentType : TypeMeta
{
    internal readonly int _sizeOf;

    internal ComponentType(Type type, int sizeOf)
        : base(type) => _sizeOf = sizeOf;

    internal abstract Storage CreateStorage(int capacity);
}
