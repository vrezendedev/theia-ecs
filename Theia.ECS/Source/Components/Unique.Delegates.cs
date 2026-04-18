namespace Theia.ECS.Components;

public interface IUniqueQuery<TComponent>
    where TComponent : struct
{
    public void Execute(ref TComponent component);
}
