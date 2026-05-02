using Theia.ECS.Entities;

namespace Theia.ECS.Queries;

/// <summary>
/// Per-entity callback contract for query iteration that <b>needs both the entity handle and a
/// reference to its component data</b>. Implementations are <c>struct</c> or <c>ref struct</c> values.
/// </summary>
/// <typeparam name="ComponentT1">The component type whose row is passed by reference.</typeparam>
/// <remarks>
/// <para>
/// <see cref="Execute"/> is invoked once per matched entity in iteration order. The
/// <c>ref</c> parameter writes back to the underlying storage; mutations made inside
/// <see cref="Execute"/> persist in the entity's component data.
/// </para>
/// <para>
/// <b>Why a struct interface and not a lambda?</b> Main reasons:
/// </para>
/// <list type="bullet">
///   <item><description>
///     <b>No allocation:</b> a lambda that captures any local variable allocates a closure object
///     on every iteration call, <b>measurable GC pressure when queries run every frame across
///     thousands of entities</b>. A struct callback lives on the stack; capturing state means
///     placing fields on the struct itself, <b>which costs nothing</b>.
///   </description></item>
///   <item><description>
///     <b>No virtual dispatch:</b> calls through a struct that implements this interface (with
///     <c>where T : struct, IForEach...</c>) are devirtualized by the JIT, each closed generic
///     specializes the iteration loop and inlines <see cref="Execute"/> directly.
///   </description></item>
///   <item><description>
///     <b>Stack-only state:</b> a <c>ref struct</c> implementation can hold spans, refs, and
///     other stack-only data that a delegate cannot capture at all. This lets the callback work
///     with views into world state without copying.
///   </description></item>
/// </list>
/// <para>
/// In short: declare a small struct (or <c>ref struct</c>) implementing this interface, place
/// any per-call state as fields on that struct, and pass it by <c>ref</c> to
/// <c>ForEachEntity</c>. The JIT specializes the iteration to your callback and <b>the loop runs
/// at near-hand-written speed</b>.
/// </para>
/// </remarks>
public interface IForEachEntity<ComponentT1>
    where ComponentT1 : struct
{
    /// <summary>Invoked once per matched entity, with the entity handle and a reference to its component row.</summary>
    public void Execute(Entity entity, ref ComponentT1 componentT1);
}

/// <summary>
/// Per-entity callback contract for query iteration that <b>needs only the component data</b>,
/// not the entity handle. Implementations are typically <c>struct</c> or <c>ref struct</c>
/// values, not delegates, see <see cref="IForEachEntity{ComponentT1}"/> for the rationale
/// behind the struct-callback shape.
/// </summary>
/// <typeparam name="ComponentT1">The component type whose row is passed by reference.</typeparam>
/// <remarks>
/// Use this in preference to <see cref="IForEachEntity{ComponentT1}"/> when the entity handle
/// is not needed; the iteration loop avoids loading the entity span entirely.
/// </remarks>
public interface IForEach<ComponentT1>
    where ComponentT1 : struct
{
    /// <summary>Invoked once per matched entity, with a reference to its component row.</summary>
    public void Execute(ref ComponentT1 componentT1);
}
