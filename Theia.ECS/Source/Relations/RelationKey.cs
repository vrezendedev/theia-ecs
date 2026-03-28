using System;

namespace Theia.ECS.Relations;

internal abstract class RelationKey
{
    protected const int InvalidKey = -1;

    internal abstract void Reset();

    protected void ResetKeys<T>(ref T[]? keys, T invalid)
        where T : struct
    {
        if (keys is not null)
            keys.AsSpan().Fill(invalid);
    }
}

internal sealed class ExclusiveKey : RelationKey
{
    internal int _primaryKey;

    internal int[]? _foreignKeys; //Points out to the relation itself -> size of relations

    internal override void Reset()
    {
        _primaryKey = InvalidKey;

        ResetKeys(ref _foreignKeys, InvalidKey);
    }
}

internal sealed class TreeKey : RelationKey
{
    internal int _primaryKey;

    internal int _foreignKey;
    internal int _compositeKey;

    internal override void Reset()
    {
        _primaryKey = InvalidKey;
        _foreignKey = InvalidKey;
        _compositeKey = InvalidKey;
    }
}

internal record struct MultipleKeyIndexer(int ForeignKeys, int CompositeKeys);

internal sealed class MultipleKey : RelationKey
{
    internal int _primaryKey;

    internal MultipleKeyIndexer[]? _keys; //Points out to the relation itself -> size of relations

    internal override void Reset()
    {
        _primaryKey = InvalidKey;

        ResetKeys(ref _keys, new(InvalidKey, InvalidKey));
    }
}
