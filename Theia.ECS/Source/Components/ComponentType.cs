using System;
using Theia.ECS.Reflection;

namespace Theia.ECS.Components;

internal abstract class ComponentType : ITypeMeta
{
    internal readonly Type _type;
    internal readonly int _sizeOf;

    internal ComponentType(Type type, int sizeOf)
        : base()
    {
        _type = type;
        _sizeOf = sizeOf;
    }

    public Type Get() => _type;

    public static int Count() => ComponentsMeta.Count();

    public static int GetId(Type type) => ComponentsMeta.GetComponentId(type);

    internal abstract Storage CreateStorage(int capacity);
}
