using System;
using Theia.ECS.Relations;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Relations;

public sealed class RelationsMetaTests
{
    private static RelationType<TRelation> BuildRelationType<TRelation>(
        RelationCardinality cardinality,
        RelationSubtype subtype
    )
        where TRelation : struct =>
        new RelationType<TRelation>(typeof(TRelation), cardinality, subtype);

    [Fact]
    public void ContainsRelationsAttributes_WithExclusiveAttribute_ReturnsTrue() =>
        Assert.True(RelationsMeta.ContainsRelationsAttributes<ExclusiveTag>());

    [Fact]
    public void ContainsRelationsAttributes_WithTreeAttribute_ReturnsTrue() =>
        Assert.True(RelationsMeta.ContainsRelationsAttributes<TreeTag>());

    [Fact]
    public void ContainsRelationsAttributes_WithMultipleAttribute_ReturnsTrue() =>
        Assert.True(RelationsMeta.ContainsRelationsAttributes<MultipleTag>());

    [Fact]
    public void ContainsRelationsAttributes_WithoutAttribute_ReturnsFalse() =>
        Assert.False(RelationsMeta.ContainsRelationsAttributes<NoAttributeRelation>());

    [Fact]
    public void IsTag_WithStructWithNoFields_ReturnsTrue() =>
        Assert.True(RelationsMeta.IsTag<ExclusiveTag>());

    [Fact]
    public void IsTag_WithStructWithFields_ReturnsFalse() =>
        Assert.False(RelationsMeta.IsTag<ExclusiveData>());

    [Fact]
    public void ValidateRelation_WithExclusiveAttribute_ReturnsExclusiveCardinality()
    {
        RelationCardinality cardinality = RelationsMeta.ValidateRelation<ExclusiveTag>();
        Assert.Equal(RelationCardinality.Exclusive, cardinality);
    }

    [Fact]
    public void ValidateRelation_WithTreeAttribute_ReturnsTreeCardinality()
    {
        RelationCardinality cardinality = RelationsMeta.ValidateRelation<TreeTag>();
        Assert.Equal(RelationCardinality.Tree, cardinality);
    }

    [Fact]
    public void ValidateRelation_WithMultipleAttribute_ReturnsMultipleCardinality()
    {
        RelationCardinality cardinality = RelationsMeta.ValidateRelation<MultipleTag>();
        Assert.Equal(RelationCardinality.Multiple, cardinality);
    }

    [Fact]
    public void ValidateRelation_WithoutCardinalityAttribute_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() =>
            RelationsMeta.ValidateRelation<NoAttributeRelation>()
        );

    [Fact]
    public void ValidateRelation_WithMultipleCardinalityAttributes_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() =>
            RelationsMeta.ValidateRelation<MultipleCardinalitiesRelation>()
        );

    [Fact]
    public void RelationMeta_AssignsNonNegativeId_ReturnsNonNegative() =>
        Assert.True(RelationMeta<ExclusiveTag>.s_id >= 0);

    [Fact]
    public void RelationMeta_WithSameType_ReturnsSameId()
    {
        int first = RelationMeta<TreeTag>.s_id;
        int second = RelationMeta<TreeTag>.s_id;
        Assert.Equal(first, second);
    }

    [Fact]
    public void RelationMeta_WithDifferentTypes_ReturnsDifferentIds() =>
        Assert.NotEqual(RelationMeta<ExclusiveTag>.s_id, RelationMeta<MultipleTag>.s_id);

    [Fact]
    public void GetRelationId_WithRegisteredType_ReturnsMatchingId()
    {
        int expected = RelationMeta<ExclusiveData>.s_id;
        int actual = RelationsMeta.GetRelationId(typeof(ExclusiveData));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetRelationType_WithValidId_ReturnsCorrectType()
    {
        int id = RelationMeta<TreeData>.s_id;
        RelationType meta = RelationsMeta.GetRelationType(id);
        Assert.Equal(typeof(TreeData), meta.Get());
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
    public void RelationsMeta_Count_AfterNewRegistration_IncreasesOrStaysSame()
    {
        int before = RelationsMeta.Count();
        _ = RelationMeta<MultipleData>.s_id;
        int after = RelationsMeta.Count();

        Assert.True(after >= before);
    }

    [Fact]
    public void CreateRelation_WithExclusiveTag_ReturnsExclusiveRelation()
    {
        RelationType<ExclusiveTag> relationType = BuildRelationType<ExclusiveTag>(
            RelationCardinality.Exclusive,
            RelationSubtype.Tag
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Singular>(relation);
    }

    [Fact]
    public void CreateRelation_WithTreeTag_ReturnsTreeRelation()
    {
        RelationType<TreeTag> relationType = BuildRelationType<TreeTag>(
            RelationCardinality.Tree,
            RelationSubtype.Tag
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many>(relation);
    }

    [Fact]
    public void CreateRelation_WithMultipleTag_ReturnsMultipleRelation()
    {
        RelationType<MultipleTag> relationType = BuildRelationType<MultipleTag>(
            RelationCardinality.Multiple,
            RelationSubtype.Tag
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many>(relation);
    }

    [Fact]
    public void CreateRelation_WithExclusiveData_ReturnsExclusiveStore()
    {
        RelationType<ExclusiveData> relationType = BuildRelationType<ExclusiveData>(
            RelationCardinality.Exclusive,
            RelationSubtype.Data
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Singular<ExclusiveData>>(relation);
    }

    [Fact]
    public void CreateRelation_WithTreeData_ReturnsTreeStore()
    {
        RelationType<TreeData> relationType = BuildRelationType<TreeData>(
            RelationCardinality.Tree,
            RelationSubtype.Data
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many<TreeData>>(relation);
    }

    [Fact]
    public void CreateRelation_WithMultipleData_ReturnsMultipleStore()
    {
        RelationType<MultipleData> relationType = BuildRelationType<MultipleData>(
            RelationCardinality.Multiple,
            RelationSubtype.Data
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many<MultipleData>>(relation);
    }

    [Fact]
    public void CreateRelation_AfterPooling_ReturnsSameInstance()
    {
        RelationType<ExclusiveTag> relationType = BuildRelationType<ExclusiveTag>(
            RelationCardinality.Exclusive,
            RelationSubtype.Tag
        );

        Relation created = relationType.CreateRelation();
        relationType.PoolRelation(created);

        Relation fromPool = relationType.CreateRelation();
        Assert.Same(created, fromPool);
    }

    [Fact]
    public void CreateRelation_WithEmptyPool_ReturnsNewInstance()
    {
        RelationType<ExclusiveTag> relationType = BuildRelationType<ExclusiveTag>(
            RelationCardinality.Exclusive,
            RelationSubtype.Tag
        );

        Relation first = relationType.CreateRelation();
        Relation second = relationType.CreateRelation();

        Assert.NotSame(first, second);
    }
}
