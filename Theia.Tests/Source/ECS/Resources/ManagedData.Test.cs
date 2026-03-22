using System;
using Theia.ECS.Resources;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Resources;

public sealed class ManagedDataTests
{
    [Fact]
    public void ManagedData_InvalidEnumUnderlyingType_ThrowsInvalidOperationException()
    {
        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() =>
            ManagedData<ByteKeyEnum, ManagedDataResource>.Register(
                ByteKeyEnum.A,
                Array.Empty<ManagedDataResource>()
            )
        );

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void Register_ValidKeyAndData_DoesNotThrow()
    {
        ManagedData<ManagedDataKey, ManagedDataResource>.Register(
            ManagedDataKey.A,
            [new() { Value = 1 }]
        );
    }

    [Fact]
    public void Register_AlreadyRegisteredKey_ThrowsInvalidOperationException()
    {
        ManagedData<ManagedDataKey, ManagedDataResource>.Register(
            ManagedDataKey.B,
            [new() { Value = 1 }]
        );

        Assert.Throws<InvalidOperationException>(() =>
            ManagedData<ManagedDataKey, ManagedDataResource>.Register(
                ManagedDataKey.B,
                [new() { Value = 2 }]
            )
        );
    }

    [Fact]
    public void Register_MultipleKeys_StoresIndependently()
    {
        ManagedDataResource[] dataC = [new() { Value = 1 }];
        ManagedDataResource[] dataD = [new() { Value = 2 }];

        ManagedData<ManagedDataKey, ManagedDataResource>.Register(ManagedDataKey.C, dataC);
        ManagedData<ManagedDataKey, ManagedDataResource>.Register(ManagedDataKey.D, dataD);

        Assert.Equal(
            1,
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.C, 0).Value
        );
        Assert.Equal(
            2,
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.D, 0).Value
        );
    }

    [Fact]
    public void Get_Array_RegisteredKey_ReturnsArray()
    {
        ManagedDataResource[] data = [new() { Value = 1 }, new() { Value = 2 }];

        ManagedData<ManagedDataKey, ManagedDataResource>.Register(ManagedDataKey.E, data);

        ManagedDataResource[] result = ManagedData<ManagedDataKey, ManagedDataResource>.Get(
            ManagedDataKey.E
        );

        Assert.Equal(data, result);
    }

    [Fact]
    public void Get_Array_UnregisteredKey_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() =>
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.Unregistered)
        );

    [Fact]
    public void Get_Single_RegisteredKeyAndIndex_ReturnsCorrectValue()
    {
        ManagedDataResource[] data = [new() { Value = 10 }, new() { Value = 20 }];

        ManagedData<ManagedDataKey, ManagedDataResource>.Register(ManagedDataKey.F, data);

        Assert.Equal(
            10,
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.F, 0).Value
        );
        Assert.Equal(
            20,
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.F, 1).Value
        );
    }

    [Fact]
    public void Get_Single_UnregisteredKey_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() =>
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.Unregistered, 0)
        );

    [Fact]
    public void Set_Array_RegisteredKey_ReplacesArray()
    {
        ManagedDataResource[] original = [new() { Value = 1 }];
        ManagedDataResource[] replacement = [new() { Value = 99 }];

        ManagedData<ManagedDataKey, ManagedDataResource>.Register(ManagedDataKey.G, original);
        ManagedData<ManagedDataKey, ManagedDataResource>.Set(ManagedDataKey.G, replacement);

        Assert.Equal(
            99,
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.G, 0).Value
        );
    }

    [Fact]
    public void Set_Array_UnregisteredKey_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() =>
            ManagedData<ManagedDataKey, ManagedDataResource>.Set(
                ManagedDataKey.Unregistered,
                Array.Empty<ManagedDataResource>()
            )
        );

    [Fact]
    public void Set_Single_RegisteredKeyAndIndex_SetsValue()
    {
        ManagedDataResource[] data = [new() { Value = 1 }];

        ManagedData<ManagedDataKey, ManagedDataResource>.Register(ManagedDataKey.H, data);
        ManagedData<ManagedDataKey, ManagedDataResource>.Set(
            ManagedDataKey.H,
            0,
            new ManagedDataResource { Value = 99 }
        );

        Assert.Equal(
            99,
            ManagedData<ManagedDataKey, ManagedDataResource>.Get(ManagedDataKey.H, 0).Value
        );
    }

    [Fact]
    public void Set_Single_UnregisteredKey_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() =>
            ManagedData<ManagedDataKey, ManagedDataResource>.Set(
                ManagedDataKey.Unregistered,
                0,
                new ManagedDataResource()
            )
        );
}
