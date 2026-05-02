using Theia.ECS.Relations;

namespace Theia.ECS.Contracts;

/// <summary>
/// Result of <see cref="RelationStorage.RentRelation"/>: pairs the freshly rented
/// <see cref="Relation"/> instance with the primary key locating it in storage. The caller uses
/// the relation directly and writes the primary key into the owner's <see cref="RelationsIndexer"/>.
/// </summary>
internal readonly ref struct RelationKeyed
{
    internal readonly Relation _relation;
    internal readonly int _primaryKey;

    internal RelationKeyed(Relation relation, int primaryKey)
    {
        _relation = relation;
        _primaryKey = primaryKey;
    }
}
