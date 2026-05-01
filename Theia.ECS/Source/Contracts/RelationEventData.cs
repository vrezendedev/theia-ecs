using System;
using Theia.ECS.Entities;
using Theia.ECS.Enums;
using Theia.ECS.Relations;
using Theia.ECS.Worlds;

namespace Theia.ECS.Contracts;

/// <summary>
/// Event payload for <see cref="Events.RelationsEvents.OnAnyRelationAdded">OnAnyRelationAdded</see> and the matching
/// removal events. Carries the owner and target entities and the relation's runtime type, plus
/// an enum-classification hook via <see cref="As{TEnum}"/>.
/// </summary>
public readonly ref struct RelationModified
{
    /// <summary>The world the relation change occurred in.</summary>
    public readonly World World;

    /// <summary>The entity owning the relation.</summary>
    public readonly Entity Owner;

    /// <summary>The entity the relation points at.</summary>
    public readonly Entity Target;

    /// <summary>The runtime <see cref="System.Type"/> of the relation that was added or removed.</summary>
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

    /// <summary>
    /// Returns the <typeparamref name="TEnum"/> value that classifies the affected relation
    /// type, as declared via <c>Includes&lt;T&gt;</c> or <c>Matches&lt;T&gt;</c> on the enum.
    /// Returns <see langword="default"/> when the relation has no mapping.
    /// </summary>
    public TEnum As<TEnum>()
        where TEnum : unmanaged, Enum => StructEnum<TEnum, RelationType>.FromStruct(_relationId);
}
