using System;

namespace Theia.ECS.Relations;

internal sealed class RelationType<TRelation> : RelationType
    where TRelation : struct
{
    internal RelationType(Type type, RelationCardinality constraint, RelationSubtype subtype)
        : base(type, constraint, subtype) { }

    internal override Relation CreateRelation()
    {
        Relation? relation;

        lock (_relationPoolLock)
        {
            _relationPool.TryDequeue(out relation);
        }

        if (relation is null)
        {
            RelationCardinality cardinality = _cardinality;
            RelationSubtype subtype = _subtype;
#pragma warning disable CS8524
            relation = (cardinality, subtype) switch
            {
                (RelationCardinality.Exclusive, RelationSubtype.Tag) => new Singular(
                    cardinality,
                    subtype
                ),
                (RelationCardinality.Exclusive, RelationSubtype.Data) => new Singular<TRelation>(
                    cardinality,
                    subtype
                ),
                (RelationCardinality.Tree, RelationSubtype.Tag)
                or (RelationCardinality.Multiple, RelationSubtype.Tag) => new Many(
                    cardinality,
                    subtype
                ),
                (RelationCardinality.Tree, RelationSubtype.Data)
                or (RelationCardinality.Multiple, RelationSubtype.Data) => new Many<TRelation>(
                    cardinality,
                    subtype
                ),
            };
#pragma warning restore CS8524
        }
        else
            relation.Reset();

        return relation;
    }

    internal override RelationKey CreateKey()
    {
        RelationKey? relationKey;

        lock (_relationKeyPoolLock)
        {
            _relationKeyPool.TryDequeue(out relationKey);
        }

        if (relationKey is null)
        {
#pragma warning disable CS8524
            relationKey = _cardinality switch
            {
                RelationCardinality.Exclusive => new ExclusiveKey(),
                RelationCardinality.Tree => new TreeKey(),
                RelationCardinality.Multiple => new MultipleKey(),
            };
#pragma warning restore CS8524
        }
        else
            relationKey.Reset();

        return relationKey;
    }
}
