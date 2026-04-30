using System;

namespace Theia.ECS.Systems;

/// <summary>
/// Container that owns the ordered list of systems for a <see cref="Worlds.World">World</see>
/// and drives them each tick. Calls <see cref="BaseSystem.Before"/>, <see cref="BaseSystem.Run"/>,
/// and <see cref="BaseSystem.After"/> on every system in registration order.
/// </summary>
/// <remarks>
/// <para>
/// Execution order is the order in which systems are passed to the constructor. Systems run
/// strictly sequentially at the root level; parallelism is opt-in by composing children inside
/// a <see cref="ParallelSystems"/> node, which appears as a single system from the root's
/// perspective.
/// </para>
/// <para>
/// <see cref="Dispose"/> disposes every owned system in the same order.
/// </para>
/// </remarks>
public sealed class SystemsRoot : IDisposable
{
    private readonly BaseSystem[] _systems;

    /// <summary>
    /// Constructs the root with the systems; <b>the order in which
    /// <paramref name="systems"/> are supplied is the order in which they will run</b>.
    /// </summary>
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
