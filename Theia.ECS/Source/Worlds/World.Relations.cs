using Theia.ECS.Relations;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private RelationsIndexer[] _relationsIndexer;
    private RelationStorage[] _relationStorage;
}
