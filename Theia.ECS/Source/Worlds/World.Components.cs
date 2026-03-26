using System;
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

        if ((uint)componentId < (uint)uniques.Length && uniques[componentId] is not null)
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

    public TComponent ReadUnique<TComponent>()
        where TComponent : struct => GetOrCreateUnique<TComponent>().Read();

    public void SetUnique<TComponent>(SetUnique<TComponent> set)
        where TComponent : struct => GetOrCreateUnique<TComponent>().Set(set);
}
