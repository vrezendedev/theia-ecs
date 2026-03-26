using System;

namespace Theia.ECS.Relations;

internal abstract class RelationKey
{
    protected const int InvalidKey = -1;

    internal abstract void Reset();
}

internal sealed class OneToOneKey : RelationKey
{
    internal int _primaryKey;

    internal override void Reset()
    {
        _primaryKey = InvalidKey;
    }
}

internal sealed class OneToManyKey : RelationKey
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

internal sealed class ManyToManyKey : RelationKey
{
    internal int _primaryKey;
    internal int[]? _compositeKeys;

    internal override void Reset()
    {
        _primaryKey = InvalidKey;

        if (_compositeKeys is not null)
            _compositeKeys.AsSpan().Fill(InvalidKey);
    }
}
