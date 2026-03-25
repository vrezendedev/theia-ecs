using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Relations;

internal sealed class OneToManyData<TRelation> : OneToMany<TRelation>
    where TRelation : struct
{
    private TRelation[] _data;

    internal OneToManyData()
        : base() => _data = new TRelation[1];

    protected override void Resize(int currentLength)
    {
        base.Resize(currentLength);
        Array.Resize(ref _data, currentLength * DefaultRelationGrowthFactor);
    }

    protected override void Swap(int from, int to)
    {
        base.Swap(from, to);
        _data[to] = _data[from];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<TRelation> Read() => _data.AsSpan(0, _count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TRelation Get(int index) => ref _data[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, in TRelation data) => _data[index] = data;

    internal override void Reset()
    {
        _data.AsSpan(0, _count).Fill(default);
        base.Reset();
    }
}
