using System;
using Theia.ECS.Components;
using Theia.ECS.Relations;

namespace Theia.ECS.Enums.Attributes;

/// <summary>
/// Attaches an enum field to one of several component or relation types it represents.
/// Multiple <see cref="Includes{T}"/> attributes may decorate the same enum value, grouping
/// many distinct types under a single bucket; useful for categorizing components or
/// relations into broader labels.
/// </summary>
/// <typeparam name="T">
/// A component or relation struct. The static constructor force-touches the appropriate
/// <see cref="ComponentMeta{T}"/> or <see cref="RelationMeta{T}"/>, ensuring <typeparamref name="T"/>
/// is registered with its registry before any <see cref="StructEnum{TEnum, TTypeMeta}"/> tries
/// to resolve its ID.
/// </typeparam>
/// <remarks>
/// An enum may use either <see cref="Includes{T}"/> or <c>Matches&lt;T&gt;</c> but not both;
/// the two styles are mutually exclusive and validated at first access by
/// <see cref="StructEnum{TEnum, TTypeMeta}"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class Includes<T> : Attribute
    where T : struct
{
    static Includes()
    {
        if (ComponentsMeta.ContainsFields<T>())
            _ = ComponentMeta<T>.s_id;

        if (RelationsMeta.HasRelationshipAttribute<T>())
            _ = RelationMeta<T>.s_id;
    }
}
