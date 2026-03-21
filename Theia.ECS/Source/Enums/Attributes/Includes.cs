using System;
using Theia.ECS.Components;

namespace Theia.ECS.Enums.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class Includes<TComponent> : Attribute
    where TComponent : struct
{
    static Includes() => _ = ComponentMeta<TComponent>.s_id;
}
