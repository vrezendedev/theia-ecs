using Theia.ECS.Components;

namespace Theia.ECS.Tests.Components;

#pragma warning disable CS0649
public class ComponentTypeTests
{
    private struct Position
    {
        public int X;
        public int Y;
    }

    private struct Rotation
    {
        public int X;
        public int Y;
    }

    private struct Scale
    {
        public int X;
        public int Y;
    }

    private struct Transform
    {
        public Position Position;
        public Rotation Rotation;
        public Scale Scale;
    }

    [Fact]
    public void ComponentType_SizeOf_ReturnsCorrectSize() =>
        Assert.Equal(24, ComponentsMeta.GetComponentType(ComponentMeta<Transform>.s_id)._sizeOf);

    [Fact]
    public void CreateStorage_WithT_ReturnsComponentStorageOfT()
    {
        ComponentStorage componentStorage = ComponentsMeta
            .GetComponentType(ComponentMeta<Position>.s_id)
            .CreateStorage(10);

        Assert.IsType<ComponentStorage<Position>>(componentStorage);
        Assert.IsAssignableFrom<ComponentStorage>(componentStorage);
    }
}
#pragma warning restore CS0649
