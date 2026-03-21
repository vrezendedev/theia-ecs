using Theia.ECS.Enums.Attributes;

namespace Theia.Tests.Resources;

public enum EnumA : int
{
    A,
    B,
}

public enum NoAttributesEnum
{
    None,
}

public enum IncludesEnum
{
    [Includes<IncludesComponentA>]
    [Includes<IncludesComponentB>]
    Group,
}

public enum MatchesEnum
{
    [Matches<MatchesComponentA>]
    ComponentA,
}

public enum IncludesOnlyEnum
{
    [Includes<IncludesOnlyComponentA>]
    Group,
}

public enum MixedAttributesEnum
{
    [Includes<IncludesComponentA>]
    Group,

    [Matches<MatchesComponentA>]
    ComponentA,
}
