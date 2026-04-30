using System;
using Theia.ECS.Contracts;
using Theia.ECS.Resources;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Resources;

public sealed class ResourcesTests
{
    [Fact]
    public void Resources_NonIntUnderlyingType_ThrowsOnFirstAccess()
    {
        World world = new();

        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() =>
            world.CreateResource(
                new ResourcesEntries<ByteKey, TestResource>
                {
                    Key = ByteKey.A,
                    Data = [new() { Value = 1 }],
                }
            )
        );

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void Resources_SparseEnum_ThrowsOnFirstAccess()
    {
        World world = new();

        TypeInitializationException exception = Assert.Throws<TypeInitializationException>(() =>
            world.CreateResource(
                new ResourcesEntries<SparseKey, TestResource>
                {
                    Key = SparseKey.A,
                    Data = [new() { Value = 1 }],
                },
                new ResourcesEntries<SparseKey, TestResource>
                {
                    Key = SparseKey.B,
                    Data = [new() { Value = 2 }],
                }
            )
        );

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void CreateResource_AllEnumValuesProvided_DoesNotThrow()
    {
        World world = new();

        world.CreateResource(
            new ResourcesEntries<TwoKeys, TestResource>
            {
                Key = TwoKeys.A,
                Data = [new() { Value = 1 }],
            },
            new ResourcesEntries<TwoKeys, TestResource>
            {
                Key = TwoKeys.B,
                Data = [new() { Value = 2 }],
            }
        );
    }

    [Fact]
    public void CreateResource_FewerEntriesThanEnumValues_ThrowsInvalidOperationException()
    {
        World world = new();

        Assert.Throws<InvalidOperationException>(() =>
            world.CreateResource(
                new ResourcesEntries<TwoKeys, TestResource>
                {
                    Key = TwoKeys.A,
                    Data = [new() { Value = 1 }],
                }
            )
        );
    }

    [Fact]
    public void CreateResource_MoreEntriesThanEnumValues_ThrowsInvalidOperationException()
    {
        World world = new();

        Assert.Throws<InvalidOperationException>(() =>
            world.CreateResource(
                new ResourcesEntries<SingleKey, TestResource>
                {
                    Key = SingleKey.A,
                    Data = [new() { Value = 1 }],
                },
                new ResourcesEntries<SingleKey, TestResource>
                {
                    Key = SingleKey.A,
                    Data = [new() { Value = 2 }],
                }
            )
        );
    }

    [Fact]
    public void CreateResource_DuplicateKey_ThrowsInvalidOperationException()
    {
        World world = new();

        Assert.Throws<InvalidOperationException>(() =>
            world.CreateResource(
                new ResourcesEntries<TwoKeys, TestResource>
                {
                    Key = TwoKeys.A,
                    Data = [new() { Value = 1 }],
                },
                new ResourcesEntries<TwoKeys, TestResource>
                {
                    Key = TwoKeys.A,
                    Data = [new() { Value = 2 }],
                }
            )
        );
    }

    [Fact]
    public void CreateResource_AlreadyCreated_ThrowsInvalidOperationException()
    {
        World world = new();

        world.CreateResource(
            new ResourcesEntries<SingleKey, TestResource>
            {
                Key = SingleKey.A,
                Data = [new() { Value = 1 }],
            }
        );

        Assert.Throws<InvalidOperationException>(() =>
            world.CreateResource(
                new ResourcesEntries<SingleKey, TestResource>
                {
                    Key = SingleKey.A,
                    Data = [new() { Value = 2 }],
                }
            )
        );
    }

    [Fact]
    public void CreateResource_ReturnsWorldForFluentChaining()
    {
        World world = new();

        World result = world.CreateResource(
            new ResourcesEntries<SingleKey, TestResource>
            {
                Key = SingleKey.A,
                Data = [new() { Value = 1 }],
            }
        );

        Assert.Same(world, result);
    }

    [Fact]
    public void GetResource_ReturnsRegisteredResource()
    {
        World world = new();

        TestResource[] dataA = [new() { Value = 1 }];
        TestResource[] dataB = [new() { Value = 2 }];

        world.CreateResource(
            new ResourcesEntries<TwoKeys, TestResource> { Key = TwoKeys.A, Data = dataA },
            new ResourcesEntries<TwoKeys, TestResource> { Key = TwoKeys.B, Data = dataB }
        );

        Resources<TwoKeys, TestResource> resources = world.GetResource<TwoKeys, TestResource>();

        Assert.Equal(1, resources.Get(TwoKeys.A, 0).Value);
        Assert.Equal(2, resources.Get(TwoKeys.B, 0).Value);
    }

    [Fact]
    public void GetResource_GetByKey_ReturnsFullArray()
    {
        World world = new();

        TestResource[] data = [new() { Value = 10 }, new() { Value = 20 }];

        world.CreateResource(
            new ResourcesEntries<SingleKey, TestResource> { Key = SingleKey.A, Data = data }
        );

        Resources<SingleKey, TestResource> resources = world.GetResource<SingleKey, TestResource>();

        Assert.All(resources.Get(SingleKey.A).ToArray(), (item) => data.Contains(item));
    }

    [Fact]
    public void GetResource_GetByKeyAndIndex_ReturnsCorrectElement()
    {
        World world = new();

        TestResource[] data = [new() { Value = 10 }, new() { Value = 20 }, new() { Value = 30 }];

        world.CreateResource(
            new ResourcesEntries<SingleKey, TestResource> { Key = SingleKey.A, Data = data }
        );

        Resources<SingleKey, TestResource> resources = world.GetResource<SingleKey, TestResource>();

        Assert.Equal(10, resources.Get(SingleKey.A, 0).Value);
        Assert.Equal(20, resources.Get(SingleKey.A, 1).Value);
        Assert.Equal(30, resources.Get(SingleKey.A, 2).Value);
    }

    [Fact]
    public void GetResource_DifferentTypes_AreIndependent()
    {
        World world = new();

        world.CreateResource(
            new ResourcesEntries<SingleKey, TestResource>
            {
                Key = SingleKey.A,
                Data = [new() { Value = 1 }],
            }
        );

        world.CreateResource(
            new ResourcesEntries<TwoKeys, TestResource>
            {
                Key = TwoKeys.A,
                Data = [new() { Value = 100 }],
            },
            new ResourcesEntries<TwoKeys, TestResource>
            {
                Key = TwoKeys.B,
                Data = [new() { Value = 200 }],
            }
        );

        Assert.Equal(1, world.GetResource<SingleKey, TestResource>().Get(SingleKey.A, 0).Value);
        Assert.Equal(100, world.GetResource<TwoKeys, TestResource>().Get(TwoKeys.A, 0).Value);
        Assert.Equal(200, world.GetResource<TwoKeys, TestResource>().Get(TwoKeys.B, 0).Value);
    }

    [Fact]
    public void GetResource_DifferentWorlds_AreIsolated()
    {
        World world1 = new();
        World world2 = new();

        world1.CreateResource(
            new ResourcesEntries<SingleKey, TestResource>
            {
                Key = SingleKey.A,
                Data = [new() { Value = 1 }],
            }
        );

        world2.CreateResource(
            new ResourcesEntries<SingleKey, TestResource>
            {
                Key = SingleKey.A,
                Data = [new() { Value = 999 }],
            }
        );

        Assert.Equal(1, world1.GetResource<SingleKey, TestResource>().Get(SingleKey.A, 0).Value);
        Assert.Equal(999, world2.GetResource<SingleKey, TestResource>().Get(SingleKey.A, 0).Value);
    }
}
