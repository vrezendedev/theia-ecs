using Theia.ECS.Assemblages;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    public Assemblage<T> CreateAssemblage<T>()
        where T : struct => (Assemblage<T>)CreateAssemblage(stackalloc int[ComponentMeta<T>.s_id]);
}
