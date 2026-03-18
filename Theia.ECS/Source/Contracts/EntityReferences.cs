using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

internal readonly ref struct EntityReferences
{
    internal readonly bool _valid;
    internal readonly ref EntityMeta _entityMeta;
    internal readonly Archetype? _archetype;

    public EntityReferences()
    {
        _valid = false;
    }

    internal EntityReferences(ref EntityMeta entityMeta, Archetype archetype)
    {
        _entityMeta = ref entityMeta;
        _archetype = archetype;
        _valid = true;
    }

    internal static EntityReferences Invalid => new() { };
}
