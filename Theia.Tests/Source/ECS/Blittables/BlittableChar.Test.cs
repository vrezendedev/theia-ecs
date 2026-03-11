using Theia.ECS.Blittables;

namespace Theia.Tests.ECS.Blittables;

public sealed class BlittableCharTests
{
    [Fact]
    public void Equals_WithSameChar_ReturnsTrue()
    {
        BlittableChar charA = new();
        BlittableChar charB = new();

        Assert.True(charA.Equals(charB));
    }

    [Fact]
    public void Equals_WithDifferentChar_ReturnsFalse()
    {
        BlittableChar charA = new();
        BlittableChar charB = new();

        charA.Set('a');
        charB.Set('b');

        Assert.False(charA.Equals(charB));
    }

    [Fact]
    public void GetHashCode_WithSameChar_ShouldBeEqual()
    {
        BlittableChar charA = new() { };
        BlittableChar charB = new() { };

        Assert.Equal(charA.GetHashCode(), charB.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentChar_ShouldBeDifferent()
    {
        BlittableChar charA = new();
        BlittableChar charB = new();

        charA.Set('a');
        charB.Set('b');

        Assert.NotEqual(charA.GetHashCode(), charB.GetHashCode());
    }

    [Fact]
    public void Implicit_FromChar_ShouldCreateBlittableChar()
    {
        char input = 'a';

        BlittableChar result = input;

        char output = result;
        Assert.Equal(input, output);
    }

    [Fact]
    public void Implicit_ToChar_ShouldReturnUnderlyingValue()
    {
        BlittableChar value = 'b';

        char result = value;

        Assert.Equal('b', result);
    }
}
