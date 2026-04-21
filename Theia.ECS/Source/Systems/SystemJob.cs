using Theia.ECS.Jobs;

namespace Theia.ECS.Systems;

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
