using System;

namespace Theia.ECS.Reflection;

/// <summary>
/// Common contract implemented by metadata classes that participate in a <see cref="TypeRegistry{T}"/>;
/// currently <see cref="Components.ComponentType"/> and <see cref="Relations.RelationType"/>.
/// <br/>
/// Static abstract members route count and ID-resolution queries to whichever concrete registry
/// the implementing type is paired with, so that callers can ask <c>TMeta.Count()</c> or
/// <c>TMeta.GetId(type)</c> generically without knowing which registry actually backs it.
/// </summary>
internal interface ITypeMeta
{
    internal Type Get();
    internal void SetTypeName(string name);
    internal string GetTypeName();
    static abstract int Count();
    static abstract int GetId(Type type);
}
