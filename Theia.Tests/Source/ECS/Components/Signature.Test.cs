using System;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Components;

#pragma warning disable CS0618
public sealed class SignatureTests
{
    [Fact]
    public void Constructor_WithSingleComponent_LengthIsOne()
    {
        Signature signature = new Signature([ComponentMeta<Position>.s_id]);

        Assert.Equal(1, signature._length);
    }

    [Fact]
    public void Constructor_WithMultipleComponents_LengthMatchesInput()
    {
        Signature signature = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Assert.Equal(2, signature._length);
    }

    [Fact]
    public void Constructor_WithMultipleComponents_MaxIdIsCorrect()
    {
        int posId = ComponentMeta<Position>.s_id;
        int velId = ComponentMeta<Velocity>.s_id;

        Signature signature = new Signature([posId, velId]);

        Assert.Equal(Math.Max(posId, velId), signature._maxId);
    }

    [Fact]
    public void Constructor_WithMultipleComponents_SizeOfIsSum()
    {
        Signature signature = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        int expected =
            ComponentsMeta.GetComponentType(ComponentMeta<Position>.s_id)._sizeOf
            + ComponentsMeta.GetComponentType(ComponentMeta<Velocity>.s_id)._sizeOf;

        Assert.Equal(expected, signature._sizeOf);
    }

    [Fact]
    public void Constructor_WithComponentIds_BucketCountIsCorrect()
    {
        Signature signature = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        int expectedBucketCount = (signature._maxId >> 6) + 1;

        Assert.Equal(expectedBucketCount, signature._bucketCount);
    }

    [Fact]
    public void Constructor_WithComponentIds_BitsAreSetCorrectly()
    {
        int posId = ComponentMeta<Position>.s_id;
        int velId = ComponentMeta<Velocity>.s_id;

        Signature signature = new Signature([posId, velId]);

        Assert.NotEqual(0UL, signature.Buckets()[posId >> 6] & (1UL << (posId & 63)));
        Assert.NotEqual(0UL, signature.Buckets()[velId >> 6] & (1UL << (velId & 63)));
    }

    [Fact]
    public void Constructor_WithDuplicateComponentIds_ThrowsArgumentException()
    {
        int posId = ComponentMeta<Position>.s_id;

        Assert.Throws<ArgumentException>(() => new Signature([posId, posId]));
    }

    [Fact]
    public void Constructor_WithComponentIdAbove63_CreatesTwoBuckets()
    {
        Signature signature = new Signature([0, 64], bypass: true);

        Assert.Equal(2, signature._bucketCount);
        Assert.Equal(2, signature.Buckets().Length);
    }

    [Fact]
    public void Components_WithAllComponentsSet_ReturnsAllComponentIds()
    {
        int posId = ComponentMeta<Position>.s_id;
        int velId = ComponentMeta<Velocity>.s_id;

        Signature signature = new Signature([posId, velId]);

        ReadOnlySpan<int> values = signature.Components();

        Assert.Equal(2, values.Length);
        Assert.Contains(posId, values.ToArray());
        Assert.Contains(velId, values.ToArray());
    }

    [Fact]
    public void Constructor_WithPreComputedMetaAndBuckets_StoresValuesCorrectly()
    {
        int posId = ComponentMeta<Position>.s_id;
        int velId = ComponentMeta<Velocity>.s_id;

        ReadOnlySpan<int> componentIds = [posId, velId];

        int expectedMaxId = Math.Max(posId, velId);
        int expectedBucketCount = (expectedMaxId >> 6) + 1;
        int expectedSize =
            ComponentsMeta.GetComponentType(posId)._sizeOf
            + ComponentsMeta.GetComponentType(velId)._sizeOf;

        SignatureMeta meta = new SignatureMeta()
        {
            _size = expectedSize,
            _maxId = expectedMaxId,
            _bucketCount = expectedBucketCount,
        };

        ulong[] buckets = new ulong[expectedBucketCount];
        buckets[posId >> 6] |= 1UL << (posId & 63);
        buckets[velId >> 6] |= 1UL << (velId & 63);

        Signature signature = new Signature(componentIds, meta, buckets);

        Assert.Equal(expectedSize, signature._sizeOf);
        Assert.Equal(expectedMaxId, signature._maxId);
        Assert.Equal(expectedBucketCount, signature._bucketCount);
        Assert.Equal(componentIds.Length, signature._length);
        Assert.Equal(buckets.Length, signature.Buckets().Length);
    }

