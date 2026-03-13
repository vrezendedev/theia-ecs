using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

internal readonly ref struct EntityCreated
{
    internal readonly Entity _entity;
    internal readonly ref EntityMeta _entityMeta;

    internal EntityCreated(Entity entity, ref EntityMeta entityMeta)
    {
        _entity = entity;
        _entityMeta = ref entityMeta;
    }
}
