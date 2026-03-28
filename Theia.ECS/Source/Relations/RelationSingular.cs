using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal class Singular : Relation
{
    protected Entity _target;

    internal Singular(RelationCardinality cardinality, RelationSubtype subtype)
        : base(cardinality, subtype) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Relate(Entity a)
    {
        ThrowIfUpdating();

        _target = a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Disrelate()
    {
        ThrowIfUpdating();

        Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity To() => _target;

    internal override void Update(UpdateRelation updateRelation)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            updateRelation(_target);
            DecrementUpdateCount();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
        base.Reset();

        _target = default;
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

    internal void Update(UpdateRelation<TRelation> update)
    {
        IncrementUpdateCount();

        lock (_updateLock)
        {
            update(_target, ref _data);
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
