using System;
using Theia.ECS.Reflection;

namespace Theia.ECS.Components;

internal abstract class ComponentType : ITypeMeta
{
    internal Type _type;
    internal string? _name;
    internal int _sizeOf;

#pragma warning disable CS8618
    public ComponentType() { }
#pragma warning restore CS8618

    internal ComponentType(Type type, int sizeOf)
        : base()
    {
        _type = type;
        _sizeOf = sizeOf;
    }

    public void Initialize(Type type, int sizeOf)
    {
        _type = type;
        _sizeOf = sizeOf;
    }

    public Type Get() => _type;

    public void SetTypeName(string name) => _name = name;

    public string GetTypeName() => _name!;

    public static int Count() => ComponentsMeta.Count();

    public static int GetId(Type type) => ComponentsMeta.GetComponentId(type);

    internal abstract Storage CreateStorage(int capacity);

    internal abstract Unique CreateUnique();
}
