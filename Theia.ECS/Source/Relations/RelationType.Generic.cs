using System;

namespace Theia.ECS.Relations;

internal sealed class RelationType<TRelation> : RelationType
    where TRelation : struct
{
    internal RelationType(Type type, RelationCardinality constraint, RelationSubtype subtype)
        : base(type, constraint, subtype) { }

    internal override Relation CreateRelation()
    {
        lock (_poolLock)
        {
            _pool.TryDequeue(out Relation? relation);

            if (relation is null)
#pragma warning disable CS8524
                relation = (_cardinality, _subtype) switch
                {
                    (RelationCardinality.OneToOne, RelationSubtype.Tag) =>
                        new OneToOne<TRelation>(),
                    (RelationCardinality.OneToMany, RelationSubtype.Tag) =>
                        new OneToMany<TRelation>(),
                    (RelationCardinality.ManyToMany, RelationSubtype.Tag) =>
                        new ManyToMany<TRelation>(),
                    (RelationCardinality.OneToOne, RelationSubtype.Data) =>
                        new OneToOneData<TRelation>(),
                    (RelationCardinality.OneToMany, RelationSubtype.Data) =>
                        new OneToManyData<TRelation>(),
                    (RelationCardinality.ManyToMany, RelationSubtype.Data) =>
                        new ManyToManyData<TRelation>(),
                };
#pragma warning restore CS8524

            return relation;
        }
    }
}
