using Theia.ECS.Archetypes;

namespace Theia.ECS.Contracts;

internal readonly ref struct EntityAccounted
{
    internal readonly Archetype _archetype;
    internal readonly int _storageIndex;
    internal readonly int _componentIndex;

    internal EntityAccounted(Archetype archetype, int storageIndex, int componentIndex)
    {
        _archetype = archetype;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
    }
}
