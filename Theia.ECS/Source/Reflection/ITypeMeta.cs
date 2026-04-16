using System;

namespace Theia.ECS.Reflection;

internal interface ITypeMeta
{
    internal Type Get();
    internal void SetTypeName(string name);
    internal string GetTypeName();
    static abstract int Count();
    static abstract int GetId(Type type);
}
