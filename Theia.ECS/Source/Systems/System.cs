using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Systems;

/// <summary>
/// Base class for every system in Theia ECS. Defines the per-tick lifecycle:
/// <see cref="Before"/>, <see cref="Run"/>, <see cref="After"/>. And the
/// <see cref="IDisposable"/> contract for releasing resources at <see cref="Worlds.World">World</see> teardown.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="SystemsRoot"/> invokes <see cref="Before"/>, then <see cref="Run"/>, then <see cref="After"/>
/// in order on every tick. <see cref="Before"/> and <see cref="After"/>
/// are open extension points for per-frame setup and cleanup; <see cref="Run"/> is the dispatch
/// hook used by derived classes to invoke <b>the user's</b> <c>Execute</c> <b>overrides with whatever
/// queries and shared state the variant supplies</b>.
/// </para>
/// <para>
/// <b>User code does not derive from</b> <see cref="BaseSystem"/> <b>directly</b>.
/// </para>
/// </remarks>
public abstract class BaseSystem : IDisposable
{
    /// <summary>Per-tick setup hook, called by the <see cref="SystemsRoot"/> before <see cref="Run"/>.</summary>
    public virtual void Before() { }

    internal abstract void Run();

    /// <summary>Per-tick cleanup hook, called by the scheduler after <see cref="Run"/>.</summary>
    public virtual void After() { }

    /// <inheritdoc/>
    public virtual void Dispose() { }
}

/// <summary>
/// Parameterless system. Override <see cref="Execute"/> with the work to perform each tick;
/// the scheduler will call it between <see cref="BaseSystem.Before"/> and
/// <see cref="BaseSystem.After"/>.
/// </summary>
public abstract class System : BaseSystem
{
    internal override void Run() => Execute();

    /// <summary>Per-tick work performed by this system.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Execute();
}
