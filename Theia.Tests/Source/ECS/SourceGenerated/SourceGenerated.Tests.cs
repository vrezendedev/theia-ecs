using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.SourceGenerated;

public sealed class SourceGeneratedTests
{
    [Fact]
    public void CreateAssemblage_T2_ReturnsNonNullInstance()
    {
        Assemblage<Position, Velocity> assemblage = new World().CreateAssemblage<
            Position,
            Velocity
        >();

        Assert.NotNull(assemblage);
    }

    [Fact]
    public void Create_T2_EntityIsAlive()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        Entity entity = assemblage.Create(new Position() { X = 1 }, new Velocity() { X = 2 });

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void Create_T2_BothComponentsStoredCorrectly()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        Entity entity = assemblage.Create(
            new Position() { X = 3, Y = 7 },
            new Velocity() { X = 5, Y = 11 }
        );

        Assert.Equal(3, world.Get<Position>(entity).X);
        Assert.Equal(7, world.Get<Position>(entity).Y);
        Assert.Equal(5, world.Get<Velocity>(entity).X);
        Assert.Equal(11, world.Get<Velocity>(entity).Y);
    }

    [Fact]
    public void Create_T2_MultipleEntities_ComponentDataIsIndependent()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        Entity entityA = assemblage.Create(new Position() { X = 1 }, new Velocity() { X = 10 });
        Entity entityB = assemblage.Create(new Position() { X = 2 }, new Velocity() { X = 20 });

        Assert.Equal(1, world.Get<Position>(entityA).X);
        Assert.Equal(2, world.Get<Position>(entityB).X);
        Assert.Equal(10, world.Get<Velocity>(entityA).X);
        Assert.Equal(20, world.Get<Velocity>(entityB).X);
    }

    [Fact]
    public void DeferredCreate_T2_AfterFlush_EntityIsCreated()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        int countBefore = world.CountEntitiesAlive();

        assemblage.DeferredCreate(new Position() { X = 9 }, new Velocity() { X = 4 });

        Assert.Equal(countBefore, world.CountEntitiesAlive());

        world.FlushDeferred();

        Assert.Equal(countBefore + 1, world.CountEntitiesAlive());
    }

    [Fact]
    public void Create_T4_EntityIsAlive()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health> assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        Entity entity = assemblage.Create(
            new Position() { X = 1 },
            new Velocity() { X = 2 },
            new Rotation(),
            new Health()
        );

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void Create_T4_FirstAndLastComponentsStoredCorrectly()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health> assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        Entity entity = assemblage.Create(
            new Position() { X = 42, Y = 13 },
            new Velocity() { X = 0 },
            new Rotation(),
            new Health()
        );

        Assert.Equal(42, world.Get<Position>(entity).X);
        Assert.Equal(13, world.Get<Position>(entity).Y);
        Assert.True(world.Has<Health>(entity));
    }

    [Fact]
    public void Create_T4_AllFourComponentsPresent()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health> assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        Entity entity = assemblage.Create(
            new Position(),
            new Velocity(),
            new Rotation(),
            new Health()
        );

        Assert.True(world.Has<Position>(entity));
        Assert.True(world.Has<Velocity>(entity));
        Assert.True(world.Has<Rotation>(entity));
        Assert.True(world.Has<Health>(entity));
    }

    [Fact]
    public void Create_T8_EntityIsAlive()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> assemblage =
            world.CreateAssemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        Entity entity = assemblage.Create(
            new Position() { X = 1 },
            new Velocity() { X = 2 },
            new Rotation(),
            new Health(),
            new Mass() { Value = 5f },
            new Scale() { Value = 1 },
            new Age() { Value = 3 },
            new Tag() { Value = 7 }
        );

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void Create_T8_AllEightComponentsPresent()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> assemblage =
            world.CreateAssemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        Entity entity = assemblage.Create(
            new Position(),
            new Velocity(),
            new Rotation(),
            new Health(),
            new Mass(),
            new Scale(),
            new Age(),
            new Tag()
        );

        Assert.True(world.Has<Position>(entity));
        Assert.True(world.Has<Velocity>(entity));
        Assert.True(world.Has<Rotation>(entity));
        Assert.True(world.Has<Health>(entity));
        Assert.True(world.Has<Mass>(entity));
        Assert.True(world.Has<Scale>(entity));
        Assert.True(world.Has<Age>(entity));
        Assert.True(world.Has<Tag>(entity));
    }

    [Fact]
    public void Create_T8_SampledComponentDataStoredCorrectly()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> assemblage =
            world.CreateAssemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        Entity entity = assemblage.Create(
            new Position() { X = 11 },
            new Velocity() { X = 22 },
            new Rotation(),
            new Health(),
            new Mass() { Value = 33f },
            new Scale(),
            new Age() { Value = 44 },
            new Tag() { Value = 55 }
        );

        Assert.Equal(11, world.Get<Position>(entity).X);
        Assert.Equal(22, world.Get<Velocity>(entity).X);
        Assert.Equal(33f, world.Get<Mass>(entity).Value);
        Assert.Equal(44, world.Get<Age>(entity).Value);
        Assert.Equal(55, world.Get<Tag>(entity).Value);
    }

    [Fact]
    public void Create_T16_EntityIsAlive()
    {
        World world = new();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        Entity entity = assemblage.Create(
            new Position() { X = 1 },
            new Velocity() { X = 2 },
            new Rotation(),
            new Health(),
            new Mass() { Value = 3f },
            new Scale() { Value = 1 },
            new Age() { Value = 4 },
            new Tag() { Value = 5 },
            new Color() { R = 255 },
            new Force() { X = 6f },
            new Momentum() { X = 7f },
            new Gravity() { Value = 8f },
            new Friction() { Value = 9f },
            new Torque() { Value = 10f },
            new Impulse() { X = 11f },
            new Damping() { Value = 12f }
        );

        Assert.True(world.IsAlive(entity));
    }

    [Fact]
    public void Create_T16_AllSixteenComponentsPresent()
    {
        World world = new();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        Entity entity = assemblage.Create(
            new Position(),
            new Velocity(),
            new Rotation(),
            new Health(),
            new Mass(),
            new Scale(),
            new Age(),
            new Tag(),
            new Color(),
            new Force(),
            new Momentum(),
            new Gravity(),
            new Friction(),
            new Torque(),
            new Impulse(),
            new Damping()
        );

        Assert.True(world.Has<Position>(entity));
        Assert.True(world.Has<Velocity>(entity));
        Assert.True(world.Has<Rotation>(entity));
        Assert.True(world.Has<Health>(entity));
        Assert.True(world.Has<Mass>(entity));
        Assert.True(world.Has<Scale>(entity));
        Assert.True(world.Has<Age>(entity));
        Assert.True(world.Has<Tag>(entity));
        Assert.True(world.Has<Color>(entity));
        Assert.True(world.Has<Force>(entity));
        Assert.True(world.Has<Momentum>(entity));
        Assert.True(world.Has<Gravity>(entity));
        Assert.True(world.Has<Friction>(entity));
        Assert.True(world.Has<Torque>(entity));
        Assert.True(world.Has<Impulse>(entity));
        Assert.True(world.Has<Damping>(entity));
    }

    [Fact]
    public void Create_T16_SampledComponentDataStoredCorrectly()
    {
        World world = new();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        Entity entity = assemblage.Create(
            new Position() { X = 1 },
            new Velocity() { X = 2 },
            new Rotation(),
            new Health(),
            new Mass() { Value = 3f },
            new Scale(),
            new Age() { Value = 4 },
            new Tag(),
            new Color() { R = 128 },
            new Force() { X = 5f },
            new Momentum(),
            new Gravity() { Value = 9.8f },
            new Friction(),
            new Torque(),
            new Impulse(),
            new Damping() { Value = 0.5f }
        );

        Assert.Equal(1, world.Get<Position>(entity).X);
        Assert.Equal(2, world.Get<Velocity>(entity).X);
        Assert.Equal(3f, world.Get<Mass>(entity).Value);
        Assert.Equal(4, world.Get<Age>(entity).Value);
        Assert.Equal(128, world.Get<Color>(entity).R);
        Assert.Equal(5f, world.Get<Force>(entity).X);
        Assert.Equal(9.8f, world.Get<Gravity>(entity).Value, precision: 4);
        Assert.Equal(0.5f, world.Get<Damping>(entity).Value, precision: 4);
    }

    [Fact]
    public void SettlerQuery_T2_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        SettlerQuery<Position, Velocity> query = world.CreateSettlerQuery(assemblage);

        const int count = 5;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position() { X = i }, new Velocity() { X = i });

        int callCount = 0;

        query.ForEach((ref Position _, ref Velocity __) => callCount++);

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void SettlerQuery_T2_ForEach_CanReadBothComponents()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        SettlerQuery<Position, Velocity> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position() { X = 3, Y = 7 }, new Velocity() { X = 5, Y = 11 });

        float sumX = 0;
        float sumY = 0;

        query.ForEach(
            (ref Position p, ref Velocity v) =>
            {
                sumX = p.X + v.X;
                sumY = p.Y + v.Y;
            }
        );

        Assert.Equal(8, sumX);
        Assert.Equal(18, sumY);
    }

    [Fact]
    public void SettlerQuery_T2_ForEach_CanMutateBothComponents()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        SettlerQuery<Position, Velocity> query = world.CreateSettlerQuery(assemblage);

        Entity entity = assemblage.Create(
            new Position() { X = 1, Y = 2 },
            new Velocity() { X = 3, Y = 4 }
        );

        query.ForEach(
            (ref Position p, ref Velocity v) =>
            {
                p.X = 99;
                v.Y = 77;
            }
        );

        Assert.Equal(99, world.Get<Position>(entity).X);
        Assert.Equal(77, world.Get<Velocity>(entity).Y);
    }

    [Fact]
    public void SettlerQuery_T2_ForEachEntity_ProvidesCorrectEntityAndBothComponents()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        SettlerQuery<Position, Velocity> query = world.CreateSettlerQuery(assemblage);

        Entity created = assemblage.Create(new Position() { X = 10 }, new Velocity() { X = 20 });

        Entity receivedEntity = default;
        float receivedPX = 0;
        float receivedVX = 0;

        query.ForEachEntity(
            (Entity e, ref Position p, ref Velocity v) =>
            {
                receivedEntity = e;
                receivedPX = p.X;
                receivedVX = v.X;
            }
        );

        Assert.Equal(created._id, receivedEntity._id);
        Assert.Equal(10, receivedPX);
        Assert.Equal(20, receivedVX);
    }

    [Fact]
    public void NomadQuery_T2_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        NomadQuery<Position, Velocity> query = world.CreateNomadQuery<Position, Velocity>();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        const int count = 4;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position() { X = i }, new Velocity() { X = i });

        int callCount = 0;

        query.ForEach((ref Position _, ref Velocity __) => callCount++);

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void NomadQuery_T2_ForEach_CanMutateBothComponents()
    {
        World world = new();

        NomadQuery<Position, Velocity> query = world.CreateNomadQuery<Position, Velocity>();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        Entity entity = assemblage.Create(new Position() { X = 1 }, new Velocity() { X = 2 });

        query.ForEach(
            (ref Position p, ref Velocity v) =>
            {
                p.X = 55;
                v.X = 66;
            }
        );

        Assert.Equal(55, world.Get<Position>(entity).X);
        Assert.Equal(66, world.Get<Velocity>(entity).X);
    }

    [Fact]
    public void NomadQuery_T2_ForEachEntity_ProvidesCorrectEntityAndBothComponents()
    {
        World world = new();

        NomadQuery<Position, Velocity> query = world.CreateNomadQuery<Position, Velocity>();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        Entity created = assemblage.Create(new Position() { X = 7 }, new Velocity() { X = 13 });

        Entity receivedEntity = default;

        float receivedPX = 0;
        float receivedVX = 0;

        query.ForEachEntity(
            (Entity e, ref Position p, ref Velocity v) =>
            {
                receivedEntity = e;
                receivedPX = p.X;
                receivedVX = v.X;
            }
        );

        Assert.Equal(created._id, receivedEntity._id);
        Assert.Equal(7, receivedPX);
        Assert.Equal(13, receivedVX);
    }

    [Fact]
    public void NomadQuery_T2_ArchetypeWithSupersetOfComponents_IsMatched()
    {
        World world = new();

        NomadQuery<Position, Velocity> query = world.CreateNomadQuery<Position, Velocity>();

        Assemblage<Position, Velocity> base_ = world.CreateAssemblage<Position, Velocity>();

        Entity entity = base_.Create(new Position() { X = 1 }, new Velocity() { X = 2 });

        world.TryAddComponent<Rotation>(entity);

        int callCount = 0;

        query.ForEach((ref Position _, ref Velocity __) => callCount++);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void SettlerQuery_T4_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health> assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        SettlerQuery<Position, Velocity, Rotation, Health> query = world.CreateSettlerQuery(
            assemblage
        );

        const int count = 3;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position(), new Velocity(), new Rotation(), new Health());

        int callCount = 0;

        query.ForEach(
            (ref Position _, ref Velocity __, ref Rotation ___, ref Health ____) => callCount++
        );

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void SettlerQuery_T4_ForEach_CanReadAllFourComponents()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health> assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        SettlerQuery<Position, Velocity, Rotation, Health> query = world.CreateSettlerQuery(
            assemblage
        );

        assemblage.Create(
            new Position() { X = 1 },
            new Velocity() { X = 2 },
            new Rotation(),
            new Health()
        );

        float posX = 0;
        float velX = 0;

        query.ForEach(
            (ref Position p, ref Velocity v, ref Rotation _, ref Health __) =>
            {
                posX = p.X;
                velX = v.X;
            }
        );

        Assert.Equal(1, posX);
        Assert.Equal(2, velX);
    }

    [Fact]
    public void NomadQuery_T4_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        NomadQuery<Position, Velocity, Rotation, Health> query = world.CreateNomadQuery<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        Assemblage<Position, Velocity, Rotation, Health> assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        const int count = 3;

        for (int i = 0; i < count; i++)
            assemblage.Create(new Position(), new Velocity(), new Rotation(), new Health());

        int callCount = 0;

        query.ForEach(
            (ref Position _, ref Velocity __, ref Rotation ___, ref Health ____) => callCount++
        );

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void SettlerQuery_T4_ForEachEntity_ProvidesCorrectEntity()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health> assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health
        >();

        SettlerQuery<Position, Velocity, Rotation, Health> query = world.CreateSettlerQuery(
            assemblage
        );

        Entity created = assemblage.Create(
            new Position() { X = 42 },
            new Velocity(),
            new Rotation(),
            new Health()
        );

        Entity receivedEntity = default;

        query.ForEachEntity(
            (Entity e, ref Position _, ref Velocity __, ref Rotation ___, ref Health ____) =>
                receivedEntity = e
        );

        Assert.Equal(created._id, receivedEntity._id);
    }

    [Fact]
    public void SettlerQuery_T8_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> assemblage =
            world.CreateAssemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        SettlerQuery<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> query =
            world.CreateSettlerQuery(assemblage);

        const int count = 4;

        for (int i = 0; i < count; i++)
            assemblage.Create(
                new Position(),
                new Velocity(),
                new Rotation(),
                new Health(),
                new Mass(),
                new Scale(),
                new Age(),
                new Tag()
            );

        int callCount = 0;

        query.ForEach(
            (
                ref Position _,
                ref Velocity __,
                ref Rotation ___,
                ref Health ____,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8
            ) => callCount++
        );

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void SettlerQuery_T8_ForEach_CanReadAllEightComponents()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> assemblage =
            world.CreateAssemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        SettlerQuery<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> query =
            world.CreateSettlerQuery(assemblage);

        assemblage.Create(
            new Position() { X = 1 },
            new Velocity() { X = 2 },
            new Rotation(),
            new Health(),
            new Mass() { Value = 3f },
            new Scale(),
            new Age() { Value = 4 },
            new Tag() { Value = 5 }
        );

        float posX = 0;
        float mass = 0;
        int age = 0;
        int tag = 0;

        query.ForEach(
            (
                ref Position p,
                ref Velocity _,
                ref Rotation __,
                ref Health ___,
                ref Mass m,
                ref Scale _4,
                ref Age a,
                ref Tag t
            ) =>
            {
                posX = p.X;
                mass = m.Value;
                age = a.Value;
                tag = t.Value;
            }
        );

        Assert.Equal(1, posX);
        Assert.Equal(3f, mass);
        Assert.Equal(4, age);
        Assert.Equal(5, tag);
    }

    [Fact]
    public void NomadQuery_T8_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        NomadQuery<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> query =
            world.CreateNomadQuery<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        Assemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> assemblage =
            world.CreateAssemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        const int count = 4;

        for (int i = 0; i < count; i++)
            assemblage.Create(
                new Position(),
                new Velocity(),
                new Rotation(),
                new Health(),
                new Mass(),
                new Scale(),
                new Age(),
                new Tag()
            );

        int callCount = 0;

        query.ForEach(
            (
                ref Position _,
                ref Velocity __,
                ref Rotation ___,
                ref Health ____,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8
            ) => callCount++
        );

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void SettlerQuery_T8_ForEachEntity_ProvidesCorrectEntity()
    {
        World world = new();

        Assemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> assemblage =
            world.CreateAssemblage<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>();

        SettlerQuery<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag> query =
            world.CreateSettlerQuery(assemblage);

        Entity created = assemblage.Create(
            new Position() { X = 7 },
            new Velocity(),
            new Rotation(),
            new Health(),
            new Mass(),
            new Scale(),
            new Age(),
            new Tag()
        );

        Entity receivedEntity = default;

        query.ForEachEntity(
            (
                Entity e,
                ref Position _,
                ref Velocity __,
                ref Rotation ___,
                ref Health ____,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8
            ) => receivedEntity = e
        );

        Assert.Equal(created._id, receivedEntity._id);
    }

    [Fact]
    public void SettlerQuery_T16_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        SettlerQuery<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > query = world.CreateSettlerQuery(assemblage);

        const int count = 3;

        for (int i = 0; i < count; i++)
            assemblage.Create(
                new Position(),
                new Velocity(),
                new Rotation(),
                new Health(),
                new Mass(),
                new Scale(),
                new Age(),
                new Tag(),
                new Color(),
                new Force(),
                new Momentum(),
                new Gravity(),
                new Friction(),
                new Torque(),
                new Impulse(),
                new Damping()
            );

        int callCount = 0;

        query.ForEach(
            (
                ref Position _1,
                ref Velocity _2,
                ref Rotation _3,
                ref Health _4,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8,
                ref Color _9,
                ref Force _10,
                ref Momentum _11,
                ref Gravity _12,
                ref Friction _13,
                ref Torque _14,
                ref Impulse _15,
                ref Damping _16
            ) => callCount++
        );

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void SettlerQuery_T16_ForEach_CanReadFirstAndLastComponents()
    {
        World world = new();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        SettlerQuery<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(
            new Position() { X = 1 },
            new Velocity(),
            new Rotation(),
            new Health(),
            new Mass(),
            new Scale(),
            new Age(),
            new Tag(),
            new Color(),
            new Force(),
            new Momentum(),
            new Gravity(),
            new Friction(),
            new Torque(),
            new Impulse(),
            new Damping() { Value = 0.9f }
        );

        float posX = 0;
        float damping = 0;

        query.ForEach(
            (
                ref Position p,
                ref Velocity _2,
                ref Rotation _3,
                ref Health _4,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8,
                ref Color _9,
                ref Force _10,
                ref Momentum _11,
                ref Gravity _12,
                ref Friction _13,
                ref Torque _14,
                ref Impulse _15,
                ref Damping d
            ) =>
            {
                posX = p.X;
                damping = d.Value;
            }
        );

        Assert.Equal(1, posX);
        Assert.Equal(0.9f, damping, precision: 4);
    }

    [Fact]
    public void NomadQuery_T16_ForEach_DelegateCalledForEachEntity()
    {
        World world = new();

        NomadQuery<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > query = world.CreateNomadQuery<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        const int count = 3;

        for (int i = 0; i < count; i++)
            assemblage.Create(
                new Position(),
                new Velocity(),
                new Rotation(),
                new Health(),
                new Mass(),
                new Scale(),
                new Age(),
                new Tag(),
                new Color(),
                new Force(),
                new Momentum(),
                new Gravity(),
                new Friction(),
                new Torque(),
                new Impulse(),
                new Damping()
            );

        int callCount = 0;

        query.ForEach(
            (
                ref Position _1,
                ref Velocity _2,
                ref Rotation _3,
                ref Health _4,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8,
                ref Color _9,
                ref Force _10,
                ref Momentum _11,
                ref Gravity _12,
                ref Friction _13,
                ref Torque _14,
                ref Impulse _15,
                ref Damping _16
            ) => callCount++
        );

        Assert.Equal(count, callCount);
    }

    [Fact]
    public void SettlerQuery_T16_ForEachEntity_ProvidesCorrectEntity()
    {
        World world = new();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        SettlerQuery<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > query = world.CreateSettlerQuery(assemblage);

        Entity created = assemblage.Create(
            new Position(),
            new Velocity(),
            new Rotation(),
            new Health(),
            new Mass(),
            new Scale(),
            new Age(),
            new Tag(),
            new Color(),
            new Force(),
            new Momentum(),
            new Gravity(),
            new Friction(),
            new Torque(),
            new Impulse(),
            new Damping()
        );

        Entity receivedEntity = default;

        query.ForEachEntity(
            (
                Entity e,
                ref Position _1,
                ref Velocity _2,
                ref Rotation _3,
                ref Health _4,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8,
                ref Color _9,
                ref Force _10,
                ref Momentum _11,
                ref Gravity _12,
                ref Friction _13,
                ref Torque _14,
                ref Impulse _15,
                ref Damping _16
            ) => receivedEntity = e
        );

        Assert.Equal(created._id, receivedEntity._id);
    }

    [Fact]
    public void CreateDeferred_T16_SampledComponentDataStoredCorrectly()
    {
        World world = new();

        Assemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > assemblage = world.CreateAssemblage<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        >();

        SettlerQuery<
            Position,
            Velocity,
            Rotation,
            Health,
            Mass,
            Scale,
            Age,
            Tag,
            Color,
            Force,
            Momentum,
            Gravity,
            Friction,
            Torque,
            Impulse,
            Damping
        > settlerQuery = world.CreateSettlerQuery(assemblage);

        const int initial = 5;

        for (int i = 0; i < initial; i++)
            assemblage.Create(
                new Position(),
                new Velocity(),
                new Rotation(),
                new Health(),
                new Mass(),
                new Scale(),
                new Age(),
                new Tag(),
                new Color(),
                new Force(),
                new Momentum(),
                new Gravity(),
                new Friction(),
                new Torque(),
                new Impulse(),
                new Damping()
            );

        settlerQuery.ForEach(
            (
                ref Position _1,
                ref Velocity _2,
                ref Rotation _3,
                ref Health _4,
                ref Mass _5,
                ref Scale _6,
                ref Age _7,
                ref Tag _8,
                ref Color _9,
                ref Force _10,
                ref Momentum _11,
                ref Gravity _12,
                ref Friction _13,
                ref Torque _14,
                ref Impulse _15,
                ref Damping _16
            ) =>
            {
                for (int i = 0; i < initial; i++)
                    assemblage.DeferredCreate(
                        new Position(),
                        new Velocity(),
                        new Rotation(),
                        new Health(),
                        new Mass(),
                        new Scale(),
                        new Age(),
                        new Tag(),
                        new Color(),
                        new Force(),
                        new Momentum(),
                        new Gravity(),
                        new Friction(),
                        new Torque(),
                        new Impulse(),
                        new Damping()
                    );
            }
        );

        int countBefore = initial;

        Assert.Equal(countBefore, world.CountEntitiesAlive());

        world.FlushDeferred();

        Assert.Equal(initial + (initial * initial), world.CountEntitiesAlive());
    }
}
