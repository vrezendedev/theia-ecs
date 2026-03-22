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
    None,

    [Includes<IncludesComponentA>]
    [Includes<IncludesComponentB>]
    Group,
}

public enum MatchesEnum
{
    None,

    [Matches<MatchesComponentA>]
    ComponentA,

    [Matches<MatchesComponentB>]
    ComponentB,
}

public enum IncludesOnlyEnum
{
    None,

    [Includes<IncludesOnlyComponentA>]
    Group,
}

public enum MixedAttributesEnum
{
    None,

    [Includes<IncludesComponentA>]
    Group,

    [Matches<MatchesComponentA>]
    ComponentA,
}

public enum ManagedDataKey : int
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    Unregistered = 99,
}

public enum ByteKeyEnum : byte
{
    A,
}
