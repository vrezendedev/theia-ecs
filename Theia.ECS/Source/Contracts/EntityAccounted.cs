namespace Theia.ECS.Contracts;

internal readonly ref struct EntityAccounted
{
    internal readonly int _archetypeIndex;
    internal readonly int _storageIndex;
    internal readonly int _componentIndex;

    internal EntityAccounted(int archetypeIndex, int storageIndex, int componentIndex)
    {
        _archetypeIndex = archetypeIndex;
        _storageIndex = storageIndex;
        _componentIndex = componentIndex;
    }
}
