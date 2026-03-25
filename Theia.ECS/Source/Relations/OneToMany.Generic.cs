using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal class OneToMany<TRelation> : RelationMany
    where TRelation : struct
{
    private Entity _fromRoot;
    private int _fromRootRelationIndex;

    internal OneToMany()
        : base() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetRoot(Entity root, int index)
    {
        _fromRoot = root;
        _fromRootRelationIndex = index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsRoot() => _fromRoot == default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity From() => _fromRoot;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetFromRelationIndex() => _fromRootRelationIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<Entity> To() => _relatedTo.AsSpan(0, _count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
        base.Reset();
        _fromRoot = default;
    }
}
