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

public enum SingleKey : int
{
    A = 0,
}

public enum TwoKeys : int
{
    A = 0,
    B = 1,
}

public enum SparseKey : int
{
    A = 0,
    B = 100,
}

public enum ByteKey : byte
{
    A = 0,
}

public enum MatchesRelationEnum
{
    None,

    [Matches<Friend>]
    Friend,
}

public enum MatchesMultipleRelationEnum
{
    None,

    [Matches<Friend>]
    Friend,

    [Matches<Damage>]
    Damage,
}

public enum IncludesRelationOnlyEnum
{
    None,

    [Includes<Friend>]
    Group,
}
