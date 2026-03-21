using System;
using Theia.ECS.Components;
using Theia.ECS.Enums;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Enums;

public sealed class ComponentEnumTests
{
    [Fact]
    public void FromComponent_EnumWithNoAttributes_ReturnsDefault() =>
        Assert.Equal(default, ComponentEnum<NoAttributesEnum>.FromComponent(0));

    [Fact]
    public void FromComponent_IncludesEnum_FirstIncludedComponent_ReturnsMappedValue() =>
        Assert.Equal(
            IncludesEnum.Group,
            ComponentEnum<IncludesEnum>.FromComponent(ComponentMeta<IncludesComponentA>.s_id)
        );

    [Fact]
    public void FromComponent_IncludesEnum_SecondIncludedComponent_ReturnsMappedValue() =>
        Assert.Equal(
            IncludesEnum.Group,
            ComponentEnum<IncludesEnum>.FromComponent(ComponentMeta<IncludesComponentB>.s_id)
        );

    [Fact]
    public void FromComponent_IncludesEnum_UnmappedComponent_ReturnsDefault() =>
        Assert.Equal(
            default,
            ComponentEnum<IncludesEnum>.FromComponent(ComponentMeta<UnmappedComponent>.s_id)
        );

    [Fact]
    public void FromComponent_IncludesEnum_OutOfBoundsId_ReturnsDefault() =>
        Assert.Equal(default, ComponentEnum<IncludesEnum>.FromComponent(int.MaxValue));

    [Fact]
    public void FromComponent_IncludesEnum_RegistersComponentsOnInitialization()
    {
        _ = ComponentEnum<IncludesOnlyEnum>.FromComponent(0);

        Assert.Equal(
            IncludesOnlyEnum.Group,
            ComponentEnum<IncludesOnlyEnum>.FromComponent(
                ComponentMeta<IncludesOnlyComponentA>.s_id
            )
        );
    }

    [Fact]
    public void FromComponent_MatchesEnum_ReturnsMappedValue() =>
        Assert.Equal(
            MatchesEnum.ComponentA,
            ComponentEnum<MatchesEnum>.FromComponent(ComponentMeta<MatchesComponentA>.s_id)
        );

    [Fact]
    public void FromComponent_MatchesEnum_UnmappedComponent_ReturnsDefault() =>
        Assert.Equal(
            default,
            ComponentEnum<MatchesEnum>.FromComponent(ComponentMeta<UnmappedComponent>.s_id)
        );

    [Fact]
    public void FromComponent_MatchesEnum_OutOfBoundsId_ReturnsDefault() =>
        Assert.Equal(default, ComponentEnum<MatchesEnum>.FromComponent(int.MaxValue));

    [Fact]
    public void FromComponent_MixedAttributesEnum_ThrowsInvalidOperationException()
    {
        TypeInitializationException ex = Assert.Throws<TypeInitializationException>(() =>
            ComponentEnum<MixedAttributesEnum>.FromComponent(0)
        );

        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }
}
