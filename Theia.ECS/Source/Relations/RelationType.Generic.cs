using System;

namespace Theia.ECS.Relations;

internal sealed class RelationType<TRelation> : RelationType
    where TRelation : struct
{
    internal RelationType(Type type, RelationSubtype subtype)
        : base(type, subtype) { }

    internal override Relation CreateRelation()
    {
        Relation? relation;

        lock (_relationPoolLock)
        {
            _relationPool.TryDequeue(out relation);
        }

        if (relation is null)
#pragma warning disable CS8524
            relation = _subtype switch
            {
                RelationSubtype.Tag => new TagRelation(),
                RelationSubtype.Evaluated => new EvaluatedRelation<TRelation>(),
            };
#pragma warning restore CS8524

        else
            relation.Reset();

        return relation;
    }
}
