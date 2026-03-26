using Theia.ECS.Entities;

namespace Theia.ECS.Queries;

public delegate void ForEachEntity<TComponent>(Entity entity, ref TComponent component)
    where TComponent : struct;
public delegate void ForEach<TComponent>(ref TComponent component)
    where TComponent : struct;
