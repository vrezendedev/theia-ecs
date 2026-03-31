using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection;
using Theia.ECS.Reflection.Types;
using Theia.ECS.Relations.Attributes;

namespace Theia.ECS.Relations;

internal enum RelationSubtype
{
    Tag,
    Evaluated,
}

internal static class RelationsMeta
{
    private static TypeRegistry<RelationType> s_relationRegistry = new();

    internal static int RegisterRelation<TRelation>(RelationSubtype subtype)
        where TRelation : struct
    {
        int relationId = s_relationRegistry.Account();

        RelationType<TRelation> relationType = new RelationType<TRelation>(
            typeof(TRelation),
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

    internal static bool IsTag<TRelation>()
        where TRelation : struct => typeof(TRelation).GetFields(BlittableMeta.Flags).Length == 0;

    internal static bool HasRelationshipAttribute<T>()
        where T : struct => typeof(T).GetCustomAttribute<Relationship>() is not null;
}

internal static class RelationMeta<TRelation>
    where TRelation : struct
{
    internal static readonly int s_id;

    static RelationMeta()
    {
        if (!BlittableMeta.IsStrictlyBlittable(typeof(TRelation)))
            BlittableMeta.ThrowBlittableException<TRelation>();

        if (!RelationsMeta.HasRelationshipAttribute<TRelation>())
            ThrowRelationshipAttributeNotAdded();

        RelationSubtype subtype = RelationsMeta.IsTag<TRelation>()
            ? RelationSubtype.Tag
            : RelationSubtype.Evaluated;

        s_id = RelationsMeta.RegisterRelation<TRelation>(subtype);
    }

    [DoesNotReturn]
    private static void ThrowRelationshipAttributeNotAdded() =>
        throw new InvalidOperationException(
            $"Relation '{typeof(TRelation).Name}' must add Relationship attribute."
        );
}
