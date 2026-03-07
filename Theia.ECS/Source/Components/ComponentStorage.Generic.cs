using Theia.ECS.Entities;

namespace Theia.ECS.Components;

internal sealed class ComponentStorage<T> : ComponentStorage
{
    private T[] _components;

    public ComponentStorage(int capacity) => _components = new T[capacity];

    internal override void Add(ref Entity e, ref EntityMeta entityMeta)
    {
        throw new System.NotImplementedException();
    }

    internal override void Move(int from, int to)
    {
        throw new System.NotImplementedException();
    }

    internal override void Remove(ref Entity e, ref EntityMeta entityMeta)
    {
        throw new System.NotImplementedException();
    }

    internal override void Transfer(
        ref Entity e,
        ref EntityMeta entityMeta,
        ComponentStorage target
    )
    {
        throw new System.NotImplementedException();
    }
}
