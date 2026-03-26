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
    public void ContainsRelationsAttributes_WithOneToOneAttribute_ReturnsTrue() =>
        Assert.True(RelationsMeta.ContainsRelationsAttributes<OneToOneTag>());

    [Fact]
    public void ContainsRelationsAttributes_WithOneToManyAttribute_ReturnsTrue() =>
        Assert.True(RelationsMeta.ContainsRelationsAttributes<OneToManyTag>());

    [Fact]
    public void ContainsRelationsAttributes_WithManyToManyAttribute_ReturnsTrue() =>
        Assert.True(RelationsMeta.ContainsRelationsAttributes<ManyToManyTag>());

    [Fact]
    public void ContainsRelationsAttributes_WithoutAttribute_ReturnsFalse() =>
        Assert.False(RelationsMeta.ContainsRelationsAttributes<NoAttributeRelation>());

    [Fact]
    public void IsTag_WithStructWithNoFields_ReturnsTrue() =>
        Assert.True(RelationsMeta.IsTag<OneToOneTag>());

    [Fact]
    public void IsTag_WithStructWithFields_ReturnsFalse() =>
        Assert.False(RelationsMeta.IsTag<OneToOneData>());

    [Fact]
    public void ValidateRelation_WithOneToOneAttribute_ReturnsOneToOneCardinality()
    {
        RelationCardinality cardinality = RelationsMeta.ValidateRelation<OneToOneTag>();
        Assert.Equal(RelationCardinality.OneToOne, cardinality);
    }

    [Fact]
    public void ValidateRelation_WithOneToManyAttribute_ReturnsOneToManyCardinality()
    {
        RelationCardinality cardinality = RelationsMeta.ValidateRelation<OneToManyTag>();
        Assert.Equal(RelationCardinality.OneToMany, cardinality);
    }

    [Fact]
    public void ValidateRelation_WithManyToManyAttribute_ReturnsManyToManyCardinality()
    {
        RelationCardinality cardinality = RelationsMeta.ValidateRelation<ManyToManyTag>();
        Assert.Equal(RelationCardinality.ManyToMany, cardinality);
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
        Assert.True(RelationMeta<OneToOneTag>.s_id >= 0);

    [Fact]
    public void RelationMeta_WithSameType_ReturnsSameId()
    {
        int first = RelationMeta<OneToManyTag>.s_id;
        int second = RelationMeta<OneToManyTag>.s_id;
        Assert.Equal(first, second);
    }

    [Fact]
    public void RelationMeta_WithDifferentTypes_ReturnsDifferentIds() =>
        Assert.NotEqual(RelationMeta<OneToOneTag>.s_id, RelationMeta<ManyToManyTag>.s_id);

    [Fact]
    public void GetRelationId_WithRegisteredType_ReturnsMatchingId()
    {
        int expected = RelationMeta<OneToOneData>.s_id;
        int actual = RelationsMeta.GetRelationId(typeof(OneToOneData));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetRelationType_WithValidId_ReturnsCorrectType()
    {
        int id = RelationMeta<OneToManyData>.s_id;
        RelationType meta = RelationsMeta.GetRelationType(id);
        Assert.Equal(typeof(OneToManyData), meta.Get());
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
        _ = RelationMeta<ManyToManyData>.s_id;
        int after = RelationsMeta.Count();

        Assert.True(after >= before);
    }

    [Fact]
    public void CreateRelation_WithOneToOneTag_ReturnsOneToOneRelation()
    {
        RelationType<OneToOneTag> relationType = BuildRelationType<OneToOneTag>(
            RelationCardinality.OneToOne,
            RelationSubtype.Tag
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Singular>(relation);
    }

    [Fact]
    public void CreateRelation_WithOneToManyTag_ReturnsOneToManyRelation()
    {
        RelationType<OneToManyTag> relationType = BuildRelationType<OneToManyTag>(
            RelationCardinality.OneToMany,
            RelationSubtype.Tag
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many>(relation);
    }

    [Fact]
    public void CreateRelation_WithManyToManyTag_ReturnsManyToManyRelation()
    {
        RelationType<ManyToManyTag> relationType = BuildRelationType<ManyToManyTag>(
            RelationCardinality.ManyToMany,
            RelationSubtype.Tag
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many>(relation);
    }

    [Fact]
    public void CreateRelation_WithOneToOneData_ReturnsOneToOneStore()
    {
        RelationType<OneToOneData> relationType = BuildRelationType<OneToOneData>(
            RelationCardinality.OneToOne,
            RelationSubtype.Data
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Singular<OneToOneData>>(relation);
    }

    [Fact]
    public void CreateRelation_WithOneToManyData_ReturnsOneToManyStore()
    {
        RelationType<OneToManyData> relationType = BuildRelationType<OneToManyData>(
            RelationCardinality.OneToMany,
            RelationSubtype.Data
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many<OneToManyData>>(relation);
    }

    [Fact]
    public void CreateRelation_WithManyToManyData_ReturnsManyToManyStore()
    {
        RelationType<ManyToManyData> relationType = BuildRelationType<ManyToManyData>(
            RelationCardinality.ManyToMany,
            RelationSubtype.Data
        );

        Relation relation = relationType.CreateRelation();

        Assert.IsType<Many<ManyToManyData>>(relation);
    }

    [Fact]
    public void CreateRelation_AfterPooling_ReturnsSameInstance()
    {
        RelationType<OneToOneTag> relationType = BuildRelationType<OneToOneTag>(
            RelationCardinality.OneToOne,
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
        RelationType<OneToOneTag> relationType = BuildRelationType<OneToOneTag>(
            RelationCardinality.OneToOne,
            RelationSubtype.Tag
        );

        Relation first = relationType.CreateRelation();
        Relation second = relationType.CreateRelation();

        Assert.NotSame(first, second);
    }
}
