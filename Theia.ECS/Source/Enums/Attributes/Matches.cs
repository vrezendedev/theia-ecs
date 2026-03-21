using System;
using Theia.ECS.Components;

namespace Theia.ECS.Enums.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class Matches<TComponent> : Attribute
    where TComponent : struct
{
    static Matches() => _ = ComponentMeta<TComponent>.s_id;
}
