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

[Exclusive]
internal struct ExclusiveTag { }

[Tree]
internal struct TreeTag { }

[Multiple]
internal struct MultipleTag { }

[Exclusive]
internal struct ExclusiveData
{
    public int Value;
}

[Tree]
internal struct TreeData
{
    public int Value;
}

[Multiple]
internal struct MultipleData
{
    public int Value;
}

internal struct NoAttributeRelation { }

[Exclusive]
[Tree]
internal struct MultipleCardinalitiesRelation { }

[Exclusive]
internal struct NonBlittableRelation
{
    public string ManagedField;
}

#pragma warning restore CS0649
