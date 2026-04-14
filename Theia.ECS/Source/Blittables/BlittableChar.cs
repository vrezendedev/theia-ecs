using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MessagePack;

namespace Theia.ECS.Blittables;

[MessagePackObject(AllowPrivate = true)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public partial struct BlittableChar : IEquatable<BlittableChar>
{
    [Key(0)]
    private char _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char Get() => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(char value) => _value = value;

    public bool Equals(BlittableChar other) => _value == other._value;

    public override bool Equals(object? obj) => obj is BlittableChar && Equals((BlittableChar)obj);

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => $"{nameof(BlittableChar)}(Value: {_value})";

    public static bool operator ==(BlittableChar left, BlittableChar right) => left.Equals(right);

    public static bool operator !=(BlittableChar left, BlittableChar right) => !left.Equals(right);

    public static implicit operator BlittableChar(char value) =>
        new BlittableChar { _value = value };

    public static implicit operator char(BlittableChar value) => value._value;
}
