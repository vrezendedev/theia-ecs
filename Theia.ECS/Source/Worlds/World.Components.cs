using System;
using System.Threading;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Unique[] _uniques;
    private Lock _uniquesLock = new();

    private Unique<TComponent> GetOrCreateUnique<TComponent>()
        where TComponent : struct
    {
        int componentId = ComponentMeta<TComponent>.s_id;

        lock (_uniquesLock)
        {
            if (componentId > _uniques.Length - 1)
                Array.Resize(ref _uniques, componentId + 1);

            if (_uniques[componentId] is null)
                _uniques[componentId] = new Unique<TComponent>();

            return (Unique<TComponent>)_uniques[componentId];
        }
    }

    public TComponent ReadUnique<TComponent>()
        where TComponent : struct => GetOrCreateUnique<TComponent>().Read();

    public ref TComponent GetUnique<TComponent>()
        where TComponent : struct => ref GetOrCreateUnique<TComponent>().Get();

    public void SetUnique<TComponent>(in TComponent component)
        where TComponent : struct => GetOrCreateUnique<TComponent>().Set(component);
}
