using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

internal readonly struct EntityCreateDeferred<ComponentT1>
{
    internal readonly required ComponentT1 _componentT1 { get; init; }
}

internal readonly struct EntityComponentDeferred
{
    internal readonly required Entity _entity { get; init; }
    internal readonly required int _componentId { get; init; }
}
