using System;
using Theia.ECS.Reflection;

namespace Theia.ECS.Components;

/// <summary>
/// Non-generic base for component type metadata.
/// <br/>
/// Holds the runtime <see cref="Type"/>, a cached name, the unmanaged size of the component,
/// and exposes factories for <see cref="Storage"/>.
/// </summary>
internal abstract class ComponentType : ITypeMeta
{
    internal Type _type;
    internal string? _name;
    internal int _sizeOf;

    /// <summary>
    /// Parameterless constructor used by <see cref="Activator.CreateInstance(Type)"/> on the <see cref="ComponentsMeta.AttemptRegisterComponent(string)"/>
    /// reflection path, where fields are populated immediately afterwards via <see cref="Initialize"/>.
    /// </summary>
#pragma warning disable CS8618
    public ComponentType() { }
#pragma warning restore CS8618

    internal ComponentType(Type type, int sizeOf)
        : base()
    {
        _type = type;
        _sizeOf = sizeOf;
    }

    /// <summary>
    /// Populates the type and size fields. Used by <see cref="ComponentsMeta.AttemptRegisterComponent(string)"/>.
    /// </summary>
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

    /// <summary>
    /// Creates a <see cref="Storage"/> block sized per-archetype to hold up to
    /// <paramref name="capacity"/> instances of this component type contiguously.
    /// </summary>
    internal abstract Storage CreateStorage(int capacity);

    /// <summary>
    /// Creates a <see cref="Unique"/> holder for this component type.
    /// </summary>
    internal abstract Unique CreateUnique();
}
