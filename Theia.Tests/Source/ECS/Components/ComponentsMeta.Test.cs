using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Theia.ECS.Components;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Components;

public sealed class ComponentsMetaTests
{
    [Fact]
    public void Constructor_WithNonBlittableComponent_ThrowsTypeInitializationException()
    {
        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() =>
        {
            _ = ComponentMeta<NonBlittableComponent>.s_id;
        });

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void Constructor_WithEmptyStructComponent_ThrowsTypeInitializationException()
    {
        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() =>
        {
            _ = ComponentMeta<EmptyComponent>.s_id;
        });

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void ComponentMeta_WithSameComponent_ReturnsSameId()
    {
        int id1 = ComponentMeta<ComponentA>.s_id;
        int id2 = ComponentMeta<ComponentA>.s_id;

        Assert.Equal(id1, id2);
    }

    [Fact]
    public void ComponentMeta_WithDifferentComponent_ReturnsDifferentId()
    {
        int id1 = ComponentMeta<ComponentA>.s_id;
        int id2 = ComponentMeta<ComponentB>.s_id;

        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void ComponentMeta_WithSameGenericComponentType_ReturnsSameId()
    {
        int id1 = ComponentMeta<GenericComponent<ComponentA>>.s_id;
        int id2 = ComponentMeta<GenericComponent<ComponentA>>.s_id;

        Assert.Equal(id1, id2);
    }

    [Fact]
    public void ComponentMeta_WithDifferentGenericComponentType_ReturnsDifferentId()
    {
        int id1 = ComponentMeta<GenericComponent<ComponentA>>.s_id;
        int id2 = ComponentMeta<GenericComponent<ComponentC>>.s_id;

        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public async Task RegisterComponent_WhenComponentsInitializedConcurrently_AreDistinct()
    {
        int[] results = await Task.WhenAll(
            [
                Task.Run(() => ComponentMeta<ComponentA>.s_id),
                Task.Run(() => ComponentMeta<ComponentB>.s_id),
                Task.Run(() => ComponentMeta<ComponentC>.s_id),
                Task.Run(() => ComponentMeta<ComponentD>.s_id),
                Task.Run(() => ComponentMeta<ComponentE>.s_id),
            ]
        );

        Assert.Distinct(results);
    }

    [Fact]
    public void GetComponentType_ValidIndex_ReturnsComponentType()
    {
        int id = ComponentMeta<ComponentA>.s_id;
        Assert.NotNull(ComponentsMeta.GetComponentType(id));
    }

    [Fact]
    public void RegisterComponent_WithType_ReturnsIndexOfValidComponentType()
    {
        int id = ComponentsMeta.RegisterComponent(typeof(ViaTypeComponent));

        ComponentType componentType = ComponentsMeta.GetComponentType(id);

        Assert.Equal(typeof(ViaTypeComponent), componentType.Get());
        Assert.Equal(Unsafe.SizeOf<ViaTypeComponent>(), componentType._sizeOf);
        Assert.NotNull(componentType);
        Assert.IsType<ComponentType<ViaTypeComponent>>(componentType);

        Storage storage = componentType.CreateStorage(1);

        Assert.IsType<Storage<ViaTypeComponent>>(storage);

        int idViaGeneric = ComponentMeta<ViaTypeComponent>.s_id;

        Assert.Equal(id, idViaGeneric);
    }
}
