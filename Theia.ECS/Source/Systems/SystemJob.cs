using Theia.ECS.Jobs;

namespace Theia.ECS.Systems;

/// <summary>
/// <see cref="Job"/> wrapper that runs a single <see cref="BaseSystem"/>'s full per-tick
/// lifecycle inside a worker thread.
/// </summary>
internal sealed class SystemJob : Job
{
    private readonly BaseSystem _system;

    internal SystemJob(BaseSystem system) => _system = system;

    public override void Execute()
    {
        _system.Before();
        _system.Run();
        _system.After();
    }
}
