using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Unique[] _uniques = Array.Empty<Unique>();
    private readonly Lock _uniquesLock = new();

    private Unique<TComponent> GetOrCreateUnique<TComponent>()
        where TComponent : struct
    {
        int componentId = ComponentMeta<TComponent>.s_id;

        Unique[] uniques = _uniques;

        if (componentId < uniques.Length && uniques[componentId] is not null)
            return (Unique<TComponent>)uniques[componentId];

        lock (_uniquesLock)
        {
            if (componentId >= _uniques.Length)
                Array.Resize(ref _uniques, componentId + 1);

            if (_uniques[componentId] is null)
                _uniques[componentId] = new Unique<TComponent>();

            return (Unique<TComponent>)_uniques[componentId];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<Unique> GetUniques() => _uniques;

    public TComponent ReadUnique<TComponent>()
        where TComponent : struct => GetOrCreateUnique<TComponent>().Read();

    public ref TComponent GetUnique<TComponent>()
        where TComponent : struct => ref GetOrCreateUnique<TComponent>().Get();

    public void SetUnique<TComponent>(in TComponent component)
        where TComponent : struct => GetOrCreateUnique<TComponent>().Set(component);

    public void UpdateUnique<TComponent>(UpdateUnique<TComponent> update)
        where TComponent : struct => GetOrCreateUnique<TComponent>().Update(update);
}
