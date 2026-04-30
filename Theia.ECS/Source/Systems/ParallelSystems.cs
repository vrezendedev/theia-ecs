using System;
using System.Diagnostics.CodeAnalysis;
using Theia.ECS.Jobs;

namespace Theia.ECS.Systems;

/// <summary>
/// Composite system that runs two or more child <see cref="BaseSystem"/> instances <b>in parallel
/// each tick</b>. Each child is wrapped in a <see cref="SystemJob"/> and dispatched through
/// <see cref="JobScheduler"/>; the parent <c>Run</c> returns once all children have finished.
/// </summary>
/// <remarks>
/// <para>
/// <b>Correctness is the caller's responsibility.</b> The framework does not analyze the child
/// systems' component access to detect conflicts: if two children write to the same component
/// type on overlapping entity sets, or one writes while another reads, the result is undefined.
/// Compose only systems whose data access is known to be disjoint.
/// </para>
/// <para>
/// Each child runs its own <see cref="BaseSystem.Before"/>, <see cref="BaseSystem.Run"/>, and
/// <see cref="BaseSystem.After"/> sequence inside its job, so per-system lifecycle hooks are
/// preserved.
/// </para>
/// <para>
/// <see cref="ParallelSystems"/> itself is also a <see cref="BaseSystem"/>, so its own
/// <see cref="BaseSystem.Before"/> and <see cref="BaseSystem.After"/> wrap the parallel
/// dispatch as a unit.
/// </para>
/// </remarks>
public abstract class ParallelSystems : BaseSystem
{
    private readonly BaseSystem[] _systems;
    private readonly SystemJob[] _jobs;

    /// <summary>
    /// Constructs a parallel composite over <paramref name="systems"/>. At least two systems
    /// must be supplied; a single system has no parallelism to exploit and is rejected outright
    /// rather than silently downgrading to sequential execution.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when fewer than two systems are supplied.</exception>
    public ParallelSystems(params ReadOnlySpan<BaseSystem> systems)
    {
        if (systems.Length < 2)
            ThrowRequiresAtLeastTwoSystems();

        _systems = systems.ToArray();

        _jobs = new SystemJob[_systems.Length];

        for (int i = 0; i < _systems.Length; i++)
            _jobs[i] = new SystemJob(_systems[i]);
    }

    internal override void Run() => JobScheduler.Run(_jobs.AsSpan());

    [DoesNotReturn]
    private void ThrowRequiresAtLeastTwoSystems() =>
        throw new InvalidOperationException(
            "ParallelSystems must be composed of at least two systems."
        );
}
