using System;
using Theia.ECS.Reflection.Types;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Reflection.Types;

#region Primitives
public sealed partial class BlittableMetaTests
{
    [Fact]
    public void IsStrictlyBlittable_PrimitiveBool_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(bool)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveByte_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(byte)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveChar_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(char)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveDecimal_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(decimal)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveDouble_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(double)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveFloat_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(float)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveInt_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(int)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveUInt_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(uint)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveNInt_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(nint)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveNUint_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(nuint)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveLong_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(long)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveULong_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(ulong)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveShort_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(short)));

    [Fact]
    public void IsStrictlyBlittable_PrimitiveUShort_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(ushort)));
}
#endregion

#region Enums, Reference, ByRef, Pointer and Generic Types
public sealed partial class BlittableMetaTests
{
    [Fact]
    public void IsStrictlyBlittable_ReferenceInterface_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(INonBlittableInterface)));

    [Fact]
    public void IsStrictlyBlittable_ReferenceClass_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(NonBlittableClass)));

    [Fact]
    public void IsStrictlyBlittable_ReferenceDelegate_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(NonBlittableDelegate)));

    [Fact]
    public void IsStrictlyBlittable_ReferenceRecord_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(NonBlittableRecord)));

    [Fact]
    public void IsStrictlyBlittable_ReferenceRecordStruct_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(BlittableRecordStruct)));

    [Fact]
    public void IsStrictlyBlittable_ReferenceString_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(string)));

    [Fact]
    public void IsStrictlyBlittable_GenericTypeDefinition_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(GenericStruct<>)));

    [Fact]
    public void IsStrictlyBlittable_GenericStructWithValueType_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(GenericStruct<int>)));

    [Fact]
    public void IsStrictlyBlittable_GenericStructWithReferenceType_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(GenericStruct<NonBlittableClass>)));

    [Fact]
    public void IsStrictlyBlittable_GenericStructWithValueTypes_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(GenericStruct<int, int>)));

    [Fact]
    public void IsStrictlyBlittable_GenericStructWithValueAndReferenceTypes_ReturnsFalse() =>
        Assert.False(
            BlittableMeta.IsStrictlyBlittable(typeof(GenericStruct<int, NonBlittableClass>))
        );

    [Fact]
    public void IsStrictlyBlittable_Pointer_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(int*)));

    [Fact]
    public void IsStrictlyBlittable_RefStruct_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(NonBlittableRefStruct)));

    [Fact]
    public void IsStrictlyBlittable_Enum_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(EnumA)));
}
#endregion


#region Struct with Attributes
public sealed partial class BlittableMetaTests
{
    [Fact]
    public void IsStrictlyBlittable_StructsWithStructLayoutAuto_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(NonBlittableStructWithAutoLayout)));
}
#endregion

#region Structs with Fields
public sealed partial class BlittableMetaTests
{
    [Fact]
    public void IsStrictlyBlittable_StructWithPrimitiveBlittableFields_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(StructWithPrimitiveBlittableFields)));

    [Fact]
    public void IsStrictlyBlittable_StructWithPrimitiveNonBlittableFields_ReturnsFalse() =>
        Assert.False(
            BlittableMeta.IsStrictlyBlittable(typeof(StructWithPrimitiveNonBlittableFields))
        );

    [Fact]
    public void IsStrictlyBlittable_StructWithBlittableFields_ReturnsTrue() =>
        Assert.True(BlittableMeta.IsStrictlyBlittable(typeof(StructWithBlittableFields)));

    [Fact]
    public void IsStrictlyBlittable_StructWithNonBlittableFields_ReturnsFalse() =>
        Assert.False(BlittableMeta.IsStrictlyBlittable(typeof(StructWithNonBlittableFields)));

    [Fact]
    public void IsStrictlyBlittable_StructWithBlittableGenericsPrimitives_ReturnsTrue() =>
        Assert.True(
            BlittableMeta.IsStrictlyBlittable(typeof(StructWithBlittableGenerics<int, int>))
        );

    [Fact]
    public void IsStrictlyBlittable_StructWithBlittableGenericsStructs_ReturnsTrue() =>
        Assert.True(
            BlittableMeta.IsStrictlyBlittable(
                typeof(StructWithBlittableGenerics<
                    StructWithPrimitiveBlittableFields,
                    StructWithBlittableGenerics<TimeSpan, TimeSpan>
                >)
            )
        );

    [Fact]
    public void IsStrictlyBlittable_StructWithNonBlittableGenerics_ReturnsFalse() =>
        Assert.False(
            BlittableMeta.IsStrictlyBlittable(typeof(StructWithBlittableGenerics<object, object>))
        );
}
#endregion
