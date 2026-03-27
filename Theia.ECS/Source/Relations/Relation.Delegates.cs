using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

public delegate void UpdateRelation(Entity other);
public delegate void UpdateRelation<TRelation>(Entity other, ref TRelation relation);
