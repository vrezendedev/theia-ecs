using Theia.ECS.Entities;

namespace Theia.ECS.Queries;

public interface IForEachEntity<ComponentT1>
    where ComponentT1 : struct
{
    public void Execute(Entity entity, ref ComponentT1 componentT1);
}

public interface IForEach<ComponentT1>
    where ComponentT1 : struct
{
    public void Execute(ref ComponentT1 componentT1);
}
