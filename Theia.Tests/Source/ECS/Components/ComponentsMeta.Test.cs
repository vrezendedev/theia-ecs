using System;
using System.Threading.Tasks;
using Theia.ECS.Components;

namespace Theia.ECS.Tests.Components;

#pragma warning disable CS0649
public class ComponentsMetaTests
{
    private struct NonBlittableComponent
    {
        public string Name;
    }

    private struct GenericComponent<T>
    {
        public T Value;
    }

    private struct ComponentA
    {
        public int A;
    }

    private struct ComponentB
    {
        public int B;
    }

    private struct ComponentC
    {
        public int C;
    }

    private struct ComponentD
    {
        public int D;
    }

    private struct ComponentE
    {
        public int E;
    }

    private struct ComponentF
    {
        public int F;
    }

    private struct ComponentG
    {
        public int G;
    }

    private struct ComponentH
    {
        public int H;
    }

    private struct ComponentI
    {
        public int I;
    }

    private struct ComponentJ
    {
        public int J;
    }

    private struct InvalidComponent { }

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
            _ = ComponentMeta<InvalidComponent>.s_id;
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
    public async Task RegisterComponent_WhenCountEqualsLength_Resize()
    {
        int initialCapacity = ComponentsMeta.GetComponentTypeMapCapacity();

        int id = ComponentMeta<ComponentA>.s_id;
        id = ComponentMeta<ComponentB>.s_id;
        id = ComponentMeta<ComponentC>.s_id;
        id = ComponentMeta<ComponentD>.s_id;
        id = ComponentMeta<ComponentE>.s_id;
        id = ComponentMeta<ComponentF>.s_id;
        id = ComponentMeta<ComponentG>.s_id;
        id = ComponentMeta<ComponentH>.s_id;
        id = ComponentMeta<ComponentI>.s_id;
        id = ComponentMeta<ComponentJ>.s_id;
        id = ComponentMeta<GenericComponent<ComponentA>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentB>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentC>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentD>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentE>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentF>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentG>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentH>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentI>>.s_id;
        id = ComponentMeta<GenericComponent<ComponentJ>>.s_id;

        Assert.True(initialCapacity != ComponentsMeta.GetComponentTypeMapCapacity());
    }

    [Fact]
    public void GetComponentType_InvalidIndex_ThrowsIndexOutOfRangeException() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            ComponentsMeta.GetComponentType(ComponentsMeta.s_count + 1)
        );

    [Fact]
    public void GetComponentType_ValidIndex_ReturnsComponentType()
    {
        int id = ComponentMeta<ComponentA>.s_id;
        Assert.NotNull(ComponentsMeta.GetComponentType(id));
    }
}
#pragma warning restore CS0649
