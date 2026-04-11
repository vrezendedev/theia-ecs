using System;

namespace Theia.ECS.Relations;

internal sealed class RelationType<TRelation> : RelationType
    where TRelation : struct
{
    public RelationType()
        : base() { }

    internal RelationType(Type type, bool isTag)
        : base(type, isTag) { }

    internal override Relation CreateRelation()
    {
        Relation? relation;

        lock (_relationPoolLock)
        {
            _relationPool.TryDequeue(out relation);
        }

        if (relation is null)
            relation = _isTag ? new Relation() : new EvaluatedRelation<TRelation>();
        else
            relation.Reset();

        return relation;
    }
}
