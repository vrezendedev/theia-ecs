using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal class Singular : Relation
{
    private Entity _a;
    private Entity _b;

    internal Singular(RelationCardinality cardinality, RelationSubtype subtype)
        : base(cardinality, subtype) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Relate(Entity a, Entity b)
    {
        _a = a;
        _b = b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Disrelate() => Reset();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity To(Entity e) =>
        e == _a ? _b
        : e == _b ? _a
        : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
        base.Reset();
        _data = default;
    }
}
