using System;
using Theia.ECS.Components;
using Theia.ECS.Relations;

namespace Theia.ECS.Enums.Attributes;

/// <summary>
/// Attaches an enum field to the single component or relation type it represents. Each enum
/// value carries at most one <see cref="Matches{T}"/> attribute, establishing a strict
/// one-to-one mapping between enum value and struct type.
/// </summary>
/// <typeparam name="T">
/// A component or relation struct. The static constructor force-touches the appropriate
/// <see cref="ComponentMeta{T}"/> or <see cref="RelationMeta{T}"/>, ensuring <typeparamref name="T"/>
/// is registered with its registry before any <see cref="StructEnum{TEnum, TTypeMeta}"/> tries
/// to resolve its ID.
/// </typeparam>
/// <remarks>
/// An enum may use either <c>Includes&lt;T&gt;</c> or <see cref="Matches{T}"/> but not both;
/// the two styles are mutually exclusive and validated at first access by
/// <see cref="StructEnum{TEnum, TTypeMeta}"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class Matches<T> : Attribute
    where T : struct
{
    static Matches()
    {
        if (ComponentsMeta.ContainsFields<T>())
            _ = ComponentMeta<T>.s_id;

        if (RelationsMeta.HasRelationshipAttribute<T>())
            _ = RelationMeta<T>.s_id;
    }
}
