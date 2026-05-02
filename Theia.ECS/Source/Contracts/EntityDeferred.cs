using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

/// <summary>
/// Deferred-creation record for a single-component assemblage. Pairs the component payload
/// with an optional <see cref="AddRelationDeferred"/> describing a relation that should
/// link an existing owner to the newly created entity once the flush materializes it.
/// </summary>
internal readonly struct EntityCreateDeferred<ComponentT1>
    where ComponentT1 : struct
{
    /// <summary>
    /// The pending relation, or <see cref="AddRelationDeferred.Invalid"/> if no relation
    /// was requested. The <c>_target</c> field is left as <c>default</c> at queue time and
    /// patched with the newly created entity's handle during flush.
    /// </summary>
    internal readonly AddRelationDeferred _relationDeferred { get; init; }

    /// <summary>The component payload to write into the new entity's storage during flush.</summary>
    internal readonly required ComponentT1 _componentT1 { get; init; }

    public EntityCreateDeferred() => _relationDeferred = AddRelationDeferred.Invalid;
}

/// <summary>
/// Pending component-mutation record carrying just the entity and component identity. The
/// associated value, when one is needed, lives in the matching
/// <see cref="Components.ComponentDeferredStorage{TComponent}"/> queue and is correlated by
/// FIFO position.
/// </summary>
internal readonly struct EntityComponentDeferred
{
    internal readonly required Entity _entity { get; init; }
    internal readonly required int _componentId { get; init; }
}

/// <summary>
/// Public input shape for "create this entity and link an existing owner to it via a relation."
/// Carries the existing owner and the relation payload; the target is supplied implicitly by
/// the assemblage when the new entity materializes.
/// </summary>
/// <typeparam name="TRelation">The relation type to add. May be a tag (fieldless) or a data relation.</typeparam>
public readonly struct DeferredRelationOnCreate<TRelation>
    where TRelation : struct
{
    /// <summary>The existing entity that will own the relation.</summary>
    public readonly required Entity Owner { get; init; }

    /// <summary>
    /// The relation payload. For tag relations this carries no meaningful data and may be left
    /// at <see langword="default"/>; for data relations this is the per-link value.
    /// </summary>
    public readonly TRelation Relation { get; init; }
}

/// <summary>
/// Fully-resolved pending relation-add record. Carries the metadata needed to apply the
/// relation at flush time without any further lookups: the relation's runtime ID, its storage
/// slot within the relation system, and whether the relation is a tag.
/// </summary>
/// <remarks>
/// Resolution happens once at queue time (typically in <c>World.GetAddRelationDeferred</c>),
/// so the flush-time work is a single dictionary-free dispatch through this record. The
/// <see cref="Invalid"/> sentinel is used as the default for <see cref="EntityCreateDeferred{ComponentT1}"/>
/// when no relation is requested.
/// </remarks>
internal readonly record struct AddRelationDeferred
{
    /// <summary>Sentinel relation ID indicating an empty/invalid record.</summary>
    internal const int InvalidRelationId = -1;

    /// <summary>Sentinel relation storage index indicating an empty/invalid record.</summary>
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

    /// <summary>Sentinel value used to mark "no pending relation" in records that carry an optional <see cref="AddRelationDeferred"/>.</summary>
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

/// <summary>
/// Pending relation-remove record. Lighter than <see cref="AddRelationDeferred"/> because
/// removal does not need the storage index or tag flag; the relation's location is rediscovered
/// at flush time.
/// </summary>
internal readonly record struct RemoveRelationDeferred
{
    /// <summary>Sentinel relation ID indicating an empty/invalid record.</summary>
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
