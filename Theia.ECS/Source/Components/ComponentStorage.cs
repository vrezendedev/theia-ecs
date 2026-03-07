using Theia.ECS.Entities;

namespace Theia.ECS.Components;

internal abstract class ComponentStorage
{
    internal int Count { get; set; }
    internal abstract void Add(ref Entity e, ref EntityMeta entityMeta);
    internal abstract void Remove(ref Entity e, ref EntityMeta entityMeta);
    internal abstract void Transfer(
        ref Entity e,
        ref EntityMeta entityMeta,
        ComponentStorage target
    );
    internal abstract void Move(int from, int to);
}
