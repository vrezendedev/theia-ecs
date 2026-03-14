using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Queue<Action> _deferredCommands;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Defer(Action action) => _deferredCommands.Enqueue(action);
}
