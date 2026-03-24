using Theia.ECS.Relations;

internal sealed class OneToManyStore<TRelation> : OneToMany<TRelation>
    where TRelation : struct { }
