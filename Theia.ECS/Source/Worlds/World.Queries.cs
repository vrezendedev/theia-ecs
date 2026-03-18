using System;
using Theia.ECS.Queries;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private int _queriesBeingExecuted;

    private NomadQuery[] _nomadQueries;
    private SettlerQuery[] _settlerQueries;

    internal bool AreThereAnyQueriesBeingExecuted() => _queriesBeingExecuted > 0;

    internal void ThrowIfQueriesExecuting()
    {
        if (AreThereAnyQueriesBeingExecuted())
            throw new InvalidOperationException(
                "World structural changes are not permitted while queries are being executed. Defer modifications."
            );
    }
}
