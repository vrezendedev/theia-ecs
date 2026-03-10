using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
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

    [InlineArray(10)]
    private struct ComponentWithFixedSizeBuffer()
    {
        public int Element0;
    }

    [Fact]
    public void ComponentType_SizeOf_ReturnsCorrectSize() =>
        Assert.Equal(24, ComponentsMeta.GetComponentType(ComponentMeta<Transform>.s_id)._sizeOf);

    [Fact]
    public void ComponentType_SizeOfComponentWithFixedSizeBuffer_ReturnsCorrectSize() =>
        Assert.Equal(
            40,
            ComponentsMeta
                .GetComponentType(ComponentMeta<ComponentWithFixedSizeBuffer>.s_id)
                ._sizeOf
        );

    [Fact]
    public void CreateStorage_WithT_ReturnsComponentStorageOfT()
    {
        Storage componentStorage = ComponentsMeta
            .GetComponentType(ComponentMeta<Position>.s_id)
            .CreateStorage(10);

        Assert.IsType<Storage<Position>>(componentStorage);
        Assert.IsAssignableFrom<Storage>(componentStorage);
    }
}
#pragma warning restore CS0649
