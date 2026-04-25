using System;
using System.Diagnostics.CodeAnalysis;
using Theia.ECS.Jobs;

namespace Theia.ECS.Systems;

public abstract class ParallelSystems : BaseSystem
{
    private readonly BaseSystem[] _systems;
    private readonly SystemJob[] _jobs;

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
