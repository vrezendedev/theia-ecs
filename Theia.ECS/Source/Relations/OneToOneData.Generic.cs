using System.Runtime.CompilerServices;
using Theia.ECS.Relations;

internal sealed class OneToOneData<TRelation> : OneToOne<TRelation>
    where TRelation : struct
{
    private TRelation _data;

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
