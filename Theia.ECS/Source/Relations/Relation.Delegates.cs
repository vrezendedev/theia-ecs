using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

/// <summary>
/// Per-target callback contract for iterating the entities related to a given owner.
/// <br/>
/// Implementations may be <c>ref struct</c>, allowing the callback to capture spans or other stack-only state.
/// </summary>
/// <remarks>
/// As with <see cref="Queries.IForEachEntity&lt;T&gt;">IForEach&lt;T&gt;</see> on the query system, the <b>struct-callback shape exists for
/// performance</b>: a struct implementing this interface allocates nothing per iteration, and the
/// JIT specializes the iteration loop to the closed generic, inlining <see cref="Execute"/>
/// directly into the loop body.
/// </remarks>
public interface IQueryRelation
{
    /// <summary>Invoked once per related entity, with the related entity as <paramref name="other"/>.</summary>
    public void Execute(Entity other);
}

/// <summary>
/// Per-target callback contract for iterating the entities related to a given owner under a
/// data relation, with the per-link payload supplied by reference.
/// <br/>
/// Mutations to <paramref name="relation"/> data persist in the owner's relation storage.
/// </summary>
/// <typeparam name="TRelation">The relation type, supplied as <c>ref</c> so the callback can read or update the link's payload.</typeparam>
public interface IQueryEvaluatedRelation<TRelation>
    where TRelation : struct
{
    /// <summary>Invoked once per related entity, with the related entity and a reference to the link's payload.</summary>
    public void Execute(Entity other, ref TRelation relation);
}
