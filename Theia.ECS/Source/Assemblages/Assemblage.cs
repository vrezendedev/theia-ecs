using System;
using Theia.ECS.Components;

namespace Theia.ECS.Assemblages;

public abstract class Assemblage
{
    internal readonly int _worldId;
    internal readonly Signature _signature;
    internal readonly int _matchedArchetype;
    internal readonly int[] _componentStorageMapping;

    internal Assemblage() { }

    internal ReadOnlySpan<int> ComponentStorageMapping() => _componentStorageMapping.AsSpan();
}
