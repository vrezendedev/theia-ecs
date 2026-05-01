using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Relations;

namespace Theia.ECS.Events;

/// <summary>
/// Per-<see cref="Worlds.World">World</see> hub for relation-lifecycle events. Mirrors
/// <see cref="EntitiesEvents"/>'s shape: broad subscriptions for "any relation added/removed"
/// plus per-relation-type subscriptions.
/// </summary>
/// <remarks>
/// <para>
/// <b>Events are attachment points for fixed behavior, not temporary subscriptions.</b> Wire
/// handlers once at world construction to declare "this is what happens when X occurs"; the
/// only teardown mechanism is <see cref="Reset"/>, which clears the entire wiring at once.
/// </para>
/// <para>
/// Events fire after the relation has been applied or unwound in world state. <b>Subscribers run
/// synchronously on the calling thread</b>.
/// </para>
/// </remarks>
public sealed class RelationsEvents
{
    /// <summary>Fires when any relation of any type is added between two entities. <br/> Use <see cref="SubscribeOnRelationAdded"/> for per-relation-type filtering.</summary>
    public event Action<RelationModified>? OnAnyRelationAdded;

    /// <summary>Fires when any relation of any type is removed between two entities. <br/> Use <see cref="SubscribeOnRelationRemoved"/> for per-relation-type filtering.</summary>
    public event Action<RelationModified>? OnAnyRelationRemoved;
    private RelationEvents[] _relationEvents = Array.Empty<RelationEvents>();

    /// <summary>
    /// Subscribes <paramref name="onRelationAdded"/> to additions of <typeparamref name="TRelation"/>
    /// specifically, regardless of which entities are linked.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubscribeOnRelationAdded<TRelation>(Action<RelationModified> onRelationAdded)
        where TRelation : struct =>
        GetOrCreateRelationEvents<TRelation>()._onRelationAdded += onRelationAdded;

    /// <summary>
    /// Subscribes <paramref name="onRelationRemoved"/> to removals of <typeparamref name="TRelation"/>
    /// specifically, regardless of which entities are linked.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubscribeOnRelationRemoved<TRelation>(Action<RelationModified> onRelationRemoved)
        where TRelation : struct =>
        GetOrCreateRelationEvents<TRelation>()._onRelationRemoved += onRelationRemoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnRelationAdded(RelationModified relationModified)
    {
        OnAnyRelationAdded?.Invoke(relationModified);

        if (HasEvents(relationModified._relationId))
            _relationEvents[relationModified._relationId].InvokeOnRelationAdded(relationModified);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnRelationRemoved(RelationModified relationModified)
    {
        OnAnyRelationRemoved?.Invoke(relationModified);

        if (HasEvents(relationModified._relationId))
            _relationEvents[relationModified._relationId].InvokeOnRelationRemoved(relationModified);
    }

    private RelationEvents GetOrCreateRelationEvents<TRelation>()
        where TRelation : struct
    {
        int relationId = RelationMeta<TRelation>.s_id;

        if (relationId >= _relationEvents.Length)
            Array.Resize(ref _relationEvents, relationId + 1);

        if (_relationEvents[relationId] is null)
            _relationEvents[relationId] = new RelationEvents();

        return _relationEvents[relationId];
    }

    private bool HasEvents(int relationId) =>
        relationId < _relationEvents.Length && _relationEvents[relationId] is not null;

    /// <summary>Detaches every subscriber from every event on this hub, including all per-relation buckets.</summary>
    public void Reset()
    {
        OnAnyRelationAdded = null;
        OnAnyRelationRemoved = null;

        foreach (RelationEvents relationEvents in _relationEvents)
            relationEvents?.Reset();
    }
}

/// <summary>
/// Per-relation-type bucket holding the typed add and remove subscriptions. Created lazily by
/// <see cref="RelationsEvents"/> on first subscription for that relation.
/// </summary>
internal sealed class RelationEvents
{
    internal event Action<RelationModified>? _onRelationAdded;
    internal event Action<RelationModified>? _onRelationRemoved;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnRelationAdded(RelationModified relationModified) =>
        _onRelationAdded?.Invoke(relationModified);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnRelationRemoved(RelationModified relationModified) =>
        _onRelationRemoved?.Invoke(relationModified);

    internal void Reset()
    {
        _onRelationAdded = null;
        _onRelationRemoved = null;
    }
}
