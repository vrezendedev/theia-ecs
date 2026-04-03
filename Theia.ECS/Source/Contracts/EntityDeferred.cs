using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

internal readonly struct EntityCreateDeferred<ComponentT1>
    where ComponentT1 : struct
{
    internal readonly AddRelationDeferred _relationDeferred { get; init; }
    internal readonly required ComponentT1 _componentT1 { get; init; }

    public EntityCreateDeferred() => _relationDeferred = AddRelationDeferred.Invalid;
}

internal readonly struct EntityComponentDeferred
{
    internal readonly required Entity _entity { get; init; }
    internal readonly required int _componentId { get; init; }
}

public readonly struct DeferredRelationOnCreate<TRelation>
    where TRelation : struct
{
    public readonly required Entity Owner { get; init; }
    public readonly TRelation Relation { get; init; }
}

internal readonly record struct AddRelationDeferred
{
    internal const int InvalidRelationId = -1;
    internal const int InvalidRelationStorageIndex = -1;

    internal readonly required Entity _owner { get; init; }
    internal readonly required Entity _target { get; init; }
    internal readonly required int _relationId { get; init; }
    internal readonly required int _relationStorageIndex { get; init; }
    internal readonly required bool _isTag { get; init; }

    public AddRelationDeferred()
    {
        _owner = default;
        _target = default;
        _relationId = InvalidRelationId;
        _relationStorageIndex = InvalidRelationStorageIndex;
    }

    internal static AddRelationDeferred Invalid =>
        new()
        {
            _owner = default,
            _target = default,
            _relationId = InvalidRelationId,
            _relationStorageIndex = InvalidRelationStorageIndex,
            _isTag = false,
        };
}

internal readonly record struct RemoveRelationDeferred
{
    internal const int InvalidRelationId = -1;
    internal readonly required Entity _owner { get; init; }
    internal readonly required Entity _target { get; init; }
    internal readonly required int _relationId { get; init; }

    public RemoveRelationDeferred()
    {
        _owner = default;
        _target = default;
        _relationId = InvalidRelationId;
    }
}
