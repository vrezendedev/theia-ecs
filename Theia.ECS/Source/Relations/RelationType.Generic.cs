using System;

namespace Theia.ECS.Relations;

internal sealed class RelationType<TRelation> : RelationType
{
    internal RelationType(Type type, RelationCardinality constraint)
        : base(type, constraint) { }

    internal override Relation CreateRelation()
    {
        _pool.TryDequeue(out Relation? relation);

        if (relation is null)
#pragma warning disable CS8524
            relation = _cardinality switch
            {
                RelationCardinality.OneToMany => new OneToMany<TRelation>(),
                RelationCardinality.ManyToMany => new ManyToMany<TRelation>(),
            };
#pragma warning restore CS8524

        return relation;
    }
}
