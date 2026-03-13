using Theia.ECS.Queries;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private int _queriesBeingExecuted;

    private NomadQuery[] _nomadQueries;
    private SettlerQuery[] _settlerQueries;
}
