using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.ECS.Components;

/// <summary>
/// Non-generic base for a per-component-type FIFO queue of pending typed values, used by the
/// deferred-command system to stage component data captured during system execution and applied
/// at the next flush point.
/// </summary>
/// <remarks>
/// <para>
/// During query execution, the world cannot apply structural component changes immediately.
/// Instead, the typed value supplied with each pending change is enqueued in the matching
/// <see cref="ComponentDeferredStorage{TComponent}"/> while the change itself is recorded in a separate command queue.
/// At flush time, the world drains the command queue in order; for each command that carries a typed value, it calls
/// <see cref="SetWithNext"/> on the matching storage to pop and apply the corresponding queued value.
/// </para>
/// <para>
/// <b>FIFO order is essential: commands and values are correlated by position.</b>
/// <see cref="DiscardNext"/> exists for the case where a command must be dropped (for example,
/// the target entity was destroyed before the flush reached it). The queued value still has to
/// be popped to keep subsequent commands aligned with their values.
/// </para>
/// </remarks>
internal abstract class ComponentDeferredStorage
{
    /// <summary>Pops the next queued value and <b>writes it</b> into <paramref name="to"/> at the row indicated by <paramref name="entityMeta"/>.</summary>
    internal abstract void SetWithNext(in EntityMeta entityMeta, Archetype to);

    /// <summary>Pops the next queued value <b>without applying it</b>. Used when the matching command can no longer be carried out and the queue must stay aligned with the command stream.</summary>
    internal abstract void DiscardNext();
}
