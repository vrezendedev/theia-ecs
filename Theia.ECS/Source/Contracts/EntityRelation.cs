using System;
using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

/// <summary>
/// Iteration shape exposing both halves of an <see cref="Relations.EvaluatedRelation{TRelation}"/>
/// as parallel spans: the related entities and their per-link payloads, <b>indexed identically</b>.
/// </summary>
/// <typeparam name="T">The relation's per-link payload type.</typeparam>
/// <remarks>
/// Use this when iterating both sides together is more convenient than the callback-driven <see cref="Relations.IQueryEvaluatedRelation{TRelation}"/>.
/// <br/>
/// <see cref="Data"/> is mutable, and <b>writes through the span persist in the underlying relation
/// storage</b>.
/// </remarks>
public readonly ref struct EntityEvaluatedRelations<T>
    where T : struct
{
    /// <summary>The related entities, in dense iteration order. Aligned index-for-index with <see cref="Data"/>.</summary>
    public readonly ReadOnlySpan<Entity> Entities;

    /// <summary>The per-link payloads, mutable. Aligned index-for-index with <see cref="Entities"/>; <b>mutations persist in storage</b>.</summary>
    public readonly Span<T> Data;

    public EntityEvaluatedRelations(ReadOnlySpan<Entity> entities, Span<T> data)
    {
        Entities = entities;
        Data = data;
    }
}
