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
    Exclusive,
    Tree,
    Multiple,
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
        typeof(T).GetCustomAttribute<Exclusive>() is not null
        || typeof(T).GetCustomAttribute<Tree>() is not null
        || typeof(T).GetCustomAttribute<Multiple>() is not null;

    internal static bool IsTag<TRelation>()
        where TRelation : struct => typeof(TRelation).GetFields(BlittableMeta.Flags).Length == 0;

    internal static RelationCardinality ValidateRelation<TRelation>()
    {
        Exclusive? Exclusive = typeof(TRelation).GetCustomAttribute<Exclusive>();
        Tree? Tree = typeof(TRelation).GetCustomAttribute<Tree>();
        Multiple? Multiple = typeof(TRelation).GetCustomAttribute<Multiple>();

        bool hasExclusive = Exclusive is not null;
        bool hasTree = Tree is not null;
        bool hasMultiple = Multiple is not null;

        int relationAttributesCount = hasExclusive ? 1 : 0;
        relationAttributesCount += hasTree ? 1 : 0;
        relationAttributesCount += hasMultiple ? 1 : 0;

        if (relationAttributesCount == 0)
            ThrowCardinalityNotSpecified<TRelation>();

        if (relationAttributesCount > 1)
            ThrowMultipleCardinalities<TRelation>();

        return hasExclusive ? RelationCardinality.Exclusive
            : hasTree ? RelationCardinality.Tree
            : RelationCardinality.Multiple;
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
