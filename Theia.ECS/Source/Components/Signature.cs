using System;

namespace Theia.ECS.Components;

internal readonly struct Signature
{
    internal readonly int[] Components;
    internal readonly int Length;

    internal Signature(ReadOnlySpan<int> componentsIds)
    {
        Components = componentsIds.ToArray();
    }

    internal ReadOnlySpan<int> Values() => Components;

    internal int SizeOf() => throw new NotImplementedException();
}
