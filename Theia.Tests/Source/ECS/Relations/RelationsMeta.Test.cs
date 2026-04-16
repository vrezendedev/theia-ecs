using System;
using Theia.ECS.Relations;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Relations;

public sealed class RelationsMetaTests
{
    [Fact]
    public void IsTag_WithStructWithNoFields_ReturnsTrue() =>
        Assert.True(RelationsMeta.IsTag<TaggedRelation>());

    [Fact]
    public void IsTag_WithStructWithFields_ReturnsFalse() =>
        Assert.False(RelationsMeta.IsTag<EvaluatedRelation>());

    [Fact]
    public void RelationMeta_AssignsNonNegativeId_ReturnsNonNegative() =>
        Assert.True(RelationMeta<TaggedRelation>.s_id >= 0);

    [Fact]
    public void RelationMeta_WithSameType_ReturnsSameId()
    {
        int first = RelationMeta<TaggedRelation>.s_id;
        int second = RelationMeta<TaggedRelation>.s_id;
        Assert.Equal(first, second);
    }

    [Fact]
    public void RelationMeta_WithDifferentTypes_ReturnsDifferentIds() =>
        Assert.NotEqual(RelationMeta<TaggedRelation>.s_id, RelationMeta<EvaluatedRelation>.s_id);

    [Fact]
    public void GetRelationId_WithRegisteredType_ReturnsMatchingId()
    {
        int expected = RelationMeta<EvaluatedRelation>.s_id;
        int actual = RelationsMeta.GetRelationId(typeof(EvaluatedRelation));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetRelationType_WithValidId_ReturnsCorrectType()
    {
        int id = RelationMeta<EvaluatedRelation>.s_id;
        RelationType meta = RelationsMeta.GetRelationType(id);
        Assert.Equal(typeof(EvaluatedRelation), meta.Get());
    }

    [Fact]
    public void RelationMeta_WithNonBlittableRelation_ThrowsInvalidOperationException()
    {
        Exception ex = Assert.Throws<TypeInitializationException>(() =>
            _ = RelationMeta<NonBlittableRelation>.s_id
        );

        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public void CreateRelation_AfterPooling_ReturnsSameInstance()
    {
        RelationType<TaggedRelation> relationType =
            (RelationType<TaggedRelation>)
                RelationsMeta.GetRelationType(RelationMeta<TaggedRelation>.s_id);

        Relation created = relationType.CreateRelation();

        relationType.PoolRelation(created);

        Relation fromPool = relationType.CreateRelation();

        Assert.Same(created, fromPool);
    }

    [Fact]
    public void CreateRelation_WithEmptyPool_ReturnsNewInstance()
    {
        RelationType<TaggedRelation> relationType =
            (RelationType<TaggedRelation>)
                RelationsMeta.GetRelationType(RelationMeta<TaggedRelation>.s_id);

        Relation first = relationType.CreateRelation();
        Relation second = relationType.CreateRelation();

        Assert.NotSame(first, second);
    }
}
