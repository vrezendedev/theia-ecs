using Theia.ECS.Blittables;

namespace Theia.Tests.ECS.Blittables;

public sealed class BlittableBooleanTests
{
    [Fact]
    public void Equals_WithSameByte_ReturnsTrue()
    {
        BlittableBoolean booleanA = new();
        BlittableBoolean booleanB = new();

        Assert.True(booleanA.Equals(booleanB));
    }

    [Fact]
    public void Equals_WithDifferentByte_ReturnsFalse()
    {
        BlittableBoolean booleanA = new();
        BlittableBoolean booleanB = new();

        booleanB.Set(true);

        Assert.False(booleanA.Equals(booleanB));
    }

    [Fact]
    public void GetHashCode_WithSameByte_ShouldBeEqual()
    {
        BlittableBoolean booleanA = new() { };
        BlittableBoolean booleanB = new() { };

        Assert.Equal(booleanA.GetHashCode(), booleanB.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentByte_ShouldBeDifferent()
    {
        BlittableBoolean booleanA = new();
        BlittableBoolean booleanB = new();

        booleanB.Set(true);

        Assert.NotEqual(booleanA.GetHashCode(), booleanB.GetHashCode());
    }

    [Fact]
    public void Implicit_FromBool_ReturnsTrue()
    {
        BlittableBoolean boolean = true;
        Assert.True(boolean);
    }

    [Fact]
    public void Implicit_FromBool_ReturnsFalse()
    {
        BlittableBoolean boolean = false;
        Assert.False(boolean);
    }

    [Fact]
    public void Implicit_FromBlittable_ReturnsTrue()
    {
        BlittableBoolean blittableBoolean = new BlittableBoolean();
        blittableBoolean.Set(true);

        bool boolean = blittableBoolean;
        Assert.True(boolean);
    }

    [Fact]
    public void Implicit_FromBlittable_ReturnsFalse()
    {
        BlittableBoolean blittableBoolean = new BlittableBoolean();

        bool boolean = blittableBoolean;
        Assert.False(boolean);
    }
}
