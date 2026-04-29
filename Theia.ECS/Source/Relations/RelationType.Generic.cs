using System;

namespace Theia.ECS.Relations;

/// <summary>
/// Generic concrete implementation of <see cref="RelationType"/> for a specific
/// <typeparamref name="TRelation"/>. Owns the type-parameterized construction of
/// <see cref="Relation"/> instances, choosing between a tag-only <see cref="Relation"/> and a
/// payload-bearing <see cref="EvaluatedRelation{TRelation}"/> based on the tag flag.
/// </summary>
internal sealed class RelationType<TRelation> : RelationType
    where TRelation : struct
{
    /// <summary>
    /// Parameterless constructor used by the reflection-based registration path; see
    /// <see cref="RelationType()"/> on the base class.
    /// </summary>
    public RelationType()
        : base() { }

    internal RelationType(Type type, bool isTag)
        : base(type, isTag) { }

    /// <summary>
    /// Returns a fresh or recycled <see cref="Relation"/>. Pool dequeue is the fast path; on
    /// pool miss, a tag relation allocates a plain <see cref="Relation"/>, while a data
    /// relation allocates the <see cref="EvaluatedRelation{TRelation}"/> variant that carries
    /// the per-link payload.
    /// </summary>
    internal override Relation CreateRelation()
    {
        Relation? relation;

        lock (_relationPoolLock)
        {
            _relationPool.TryDequeue(out relation);
        }

        if (relation is null)
            relation = _isTag ? new Relation() : new EvaluatedRelation<TRelation>();
        else
            relation.Reset();

        return relation;
    }
}
