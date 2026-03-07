using System;
using Theia.ECS.Components;

namespace Theia.ECS.Archetypes;

internal sealed class Archetype
{
    private const int DefaultComponentStorageCapacity = 4;

    private ComponentStorage[][] _storages;
    private int[] _componentStorageMapping;

    internal Archetype(ReadOnlySpan<int> indexes)
    {
        throw new NotImplementedException();
    }

    internal void Resize()
    {
        throw new NotImplementedException();
    }
}
