namespace Theia.ECS.Relations;

internal abstract class Relation
{
    internal readonly RelationCardinality _cardinality;
    internal readonly RelationSubtype _subtype;

    internal Relation(RelationCardinality cardinality, RelationSubtype subtype)
    {
        _cardinality = cardinality;
        _subtype = subtype;
    }

    internal abstract void Reset();
}
