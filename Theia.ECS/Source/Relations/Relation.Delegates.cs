using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

public interface IQueryRelation
{
    public void Execute(Entity other);
}

public interface IQueryEvaluatedRelation<TRelation>
    where TRelation : struct
{
    public void Execute(Entity other, ref TRelation relation);
}