    [Fact]
    public void IsSatisfiedBy_SameSignature_ReturnsTrue()
    {
        Signature signature = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Assert.True(signature.IsSatisfiedBy(signature));
    }

    [Fact]
    public void IsSatisfiedBy_OtherHasMoreComponents_ReturnsTrue()
    {
        Signature signatureA = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Signature signatureB = new Signature(
            [
                ComponentMeta<Position>.s_id,
                ComponentMeta<Velocity>.s_id,
                ComponentMeta<Rotation>.s_id,
                ComponentMeta<Health>.s_id,
            ]
        );

        Assert.True(signatureA.IsSatisfiedBy(signatureB));
    }

    [Fact]
    public void IsSatisfiedBy_OtherIsMissingComponent_ReturnsFalse()
    {
        Signature signatureA = new Signature(
            [
                ComponentMeta<Position>.s_id,
                ComponentMeta<Velocity>.s_id,
                ComponentMeta<Rotation>.s_id,
            ]
        );

        Signature signatureB = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Assert.False(signatureA.IsSatisfiedBy(signatureB));
    }

    [Fact]
    public void IsSatisfiedBy_OtherIsMissingComponentFromHigherBucket_ReturnsFalse()
    {
        Signature signatureA = new Signature([0, 64], bypass: true);
        Signature signatureB = new Signature([0], bypass: true);

        Assert.False(signatureA.IsSatisfiedBy(signatureB));
    }

    [Fact]
    public void IsSatisfiedBy_OtherMissingSecondBucketComponent_ReturnsFalse()
    {
        Signature signatureA = new Signature([0, 64], bypass: true);
        Signature signatureB = new Signature([0, 65], bypass: true);

        Assert.False(signatureA.IsSatisfiedBy(signatureB));
    }

    [Fact]
    public void IsSatisfiedBy_CompletelyDifferentComponents_ReturnsFalse()
    {
        Signature signatureA = new Signature([ComponentMeta<Position>.s_id]);
        Signature signatureB = new Signature(
            [ComponentMeta<Velocity>.s_id, ComponentMeta<Rotation>.s_id]
        );

        Assert.False(signatureA.IsSatisfiedBy(signatureB));
    }

    [Fact]
    public void IsSatisfiedBy_ComponentsSpanningMultipleBuckets_ReturnsTrue()
    {
        Signature signatureA = new Signature([0, 64], bypass: true);
        Signature signatureB = new Signature([0, 64, 128], bypass: true);

        Assert.True(signatureA.IsSatisfiedBy(signatureB));
    }

    [Fact]
    public void Equals_SameSignature_ReturnsTrue()
    {
        Signature signature = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Assert.True(signature.Equals(signature));
    }

    [Fact]
    public void Equals_IdenticalComponents_ReturnsTrue()
    {
        Signature signatureA = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Signature signatureB = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Assert.True(signatureA.Equals(signatureB));
    }

    [Fact]
    public void Equals_OtherHasMoreComponents_ReturnsFalse()
    {
        Signature signatureA = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Signature signatureB = new Signature(
            [
                ComponentMeta<Position>.s_id,
                ComponentMeta<Velocity>.s_id,
                ComponentMeta<Rotation>.s_id,
            ]
        );

        Assert.False(signatureA.Equals(signatureB));
    }

    [Fact]
    public void Equals_OtherHasFewerComponents_ReturnsFalse()
    {
        Signature signatureA = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Signature signatureB = new Signature([ComponentMeta<Position>.s_id]);

        Assert.False(signatureA.Equals(signatureB));
    }

    [Fact]
    public void Equals_DifferentComponents_ReturnsFalse()
    {
        Signature signatureA = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Velocity>.s_id]
        );

        Signature signatureB = new Signature(
            [ComponentMeta<Position>.s_id, ComponentMeta<Rotation>.s_id]
        );

        Assert.False(signatureA.Equals(signatureB));
    }

    [Fact]
    public void Equals_ComponentsSpanningMultipleBuckets_ReturnsTrue()
    {
        Signature signatureA = new Signature([0, 64], bypass: true);
        Signature signatureB = new Signature([0, 64], bypass: true);

        Assert.True(signatureA.Equals(signatureB));
    }

    [Fact]
    public void Equals_DifferentComponentsInSecondBucket_ReturnsFalse()
    {
        Signature signatureA = new Signature([0, 64], bypass: true);
        Signature signatureB = new Signature([0, 65], bypass: true);

        Assert.False(signatureA.Equals(signatureB));
    }
}
#pragma warning restore CS0618
