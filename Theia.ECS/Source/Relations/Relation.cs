using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal abstract class Relation
{
    internal readonly RelationCardinality _cardinality;
    internal readonly RelationSubtype _subtype;

    protected Entity _owner;

    internal Lock _relationLock = new();
    protected Lock _updateLock = new();

    protected int _updateCount;

    protected void IncrementUpdateCount() => Interlocked.Increment(ref _updateCount);

    protected void DecrementUpdateCount() => Interlocked.Decrement(ref _updateCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity GetOwner() => _owner;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetOwner(Entity owner) => _owner = owner;

    internal Relation(RelationCardinality cardinality, RelationSubtype subtype)
    {
        _cardinality = cardinality;
        _subtype = subtype;
    }

    internal virtual void Reset()
    {
        _owner = default;
        _updateCount = 0;
    }

    internal abstract void Update(UpdateRelation update);

    protected void ThrowIfUpdating()
    {
        if (_updateCount > 0)
            throw new InvalidOperationException(
                "Cannot perform structural changes to a Relation while an Update is in progress. Use deferred commands to schedule Relate or Disrelate calls."
            );
    }
}
