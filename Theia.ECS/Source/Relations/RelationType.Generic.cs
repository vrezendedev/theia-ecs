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
                (RelationCardinality.OneToOne, RelationSubtype.Tag) => new Singular(
                    cardinality,
                    subtype
                ),
                (RelationCardinality.OneToOne, RelationSubtype.Data) => new Singular<TRelation>(
                    cardinality,
                    subtype
                ),
                (RelationCardinality.OneToMany, RelationSubtype.Tag)
                or (RelationCardinality.ManyToMany, RelationSubtype.Tag) => new Many(
                    cardinality,
                    subtype
                ),
                (RelationCardinality.OneToMany, RelationSubtype.Data)
                or (RelationCardinality.ManyToMany, RelationSubtype.Data) => new Many<TRelation>(
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
                RelationCardinality.OneToOne => new OneToOneKey(),
                RelationCardinality.OneToMany => new OneToManyKey(),
                RelationCardinality.ManyToMany => new ManyToManyKey(),
            };
#pragma warning restore CS8524
        }
        else
            relationKey.Reset();

        return relationKey;
    }
}
