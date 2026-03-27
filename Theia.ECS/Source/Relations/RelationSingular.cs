using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal class Singular : Relation
{
    protected Entity _a;
    protected Entity _b;

    internal Singular(RelationCardinality cardinality, RelationSubtype subtype)
        : base(cardinality, subtype) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Relate(Entity a, Entity b)
    {
        ThrowIfUpdating();

        _a = a;
        _b = b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Disrelate()
    {
        ThrowIfUpdating();

        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity To(Entity e) =>
        e == _a ? _b
        : e == _b ? _a
        : default;

    internal void To(Entity e, UpdateRelation updateRelation)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            updateRelation(To(e));
            DecrementUpdateCount();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
        base.Reset();

        _a = default;
        _b = default;
    }
}

internal sealed class Singular<TRelation> : Singular
    where TRelation : struct
{
    private TRelation _data;

    internal Singular(RelationCardinality cardinality, RelationSubtype subtype)
        : base(cardinality, subtype) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TRelation Read() => _data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TRelation Get() => ref _data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(in TRelation data) => _data = data;

    internal void Update(Entity e, UpdateRelation<TRelation> update)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            update(To(e), ref _data);
            DecrementUpdateCount();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
        base.Reset();
        _data = default;
    }
}
