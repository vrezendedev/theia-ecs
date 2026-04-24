using System;
using System.IO;
using System.Threading.Tasks;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Worlds;

[Collection("MetaRequester")]
public sealed class WorldSerializationTests
{
    [Fact]
    public void Serialize_EmptyWorld_DoesNotThrow()
    {
        string path = Path.GetTempFileName();

        try
        {
            Exception exception = Record.Exception(() => new World().Serialize(path));
            Assert.Null(exception);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Serialize_CreatesNonEmptyFile()
    {
        string path = Path.GetTempFileName();

        World world = new();

        world.CreateAssemblage<Position>().Create(new Position { X = 1, Y = 2 });

        try
        {
            world.Serialize(path);

            Assert.True(new FileInfo(path).Length > 0);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Serialize_CalledTwice_OverwritesPreviousFile()
    {
        string path = Path.GetTempFileName();

        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();
        assemblage.Create(new Position());

        try
        {
            world.Serialize(path);

            long sizeFirst = new FileInfo(path).Length;

            for (int i = 0; i < 10; i++)
                assemblage.Create(new Position { X = i, Y = i });

            world.Serialize(path);

            long sizeSecond = new FileInfo(path).Length;

            Assert.True(sizeSecond > sizeFirst);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Serialize_WithExplicitDefaultOptions_DoesNotThrow()
    {
        string path = Path.GetTempFileName();

        try
        {
            Exception exception = Record.Exception(() =>
                new World().Serialize(path, World.DefaultMessagePackSerializerOptions)
            );
            Assert.Null(exception);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Deserialize_EmptyWorld_EntityCountRemainsZero()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(0, target.CountEntities());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_SingleEntity_EntityCountMatchesSource()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.CreateAssemblage<Position>().Create(new Position());

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(source.CountEntities(), target.CountEntities());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleEntities_EntityCountMatchesSource()
    {
        string path = Path.GetTempFileName();
        const int entityCount = 10_000;

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            for (int i = 0; i < entityCount; i++)
                assemblage.Create(new Position { X = i, Y = i * 2 });

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(source.CountEntities(), target.CountEntities());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleArchetypes_AllEntitiesRestored()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.CreateAssemblage<Position>().Create(new Position());
            source.CreateAssemblage<Velocity>().Create(new Velocity());

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(2, target.CountEntities());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_SingleEntity_IsAliveAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.CreateAssemblage<Position>().Create(new Position());

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(target.IsAlive(new Entity() { _id = 0, _version = 1 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleEntities_AllAreAliveAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            assemblage.Create(new Position());
            assemblage.Create(new Position());
            assemblage.Create(new Position());

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(target.IsAlive(new Entity() { _id = 0, _version = 1 }));
            Assert.True(target.IsAlive(new Entity() { _id = 1, _version = 1 }));
            Assert.True(target.IsAlive(new Entity() { _id = 2, _version = 1 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_GhoulsAreNotRestored_EntityCountExcludesGhouls()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity alive = assemblage.Create(new Position());

            Entity ghoul = assemblage.Create(new Position());
            source.TryGhoulify(ghoul);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(1, target.CountEntities());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_GhoulsAreNotRestored_GhoulCountIsZero()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Entity entity = source.CreateAssemblage<Position>().Create(new Position());
            source.TryGhoulify(entity);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(0, target.CountGhouls());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_GhoulifiedEntity_IsNotAliveAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Entity entity = source.CreateAssemblage<Position>().Create(new Position());
            source.TryGhoulify(entity);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.False(target.IsAlive(entity));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_SomeGhoulified_OnlyAliveEntitiesRestored()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            assemblage.Create(new Position());
            Entity entityB = assemblage.Create(new Position());
            assemblage.Create(new Position());

            source.TryGhoulify(entityB);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(target.IsAlive(new Entity() { _id = 0, _version = 1 }));
            Assert.True(target.IsAlive(new Entity() { _id = 1, _version = 1 }));
            Assert.False(target.IsAlive(new Entity() { _id = 2, _version = 1 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_ComponentData_IsPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.CreateAssemblage<Position>().Create(new Position { X = 42, Y = 99 });

            source.Serialize(path);
            target.Deserialize(path);

            ref Position restored = ref target.Get<Position>(
                new Entity() { _id = 0, _version = 1 }
            );

            Assert.Equal(42, restored.X);
            Assert.Equal(99, restored.Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleEntities_ComponentDataIsDistinctPerEntity()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            assemblage.Create(new Position { X = 1, Y = 2 });
            assemblage.Create(new Position { X = 10, Y = 20 });
            assemblage.Create(new Position { X = 99, Y = 77 });

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(1, target.Get<Position>(new Entity() { _id = 0, _version = 1 }).X);
            Assert.Equal(2, target.Get<Position>(new Entity() { _id = 0, _version = 1 }).Y);

            Assert.Equal(10, target.Get<Position>(new Entity() { _id = 1, _version = 1 }).X);
            Assert.Equal(20, target.Get<Position>(new Entity() { _id = 1, _version = 1 }).Y);

            Assert.Equal(99, target.Get<Position>(new Entity() { _id = 2, _version = 1 }).X);
            Assert.Equal(77, target.Get<Position>(new Entity() { _id = 2, _version = 1 }).Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_AddedComponent_IsPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Entity entity = source.CreateAssemblage<Position>().Create(new Position());
            source.TryAddComponent(entity, new Velocity() { X = 3, Y = 7 });

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(target.Has<Velocity>(entity));

            Assert.Equal(3, target.Get<Velocity>(entity).X);
            Assert.Equal(7, target.Get<Velocity>(entity).Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleComponents_AllComponentDataPreserved()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Entity entity = source
                .CreateAssemblage<Position>()
                .Create(new Position { X = 11, Y = 22 });

            source.TryAddComponent(entity, new Velocity { X = 3, Y = 4 });

            source.Serialize(path);
            target.Deserialize(path);

            Entity newEntity = new Entity() { _id = 0, _version = 1 };

            Assert.Equal(11, target.Get<Position>(newEntity).X);
            Assert.Equal(22, target.Get<Position>(newEntity).Y);
            Assert.Equal(3, target.Get<Velocity>(newEntity).X);
            Assert.Equal(4, target.Get<Velocity>(newEntity).Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleComponents_EntityHasAllComponentsAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source
                .CreateAssemblage<Position, Velocity, Rotation>()
                .Create(default, default, default);

            source.Serialize(path);
            target.Deserialize(path);

            Entity newEntity = new Entity() { _id = 0, _version = 1 };

            Assert.True(target.Has<Position>(newEntity));
            Assert.True(target.Has<Velocity>(newEntity));
            Assert.True(target.Has<Rotation>(newEntity));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleArchetypes_ComponentDataPreservedPerArchetype()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.CreateAssemblage<Position>().Create(new Position { X = 5, Y = 6 });
            source.CreateAssemblage<Velocity>().Create(new Velocity { X = 7, Y = 8 });

            source.Serialize(path);
            target.Deserialize(path);

            Entity newPosEntity = new Entity() { _id = 0, _version = 1 };
            Entity newVelEntity = new Entity() { _id = 1, _version = 1 };

            Assert.Equal(5, target.Get<Position>(newPosEntity).X);
            Assert.Equal(6, target.Get<Position>(newPosEntity).Y);
            Assert.Equal(7, target.Get<Velocity>(newVelEntity).X);
            Assert.Equal(8, target.Get<Velocity>(newVelEntity).Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_TagRelation_OwnerHasRelationAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity owner = assemblage.Create(new Position());
            Entity linked = assemblage.Create(new Position());

            source.TryAddTagRelation<ParentOf>(owner, linked);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(target.HasRelation<ParentOf>(new Entity() { _id = 0, _version = 1 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_TagRelation_CorrectTargetPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity owner = assemblage.Create(new Position());
            Entity linked = assemblage.Create(new Position());

            source.TryAddTagRelation<ParentOf>(owner, linked);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(
                target.IsRelatedTo<ParentOf>(
                    new Entity() { _id = 0, _version = 1 },
                    new Entity() { _id = 1, _version = 1 }
                )
            );
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleRelationTargets_AllPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity owner = assemblage.Create(new Position());
            Entity targetA = assemblage.Create(new Position());
            Entity targetB = assemblage.Create(new Position());

            source.TryAddTagRelation<Friendship>(owner, targetA);
            source.TryAddTagRelation<Friendship>(owner, targetB);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(
                2,
                target.CountRelations<Friendship>(new Entity() { _id = 0, _version = 1 })
            );
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_TagRelation_NonOwnerHasNoRelationAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity owner = assemblage.Create(new Position());

            Entity linked = assemblage.Create(new Position());
            Entity unrelated = assemblage.Create(new Position());

            source.TryAddTagRelation<ParentOf>(owner, linked);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.False(target.HasRelation<ParentOf>(new Entity() { _id = 1, _version = 1 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_GhoulifiedRelationTarget_RelationNotRestoredAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity owner = assemblage.Create(new Position());
            Entity linked = assemblage.Create(new Position());

            source.TryAddTagRelation<ParentOf>(owner, linked);
            source.TryGhoulify(linked);

            source.Serialize(path);
            target.Deserialize(path);

            Assert.False(target.HasRelation<ParentOf>(new Entity() { _id = 0, _version = 1 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_EvaluatedRelation_IsRestoredAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity owner = assemblage.Create(new Position());
            Entity linked = assemblage.Create(new Position());

            source.TryAddEvaluatedRelation(owner, linked, new LinkWeight { Value = 2.5f });

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(
                target.IsRelatedTo<LinkWeight>(
                    new Entity() { _id = 0, _version = 1 },
                    new Entity() { _id = 1, _version = 1 }
                )
            );
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_EvaluatedRelation_OwnerHasRelationAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            Entity owner = assemblage.Create(new Position());
            Entity linked = assemblage.Create(new Position());

            source.TryAddEvaluatedRelation(owner, linked, new LinkWeight { Value = 1.0f });

            source.Serialize(path);
            target.Deserialize(path);

            Assert.True(target.HasRelation<LinkWeight>(new Entity() { _id = 0, _version = 1 }));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_UniqueData_IsPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.SetUnique(new Position { X = 77, Y = 88 });

            source.Serialize(path);
            target.Deserialize(path);

            Position unique = target.ReadUnique<Position>();

            Assert.Equal(77, unique.X);
            Assert.Equal(88, unique.Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_MultipleUniques_AllPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.SetUnique(new Position { X = 3, Y = 4 });
            source.SetUnique(new Velocity { X = 9, Y = 1 });

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(3, target.ReadUnique<Position>().X);
            Assert.Equal(4, target.ReadUnique<Position>().Y);
            Assert.Equal(9, target.ReadUnique<Velocity>().X);
            Assert.Equal(1, target.ReadUnique<Velocity>().Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void RoundTrip_UniqueWithEntitiesCoexist_BothPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.SetUnique(new Velocity { X = 5, Y = 5 });

            source.CreateAssemblage<Position>().Create(new Position { X = 2, Y = 3 });

            source.Serialize(path);
            target.Deserialize(path);

            Assert.Equal(1, target.CountEntities());
            Assert.Equal(5, target.ReadUnique<Velocity>().X);
            Assert.Equal(2, target.Get<Position>(new Entity() { _id = 0, _version = 1 }).X);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task SerializeAsync_EmptyWorld_DoesNotThrow()
    {
        string path = Path.GetTempFileName();

        try
        {
            Exception exception = await Record.ExceptionAsync(async () =>
                await new World().SerializeAsync(path)
            );
            Assert.Null(exception);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RoundTripAsync_EntityCount_MatchesAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        const int entityCount = 10_000;

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            for (int i = 0; i < entityCount; i++)
                assemblage.Create(new Position { X = i, Y = i });

            await source.SerializeAsync(path);
            await target.DeserializeAsync(path);

            Assert.Equal(source.CountEntities(), target.CountEntities());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RoundTripAsync_ComponentData_IsPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.CreateAssemblage<Position>().Create(new Position { X = 55, Y = 66 });

            await source.SerializeAsync(path);
            await target.DeserializeAsync(path);

            Entity newEntity = new Entity() { _id = 0, _version = 1 };

            Assert.Equal(55, target.Get<Position>(newEntity).X);
            Assert.Equal(66, target.Get<Position>(newEntity).Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RoundTripAsync_UniqueData_IsPreservedAfterDeserialize()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            source.SetUnique(new Position { X = 33, Y = 44 });

            await source.SerializeAsync(path);
            await target.DeserializeAsync(path);

            Assert.Equal(33, target.ReadUnique<Position>().X);
            Assert.Equal(44, target.ReadUnique<Position>().Y);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RoundTripAsync_GhoulsAreNotRestored()
    {
        string path = Path.GetTempFileName();

        World source = new();
        World target = new();

        try
        {
            Assemblage<Position> assemblage = source.CreateAssemblage<Position>();

            assemblage.Create(new Position());
            Entity ghoul = assemblage.Create(new Position());
            source.TryGhoulify(ghoul);

            await source.SerializeAsync(path);
            await target.DeserializeAsync(path);

            Assert.Equal(1, target.CountEntities());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task Serialize_AndSerializeAsync_ProduceFilesWithSameLength()
    {
        World world = new();
        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();

        for (int i = 0; i < 5; i++)
            assemblage.Create(new Position { X = i, Y = i });

        string syncPath = Path.GetTempFileName();
        string asyncPath = Path.GetTempFileName();

        try
        {
            world.Serialize(syncPath);
            await world.SerializeAsync(asyncPath);

            long syncSize = new FileInfo(syncPath).Length;
            long asyncSize = new FileInfo(asyncPath).Length;

            Assert.Equal(syncSize, asyncSize);
        }
        finally
        {
            File.Delete(syncPath);
            File.Delete(asyncPath);
        }
    }

    [Fact]
    public void RoundTrip_WithExplicitDefaultOptions_ProducesSameResultAsImplicit()
    {
        World source = new();

        World implicitTarget = new();
        World explicitTarget = new();

        source.CreateAssemblage<Position>().Create(new Position { X = 7, Y = 14 });

        string implicitPath = Path.GetTempFileName();
        string explicitPath = Path.GetTempFileName();

        try
        {
            source.Serialize(implicitPath);
            source.Serialize(explicitPath, World.DefaultMessagePackSerializerOptions);

            implicitTarget.Deserialize(implicitPath);
            explicitTarget.Deserialize(explicitPath, World.DefaultMessagePackSerializerOptions);

            Assert.Equal(implicitTarget.CountEntities(), explicitTarget.CountEntities());
        }
        finally
        {
            File.Delete(implicitPath);
            File.Delete(explicitPath);
        }
    }
}
