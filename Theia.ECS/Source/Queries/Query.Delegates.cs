using Theia.ECS.Entities;

namespace Theia.ECS.Queries;

public interface IForEachEntity<ComponentT1>
    where ComponentT1 : struct
{
    void Execute(Entity entity, ref ComponentT1 c1);
}

public interface IForEach<ComponentT1>
    where ComponentT1 : struct
{
    void Execute(ref ComponentT1 c1);
}
