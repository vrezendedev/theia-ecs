using System;
using Theia.ECS.Entities;
using Theia.ECS.Enums;
using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

public readonly ref struct RelationModified
{
    public readonly Entity Owner;
    public readonly Entity Target;
    public readonly Type Type;
    internal readonly int _relationId;

    internal RelationModified(Entity owner, Entity target, in Type type, int relationId)
    {
        Owner = owner;
        Target = target;
        Type = type;
        _relationId = relationId;
    }

    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum => StructEnum<TEnum, RelationType>.FromStruct(_relationId);
}
