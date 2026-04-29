#pragma warning disable CS0649
using System.Runtime.InteropServices;
using Theia.ECS.Blittables;
using Theia.ECS.Relations.Attributes;

namespace Theia.Tests.Resources;

public struct NonBlittableComponent
{
    public string NonBlittableField;
}

public struct EmptyComponent { }

public struct GenericComponent<T>
{
    public T GenericField;
}

public struct ComponentA
{
    public int A;
}

public struct ComponentB
{
    public int B;
}

public struct ComponentC
{
    public int C;
}

public struct ComponentD
{
    public int D;
}

public struct ComponentE
{
    public int E;
}

public struct ComponentF
{
    public int F;
}

public struct ComponentG
{
    public int G;
}

public struct ComponentH
{
    public int H;
}

public struct ComponentI
{
    public int I;
}

public struct ComponentJ
{
    public int J;
}

public struct ViaTypeComponent
{
    public int Value;
}

[Relationship]
public struct ViaTypeTagRelation { }

[Relationship]
public struct ViaTypeEvaluatedRelation
{
    public int Value;
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Velocity
{
    public int X;
    public int Y;
}

public struct Rotation
{
    public int X;
    public int Y;
}

public struct Health
{
    public int CurrentHealth;
    public int MaxHealth;
}

public struct Scale
{
    public int Value;
}

public struct Transform
{
    public Position Position;
    public Rotation Rotation;
    public Scale Scale;
}

public struct GenericStruct<T> { }

public struct GenericStruct<T, TT> { }

public record struct BlittableRecordStruct { }

public ref struct NonBlittableRefStruct { }

[StructLayout(LayoutKind.Auto)]
public struct NonBlittableStructWithAutoLayout { }

public struct StructWithPrimitiveBlittableFields
{
    public int Int;
    public byte Byte;
}

public struct StructWithStaticFields
{
    public static int Int;
}

public struct StructWithPrimitiveNonBlittableFields
{
    public int? Int;
    public byte Byte;
}

public struct StructWithBlittableFields
{
    public int Int;
    public BlittableBoolean Boolean;
}

public struct StructWithNonBlittableFields
{
    public int Int;
    public string String;
}

public struct StructWithBlittableGenerics<T, TT>
{
    public T ValueT;
    public TT ValueTT;
}

public struct MatchesComponentA
{
    public int Value;
}

public struct MatchesComponentB
{
    public int Value;
}

public struct IncludesComponentA
{
    public int Value;
}

public struct IncludesComponentB
{
    public int Value;
}

public struct IncludesOnlyComponentA
{
    public int Value;
}

public struct UnmappedComponent
{
    public int Value;
}

[Relationship]
public struct TaggedRelation { }

[Relationship]
public struct EvaluatedRelation
{
    public int Value;
}

[Relationship]
public struct NonBlittableRelation
{
    public string Value;
}

[Relationship]
public struct Friend { }

[Relationship]
public struct Damage
{
    public float Value;
}

public struct Mass
{
    public float Value;
}

public struct Age
{
    public int Value;
}

public struct Tag
{
    public int Value;
}

public struct Color
{
    public byte R;
    public byte G;
    public byte B;
}

public struct Force
{
    public float X;
    public float Y;
}

public struct Momentum
{
    public float X;
    public float Y;
}

public struct Gravity
{
    public float Value;
}

public struct Friction
{
    public float Value;
}

public struct Torque
{
    public float Value;
}

public struct Impulse
{
    public float X;
    public float Y;
}

public struct Damping
{
    public float Value;
}

[Relationship]
public struct ParentOf { }

[Relationship]
public struct Friendship { }

[Relationship]
public struct LinkWeight
{
    public float Value;
}


#pragma warning restore CS0649
