using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

/// <summary>
/// Result of <see cref="Worlds.World.CreateEntity">World.CreateEntity()</see>: pairs the freshly created
/// <see cref="Entity"/> handle with a reference to its <see cref="EntityMeta"/> slot. The
/// <c>ref</c> field lets callers patch metadata fields (archetype index, storage index,
/// component index) without a dictionary lookup or array re-index.
/// </summary>
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
