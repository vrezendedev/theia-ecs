using Theia.ECS.Entities;

namespace Theia.ECS.Queries;

public delegate void ForEachEntity<T>(Entity entity, ref T component);
public delegate void ForEach<T>(ref T component);
