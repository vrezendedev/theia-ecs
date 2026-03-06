using System;
using System.Threading.Tasks;
using Theia.ECS.Components;

namespace Theia.ECS.Tests.Components;

#pragma warning disable CS0649
public class ComponentMetaTests
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

    private struct Velocity
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

    private struct NonBlittable
    {
        public string Name;
    }

    private struct ForTaskA { }

    private struct ForTaskB { }

    private struct ForTaskC { }

    private struct ForTaskD { }

    private struct ForTaskE { }

    [Fact]
    public void Constructor_WithDifferentComponent_AssignNewId()
    {
        int first = ComponentMeta.s_count;
        int second = first + 1;

        int id1 = ComponentMeta<Position>.s_id;
        int id2 = ComponentMeta<Velocity>.s_id;

        Assert.Equal(first, id1);
        Assert.Equal(second, id2);
    }

    [Fact]
    public void Constructor_WithComponent_SetCorrectSize() =>
        Assert.Equal(24, ComponentMeta<Transform>.s_size);

    [Fact]
    public void Constructor_WithNonBlittableComponent_ThrowsTypeInitializationException()
    {
        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() =>
        {
            _ = ComponentMeta<NonBlittable>.s_id;
        });

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public async Task Constructor_WhenInitializedConcurrently_AreDistinct()
    {
        int[] results = await Task.WhenAll(
            [
                Task.Run(() => ComponentMeta<ForTaskA>.s_id),
                Task.Run(() => ComponentMeta<ForTaskB>.s_id),
                Task.Run(() => ComponentMeta<ForTaskC>.s_id),
                Task.Run(() => ComponentMeta<ForTaskD>.s_id),
                Task.Run(() => ComponentMeta<ForTaskE>.s_id),
            ]
        );

        Assert.Distinct(results);
    }

    [Fact]
    public void ComponentMeta_WithSameComponent_ReturnsSameId()
    {
        int id1 = ComponentMeta<Position>.s_id;
        int id2 = ComponentMeta<Position>.s_id;

        Assert.Equal(id1, id2);
    }
}
#pragma warning restore CS0649
