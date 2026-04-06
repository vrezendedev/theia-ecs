using Theia.ECS.Entities;

namespace Theia.ECS.Queries;

public delegate void ForEachEntity<ComponentT1>(Entity entity, ref ComponentT1 componentT1)
    where ComponentT1 : struct;

public delegate void ForEach<ComponentT1>(ref ComponentT1 componentT1)
    where ComponentT1 : struct;
