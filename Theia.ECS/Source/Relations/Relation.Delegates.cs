using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

public delegate void QueryRelation(Entity other);
public delegate void QueryRelation<TRelation>(Entity other, ref TRelation relation)
    where TRelation : struct;
