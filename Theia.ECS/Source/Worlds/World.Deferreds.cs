using System;
using System.Collections.Generic;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Queue<Action> _deferredCommands;
}
