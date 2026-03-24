using System;
using Theia.ECS.Components;
using Theia.ECS.Relations;

namespace Theia.ECS.Enums.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class Includes<T> : Attribute
    where T : struct
{
    static Includes()
    {
        if (ComponentsMeta.ContainsFields<T>())
            _ = ComponentMeta<T>.s_id;

        if (RelationsMeta.ContainsRelationsAttributes<T>())
            _ = RelationMeta<T>.s_id;
    }
}
