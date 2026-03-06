using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Blittables;

public struct BlittableBoolean : IEquatable<BlittableBoolean>
{
    private byte byteValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Get() => Convert.ToBoolean(byteValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(bool value) => byteValue = Convert.ToByte(value);

    public bool Equals(BlittableBoolean other) => byteValue == other.byteValue;

    public override bool Equals(object? obj) => obj is BlittableBoolean other && Equals(other);

    public override int GetHashCode() => byteValue.GetHashCode();

    public override string ToString() => $"{nameof(BlittableBoolean)}(byteValue: {byteValue})";

    public static bool operator ==(BlittableBoolean left, BlittableBoolean right) =>
        left.Equals(right);

    public static bool operator !=(BlittableBoolean left, BlittableBoolean right) =>
        !left.Equals(right);

    public static implicit operator BlittableBoolean(bool value) =>
        new BlittableBoolean() { byteValue = Convert.ToByte(value) };

    public static implicit operator bool(BlittableBoolean value) => value.byteValue != 0;
}
