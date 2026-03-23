using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal sealed class EntityRelation
{
    private const int DefaulChildrenCapacity = 4;
    private const int DefaultChildrenGrowthFactor = 2;

    private Entity _parent;
    private int _indexAsChild;

    private int _childrenCount;
    private Entity[] _children;

    private int _childQueriesBeingExecuted;

    internal EntityRelation()
    {
        _childrenCount = 0;
        _children = Array.Empty<Entity>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity GetParent() => _parent;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetParent(Entity entity)
    {
        ThrowIfHasParent();

        _parent = entity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ResetParent() => _parent = default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetIndexAsChild() => _indexAsChild;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetIndexAsChild(int indexAsChild) => _indexAsChild = indexAsChild;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int AddChild(Entity child)
    {
        ThrowIfChildQueriesBeingExecuted();

        int currentLength = _children.Length;

        if (_childrenCount == _children.Length)
            Resize(currentLength);

        int index = _childrenCount;

        _children[index] = child;

        _childrenCount++;

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity RemoveChild(int childIndex)
    {
        ThrowIfChildQueriesBeingExecuted();

        Entity swapped = default;
        int last = _childrenCount - 1;

        if (childIndex < last)
        {
            Entity lastEntity = _children[last];

            _children[childIndex] = lastEntity;
            swapped = lastEntity;
        }

        _childrenCount--;

        return swapped;
    }

    private void IncrementChildQueriesBeingExecuted() =>
        Interlocked.Increment(ref _childQueriesBeingExecuted);

    private void DecrementChildQueriesBeingExecuted() =>
        Interlocked.Decrement(ref _childQueriesBeingExecuted);

    private bool AreThereChildQueriesBeingExecuted() =>
        Volatile.Read(ref _childQueriesBeingExecuted) > 0;

    internal void ForEachChild(ForEachChild forEachChild)
    {
        int childrenCount = _childrenCount;

        if (childrenCount == 0)
            return;

        IncrementChildQueriesBeingExecuted();

        ReadOnlySpan<Entity> entities = _children.AsSpan(0, childrenCount);

        for (int i = 0; i < childrenCount; i++)
            forEachChild(entities[i]);

        DecrementChildQueriesBeingExecuted();
    }

    internal void Reset()
    {
        _parent = default;
        _childrenCount = 0;
        _children.AsSpan().Fill(default);
    }

    private void Resize(int currentLength)
    {
        int length =
            currentLength == 0
                ? DefaulChildrenCapacity
                : currentLength * DefaultChildrenGrowthFactor;

        Array.Resize(ref _children, length);
    }

    private void ThrowIfHasParent()
    {
        if (_parent != default)
            throw new InvalidOperationException(
                "Entity already has a parent. Remove the existing parent before assigning a new one."
            );
    }

    private void ThrowIfChildQueriesBeingExecuted()
    {
        if (AreThereChildQueriesBeingExecuted())
            throw new InvalidOperationException(
                "Cannot modify children while iterating. Complete the iteration before adding or removing children."
            );
    }
}
