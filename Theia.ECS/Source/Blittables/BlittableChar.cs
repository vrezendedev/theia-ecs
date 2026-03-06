using System;
using System.Runtime.InteropServices;

namespace Theia.ECS.Blittables;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct BlittableChar : IEquatable<BlittableChar>
{
    public char Value;

    public bool Equals(BlittableChar other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is BlittableChar && Equals((BlittableChar)obj);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => $"{nameof(BlittableChar)}(Value: {Value})";

    public static bool operator ==(BlittableChar left, BlittableChar right) => left.Equals(right);

    public static bool operator !=(BlittableChar left, BlittableChar right) => !left.Equals(right);

    public static implicit operator BlittableChar(char value) =>
        new BlittableChar { Value = value };

    public static implicit operator char(BlittableChar value) => value.Value;
}
