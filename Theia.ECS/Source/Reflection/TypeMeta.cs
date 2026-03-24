using System;

namespace Theia.ECS.Reflection;

internal abstract class TypeMeta
{
    internal readonly Type _type;

    internal TypeMeta(Type type) => _type = type;
}
