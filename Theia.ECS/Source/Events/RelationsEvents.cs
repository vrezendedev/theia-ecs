using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Contracts;
using Theia.ECS.Relations;

namespace Theia.ECS.Events;

public sealed class RelationsEvents
{
    public event Action<RelationModified>? OnAnyRelationAdded;
    public event Action<RelationModified>? OnAnyRelationRemoved;
    private RelationEvents[] _relationEvents = Array.Empty<RelationEvents>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubscribeOnRelationAdded<TRelation>(Action<RelationModified> onRelationAdded)
        where TRelation : struct =>
        GetOrCreateRelationEvents<TRelation>()._onRelationAdded += onRelationAdded;

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

    public void Reset()
    {
        OnAnyRelationAdded = null;
        OnAnyRelationRemoved = null;

        foreach (RelationEvents relationEvents in _relationEvents)
            relationEvents?.Reset();
    }
}

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
