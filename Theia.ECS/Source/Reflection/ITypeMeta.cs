using System;

namespace Theia.ECS.Reflection;

internal interface ITypeMeta
{
    internal Type Get();
    static abstract int Count();
    static abstract int GetId(Type type);
}
