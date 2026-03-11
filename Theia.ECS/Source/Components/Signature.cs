using System;

namespace Theia.ECS.Components;

internal readonly struct Signature
{
    internal readonly int Length;
    internal readonly int SizeOf;
    internal readonly int[] Components;

    internal Signature(ReadOnlySpan<int> componentsIds)
    {
        Length = componentsIds.Length;

        int size = 0;

        for (int i = 0; i < componentsIds.Length; i++)
            size += ComponentsMeta.GetComponentType(componentsIds[i])._sizeOf;

        SizeOf = size;

        Components = componentsIds.ToArray();
    }

    internal ReadOnlySpan<int> Values() => Components;
}
