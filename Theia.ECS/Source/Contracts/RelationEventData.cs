using System;
using Theia.ECS.Entities;
using Theia.ECS.Enums;
using Theia.ECS.Relations;
using Theia.ECS.Worlds;

namespace Theia.ECS.Contracts;

public readonly ref struct RelationModified
{
    public readonly World World;
    public readonly Entity Owner;
    public readonly Entity Target;
    public readonly Type Type;
    internal readonly int _relationId;

    internal RelationModified(
        World world,
        Entity owner,
        Entity target,
        in Type type,
        int relationId
    )
    {
        World = world;
        Owner = owner;
        Target = target;
        Type = type;
        _relationId = relationId;
    }

    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum => StructEnum<TEnum, RelationType>.FromStruct(_relationId);
}
