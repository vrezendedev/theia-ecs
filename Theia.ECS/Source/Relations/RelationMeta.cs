using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection;
using Theia.ECS.Reflection.Types;
using Theia.ECS.Relations.Attributes;

namespace Theia.ECS.Relations;

internal enum RelationCardinality
{
    OneToOne,
    OneToMany,
    ManyToMany,
}

internal enum RelationSubtype
{
    Tag,
    Data,
}

internal static class RelationsMeta
{
    private static TypeRegistry<RelationType> s_relationRegistry = new();

    internal static int RegisterRelation<TRelation>(
        RelationCardinality cardinality,
        RelationSubtype subtype
    )
        where TRelation : struct
    {
        int relationId = s_relationRegistry.Account();
        RelationType<TRelation> relationType = new RelationType<TRelation>(
            typeof(TRelation),
            cardinality,
            subtype
        );
        s_relationRegistry.Set(relationId, relationType);
        return relationId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetRelationId(Type type) => s_relationRegistry.GetTypeId(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static RelationType GetRelationType(int index) =>
        s_relationRegistry.GetTypeMeta(index);

    internal static int Count() => s_relationRegistry.Count();

    internal static bool ContainsRelationsAttributes<T>()
        where T : struct =>
        typeof(T).GetCustomAttribute<OneToOne>() is not null
        || typeof(T).GetCustomAttribute<OneToMany>() is not null
        || typeof(T).GetCustomAttribute<ManyToMany>() is not null;

    internal static bool IsTag<TRelation>()
        where TRelation : struct => typeof(TRelation).GetFields(BlittableMeta.Flags).Length == 0;

    internal static RelationCardinality ValidateRelation<TRelation>()
    {
        OneToOne? oneToOne = typeof(TRelation).GetCustomAttribute<OneToOne>();
        OneToMany? oneToMany = typeof(TRelation).GetCustomAttribute<OneToMany>();
        ManyToMany? manyToMany = typeof(TRelation).GetCustomAttribute<ManyToMany>();

        bool hasOneToOne = oneToOne is not null;
        bool hasOneToMany = oneToMany is not null;
        bool hasManyToMany = manyToMany is not null;

        int relationAttributesCount = hasOneToOne ? 1 : 0;
        relationAttributesCount += hasOneToMany ? 1 : 0;
        relationAttributesCount += hasManyToMany ? 1 : 0;

        if (relationAttributesCount == 0)
            ThrowCardinalityNotSpecified<TRelation>();

        if (relationAttributesCount > 1)
            ThrowMultipleCardinalities<TRelation>();

        return hasOneToOne ? RelationCardinality.OneToOne
            : hasOneToMany ? RelationCardinality.OneToMany
            : RelationCardinality.ManyToMany;
    }

    [DoesNotReturn]
    private static void ThrowCardinalityNotSpecified<TRelation>() =>
        throw new InvalidOperationException(
            $"Relation '{typeof(TRelation).Name}' must define a cardinality."
        );

    [DoesNotReturn]
    private static void ThrowMultipleCardinalities<TRelation>() =>
        throw new InvalidOperationException(
            $"Relation '{typeof(TRelation).Name}' cannot define more than one cardinality. A relation must have exactly one cardinality."
        );
}

internal static class RelationMeta<TRelation>
    where TRelation : struct
{
    internal static readonly int s_id;

    static RelationMeta()
    {
        if (!BlittableMeta.IsStrictlyBlittable(typeof(TRelation)))
            BlittableMeta.ThrowBlittableException<TRelation>();

        RelationCardinality cardinality = RelationsMeta.ValidateRelation<TRelation>();

        RelationSubtype subtype = RelationsMeta.IsTag<TRelation>()
            ? RelationSubtype.Tag
            : RelationSubtype.Data;

        s_id = RelationsMeta.RegisterRelation<TRelation>(cardinality, subtype);
    }
}
