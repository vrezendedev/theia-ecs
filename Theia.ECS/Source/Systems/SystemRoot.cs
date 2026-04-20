using System;

namespace Theia.ECS.Systems;

public sealed class SystemsRoot : IDisposable
{
    private readonly BaseSystem[] _systems;

    public SystemsRoot(params ReadOnlySpan<BaseSystem> systems) => _systems = systems.ToArray();

    public void Execute()
    {
        ReadOnlySpan<BaseSystem> baseSystems = _systems;

        for (int i = 0; i < baseSystems.Length; i++)
        {
            BaseSystem system = baseSystems[i];

            system.Before();
            system.Run();
            system.After();
        }
    }

    public void Dispose()
    {
        ReadOnlySpan<BaseSystem> baseSystems = _systems;

        for (int i = 0; i < baseSystems.Length; i++)
        {
            BaseSystem system = baseSystems[i];

            system.Dispose();
        }
    }
}
