using Theia.ECS.Relations;

internal sealed class ManyToManyStore<TRelation> : ManyToMany<TRelation>
    where TRelation : struct { }
