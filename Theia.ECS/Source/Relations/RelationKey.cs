namespace Theia.ECS.Relations;

internal abstract class RelationKey { }

internal sealed class OneToOneKey : RelationKey
{
    internal int _primaryKey;
}

internal sealed class OneToManyKey : RelationKey
{
    internal int _primaryKey;

    internal int _foreignKey;
    internal int _compositeKey;
}

internal sealed class ManyToManyKey : RelationKey
{
    internal int _primaryKey;
    internal int[]? _compositeKeys;
}
