namespace Theia.ECS.Contracts;

internal readonly ref struct EntityTransferred
{
    internal readonly EntityAccounted _entityAccounted;
    internal readonly EntitySwapped _entitySwapped;

    internal EntityTransferred(EntityAccounted entityAccounted, EntitySwapped entitySwapped)
    {
        _entityAccounted = entityAccounted;
        _entitySwapped = entitySwapped;
    }
}
