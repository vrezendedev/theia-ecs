using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theia.ECS.Reflection;
using Theia.ECS.Reflection.Types;
using Theia.ECS.Relations.Attributes;

namespace Theia.ECS.Relations;

/// <summary>
/// Central registry that assigns and resolves the integer IDs used to identify relation types.
/// Maintains the mapping between a <see cref="Type"/> and its associated <see cref="RelationType"/> metadata.
/// </summary>
/// <remarks>
/// <para>
/// Relation IDs are <b>stable within a process but are not guaranteed to be stable across runs</b>.
/// </para>
/// </remarks>
internal static class RelationsMeta
{
    private static TypeRegistry<RelationType> s_relationRegistry = new();

    /// <summary>
    /// Returns the relation ID for <typeparamref name="TRelation"/>, registering it on first
    /// encounter. Subsequent calls for the same type return the previously assigned ID.
    /// </summary>
    /// <param name="isTag"><see langword="true"/> if the relation is a fieldless tag; otherwise
    /// <see langword="false"/>. Determined by the caller via <see cref="IsTag{TRelation}"/>.</param>
    /// <returns>The integer ID associated with <typeparamref name="TRelation"/>.</returns>
    /// <remarks>
    /// The fast path resolves an already-registered type without entering the registry's lock;
    /// only first-time registrations pay for synchronization. Atomicity of the registration
    /// itself, including the duplicate-registration check, is handled inside
    /// <see cref="TypeRegistry{T}.Register(in T)"/>.
    /// </remarks>
    internal static int AttemptRegisterRelation<TRelation>(bool isTag)
        where TRelation : struct
    {
        if (s_relationRegistry.TryGetTypeId(typeof(TRelation), out int componentId))
            return componentId;

        return s_relationRegistry.Register(new RelationType<TRelation>(typeof(TRelation), isTag));
    }

    /// <summary>
    /// Resolves a relation type by its assembly-qualified name and registers it if not already
    /// present. <b>Used during deserialization to restore relations that were persisted by name</b>.
    /// </summary>
    /// <param name="name">The assembly-qualified type name, as accepted by <see cref="Type.GetType(string)"/>.</param>
    /// <returns>The integer ID associated with the resolved relation type.</returns>
    /// <remarks>
    /// Because the concrete type is not known at compile time,
    /// this overload uses reflection to construct the matching <c>RelationType&lt;T&gt;</c>
    /// and to determine whether the type is a tag. As such, it is significantly more expensive
    /// than the generic overload and <b>is intended for cold paths such as save/load</b>, not
    /// for hot loops.
    /// </remarks>
    /// <exception cref="TypeLoadException">Thrown when <paramref name="name"/> cannot be resolved to a loaded type.</exception>
    internal static int AttemptRegisterRelation(string name)
    {
        if (s_relationRegistry.TryGetTypeId(name, out int relationId))
            return relationId;

        Type? type = Type.GetType(name);

        if (type is null)
            TypeRegistry<RelationType>.ThrowTypeLoadException(name);

        Type genericType = typeof(RelationType<>).MakeGenericType([type]);

        RelationType relationType = (RelationType)Activator.CreateInstance(genericType)!;

        relationType.Initialize(type, IsTag(type));

        return s_relationRegistry.Register(relationType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetRelationId(Type type) => s_relationRegistry.GetTypeId(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetRelationId(string typeName) => s_relationRegistry.GetTypeId(typeName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static RelationType GetRelationType(int index) =>
        s_relationRegistry.GetTypeMeta(index);

    /// <summary>Returns the number of relations currently registered.</summary>
    internal static int Count() => s_relationRegistry.Count();

    /// <summary>
    /// Returns <see langword="true"/> if <typeparamref name="TRelation"/> is a tag relation,
    /// i.e. has no instance fields and exists purely to mark that a link of that type is present.
    /// </summary>
    internal static bool IsTag<TRelation>()
        where TRelation : struct => IsTag(typeof(TRelation));

    private static bool IsTag(Type type) => type.GetFields(BlittableMeta.Flags).Length == 0;

    /// <summary>
    /// Returns <see langword="true"/> if <typeparamref name="T"/> is decorated with the
    /// <see cref="Relationship"/> attribute. Used to enforce that all relation types declare
    /// their relationship semantics explicitly.
    /// </summary>
    internal static bool HasRelationshipAttribute<T>()
        where T : struct => typeof(T).GetCustomAttribute<Relationship>() is not null;
}

/// <summary>
/// Per-type cache that <b>lazily registers</b> <typeparamref name="TRelation"/> with the global
/// <see cref="RelationsMeta"/> registry on first access and exposes its assigned ID.
/// </summary>
/// <remarks>
/// <para>
/// The static constructor enforces that <typeparamref name="TRelation"/> is a valid relation.
/// <br/>
/// Unlike components, a relation is allowed to be fieldless; that is the defining characteristic of a tag relation.
/// </para>
/// <para>
/// Because each closed generic instantiation has its own static state, <see cref="s_id"/> acts
/// as an effectively free lookup after the first access; no dictionary hit, just a static field read.
/// </para>
/// </remarks>
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

        s_id = RelationsMeta.AttemptRegisterRelation<TRelation>(RelationsMeta.IsTag<TRelation>());
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
