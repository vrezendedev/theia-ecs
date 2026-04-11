using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection;
using Theia.ECS.Reflection.Types;
using Theia.ECS.Relations.Attributes;

namespace Theia.ECS.Relations;

internal static class RelationsMeta
{
    private static TypeRegistry<RelationType> s_relationRegistry = new();

    internal static int RegisterRelation<TRelation>(bool isTag)
        where TRelation : struct
    {
        if (s_relationRegistry.TryGetTypeId(typeof(TRelation), out int relationId))
            return relationId;

        relationId = s_relationRegistry.Account();

        RelationType<TRelation> relationType = new RelationType<TRelation>(
            typeof(TRelation),
            isTag
        );

        s_relationRegistry.Set(relationId, relationType);

        return relationId;
    }

    internal static int RegisterRelation(Type type)
    {
        if (!s_relationRegistry.TryGetTypeId(type, out int relationId))
        {
            relationId = s_relationRegistry.Account();

            Type genericType = typeof(RelationType<>).MakeGenericType([type]);

            RelationType relationType = (RelationType)Activator.CreateInstance(genericType)!;

            relationType.Initialize(type, IsTag(type));

            s_relationRegistry.Set(relationId, relationType);
        }

        return relationId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetRelationId(Type type) => s_relationRegistry.GetTypeId(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static RelationType GetRelationType(int index) =>
        s_relationRegistry.GetTypeMeta(index);

    internal static int Count() => s_relationRegistry.Count();

    internal static bool IsTag<TRelation>()
        where TRelation : struct => IsTag(typeof(TRelation));

    private static bool IsTag(Type type) => type.GetFields(BlittableMeta.Flags).Length == 0;

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

        if (typeof(TRelation).IsPrimitive || typeof(TRelation).IsEnum)
            ThrowRelationNotAStructException();

        if (!RelationsMeta.HasRelationshipAttribute<TRelation>())
            ThrowRelationshipAttributeNotAdded();

        s_id = RelationsMeta.RegisterRelation<TRelation>(RelationsMeta.IsTag<TRelation>());
    }

    [DoesNotReturn]
    private static void ThrowRelationshipAttributeNotAdded() =>
        throw new InvalidOperationException(
            $"Relation '{typeof(TRelation).Name}' must add Relationship attribute."
        );

    [DoesNotReturn]
    private static void ThrowRelationNotAStructException() =>
        throw new InvalidOperationException(
            $"Relation '{typeof(TRelation).Name}' must be a struct."
        );
}
