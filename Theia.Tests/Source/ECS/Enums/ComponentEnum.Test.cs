using System;
using Theia.ECS.Components;
using Theia.ECS.Enums;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Enums;

[Collection("MetaRequester")]
public sealed class StructEnumTests
{
    [Fact]
    public void FromStruct_EnumWithNoAttributes_ReturnsDefault() =>
        Assert.Equal(default, StructEnum<NoAttributesEnum, ComponentType>.FromStruct(0));

    [Fact]
    public void FromStruct_IncludesEnum_FirstIncludedComponent_ReturnsMappedValue() =>
        Assert.Equal(
            IncludesEnum.Group,
            StructEnum<IncludesEnum, ComponentType>.FromStruct(
                ComponentMeta<IncludesComponentA>.s_id
            )
        );

    [Fact]
    public void FromStruct_IncludesEnum_SecondIncludedComponent_ReturnsMappedValue() =>
        Assert.Equal(
            IncludesEnum.Group,
            StructEnum<IncludesEnum, ComponentType>.FromStruct(
                ComponentMeta<IncludesComponentB>.s_id
            )
        );

    [Fact]
    public void FromStruct_IncludesEnum_UnmappedComponent_ReturnsDefault() =>
        Assert.Equal(
            default,
            StructEnum<IncludesEnum, ComponentType>.FromStruct(
                ComponentMeta<UnmappedComponent>.s_id
            )
        );

    [Fact]
    public void FromStruct_IncludesEnum_OutOfBoundsId_ReturnsDefault() =>
        Assert.Equal(default, StructEnum<IncludesEnum, ComponentType>.FromStruct(int.MaxValue));

    [Fact]
    public void FromStruct_IncludesEnum_RegistersComponentsOnInitialization()
    {
        _ = StructEnum<IncludesOnlyEnum, ComponentType>.FromStruct(0);

        Assert.Equal(
            IncludesOnlyEnum.Group,
            StructEnum<IncludesOnlyEnum, ComponentType>.FromStruct(
                ComponentMeta<IncludesOnlyComponentA>.s_id
            )
        );
    }

    [Fact]
    public void FromStruct_MatchesEnum_ReturnsMappedValue() =>
        Assert.Equal(
            MatchesEnum.ComponentA,
            StructEnum<MatchesEnum, ComponentType>.FromStruct(ComponentMeta<MatchesComponentA>.s_id)
        );

    [Fact]
    public void FromStruct_MatchesEnum_UnmappedComponent_ReturnsDefault() =>
        Assert.Equal(
            default,
            StructEnum<MatchesEnum, ComponentType>.FromStruct(ComponentMeta<UnmappedComponent>.s_id)
        );

    [Fact]
    public void FromStruct_MatchesEnum_OutOfBoundsId_ReturnsDefault() =>
        Assert.Equal(default, StructEnum<MatchesEnum, ComponentType>.FromStruct(int.MaxValue));

    [Fact]
    public void FromStruct_MixedAttributesEnum_ThrowsInvalidOperationException()
    {
        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() =>
            StructEnum<MixedAttributesEnum, ComponentType>.FromStruct(0)
        );

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }
}
