namespace Theia.ECS.Components;

internal sealed class ComponentType<T> : ComponentType
{
    internal ComponentType(int size)
        : base(size) { }

    internal override ComponentStorage CreateStorage(int capacity) =>
        new ComponentStorage<T>(capacity);
}
