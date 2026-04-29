using System.Runtime.CompilerServices;
using Theia.ECS.Components;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Components;

public sealed class ComponentTypeTests
{
    [Fact]
    public void ComponentType_SizeOf_ReturnsCorrectSize() =>
        Assert.Equal(20, ComponentsMeta.GetComponentType(ComponentMeta<Transform>.s_id)._sizeOf);

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

[InlineArray(10)]
file struct ComponentWithFixedSizeBuffer()
{
    public int Element0;
}
