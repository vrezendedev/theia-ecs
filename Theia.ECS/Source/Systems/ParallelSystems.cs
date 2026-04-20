using System;
using System.Diagnostics.CodeAnalysis;

namespace Theia.ECS.Systems;

public abstract class ParallelSystems : BaseSystem
{
    public static bool IsExecutingParallel { get; private set; }

    internal readonly BaseSystem[] _systems;

    public ParallelSystems(params ReadOnlySpan<BaseSystem> systems)
    {
        for (int i = 0; i < systems.Length; i++)
            if (systems[i] is ParallelSystems)
                ThrowParallelSystemsNotAllowed();

        _systems = systems.ToArray();
    }

    internal override void Run()
    {
        IsExecutingParallel = true;

        Before();

        //@TO-DO Parallel

        After();

        IsExecutingParallel = false;
    }

    [DoesNotReturn]
    private void ThrowParallelSystemsNotAllowed() =>
        throw new InvalidOperationException("Nested ParallelSystems are not allowed.");
}
