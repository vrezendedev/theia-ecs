using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Queries;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.SourceGenerated;

public sealed class SourceGeneratedTests
{
    private ref struct ForEachPositionVelocityCount : IForEach<Position, Velocity>
    {
        public int CallCount;

        public void Execute(ref Position c1, ref Velocity c2) => CallCount++;
    }

    private ref struct ForEachPositionVelocityReadSum : IForEach<Position, Velocity>
    {
        public float SumX;
        public float SumY;

        public void Execute(ref Position c1, ref Velocity c2)
        {
            SumX = c1.X + c2.X;
            SumY = c1.Y + c2.Y;
        }
    }

    private ref struct ForEachPositionVelocityMutate : IForEach<Position, Velocity>
    {
        public int PositionX;
        public int VelocityX;
        public int VelocityY;

        public void Execute(ref Position c1, ref Velocity c2)
        {
            c1.X = PositionX;
            c2.X = VelocityX;
            c2.Y = VelocityY;
        }
    }

    private ref struct ForEachEntityPositionVelocityCapture : IForEachEntity<Position, Velocity>
    {
        public Entity Entity;
        public float PositionX;
        public float VelocityX;

        public void Execute(Entity entity, ref Position c1, ref Velocity c2)
        {
            Entity = entity;
            PositionX = c1.X;
            VelocityX = c2.X;
        }
    }

    private ref struct ForEachT4Count : IForEach<Position, Velocity, Rotation, Health>
    {
        public int CallCount;

        public void Execute(ref Position c1, ref Velocity c2, ref Rotation c3, ref Health c4) =>
            CallCount++;
    }

    private ref struct ForEachT4ReadPositionVelocity
        : IForEach<Position, Velocity, Rotation, Health>
    {
        public float PositionX;
        public float VelocityX;

        public void Execute(ref Position c1, ref Velocity c2, ref Rotation c3, ref Health c4)
        {
            PositionX = c1.X;
            VelocityX = c2.X;
        }
    }

    private ref struct ForEachEntityT4CaptureEntity
        : IForEachEntity<Position, Velocity, Rotation, Health>
    {
        public Entity Entity;

        public void Execute(
            Entity entity,
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4
        ) => Entity = entity;
    }

    private ref struct ForEachT8Count
        : IForEach<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>
    {
        public int CallCount;

        public void Execute(
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4,
            ref Mass c5,
            ref Scale c6,
            ref Age c7,
            ref Tag c8
        ) => CallCount++;
    }

    private ref struct ForEachT8ReadSampled
        : IForEach<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>
    {
        public float PositionX;
        public float Mass;
        public int Age;
        public int Tag;

        public void Execute(
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4,
            ref Mass c5,
            ref Scale c6,
            ref Age c7,
            ref Tag c8
        )
        {
            PositionX = c1.X;
            Mass = c5.Value;
            Age = c7.Value;
            Tag = c8.Value;
        }
    }

    private ref struct ForEachEntityT8CaptureEntity
        : IForEachEntity<Position, Velocity, Rotation, Health, Mass, Scale, Age, Tag>
    {
        public Entity Entity;

        public void Execute(
            Entity entity,
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4,
            ref Mass c5,
            ref Scale c6,
            ref Age c7,
            ref Tag c8
        ) => Entity = entity;
    }

    private ref struct ForEachT16Count
        : IForEach<
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
        >
    {
        public int CallCount;

        public void Execute(
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4,
            ref Mass c5,
            ref Scale c6,
            ref Age c7,
            ref Tag c8,
            ref Color c9,
            ref Force c10,
            ref Momentum c11,
            ref Gravity c12,
            ref Friction c13,
            ref Torque c14,
            ref Impulse c15,
            ref Damping c16
        ) => CallCount++;
    }

    private ref struct ForEachT16ReadFirstLast
        : IForEach<
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
        >
    {
        public float PositionX;
        public float Damping;

        public void Execute(
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4,
            ref Mass c5,
            ref Scale c6,
            ref Age c7,
            ref Tag c8,
            ref Color c9,
            ref Force c10,
            ref Momentum c11,
            ref Gravity c12,
            ref Friction c13,
            ref Torque c14,
            ref Impulse c15,
            ref Damping c16
        )
        {
            PositionX = c1.X;
            Damping = c16.Value;
        }
    }

    private ref struct ForEachEntityT16CaptureEntity
        : IForEachEntity<
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
        >
    {
        public Entity Entity;

        public void Execute(
            Entity entity,
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4,
            ref Mass c5,
            ref Scale c6,
            ref Age c7,
            ref Tag c8,
            ref Color c9,
            ref Force c10,
            ref Momentum c11,
            ref Gravity c12,
            ref Friction c13,
            ref Torque c14,
            ref Impulse c15,
            ref Damping c16
        ) => Entity = entity;
    }

    private ref struct ForEachT16DeferredCreate
        : IForEach<
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
        >
    {
        public Assemblage<
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
        > Assemblage;
        public int Count;

        public void Execute(
            ref Position c1,
            ref Velocity c2,
            ref Rotation c3,
            ref Health c4,
            ref Mass c5,
            ref Scale c6,
            ref Age c7,
            ref Tag c8,
            ref Color c9,
            ref Force c10,
            ref Momentum c11,
            ref Gravity c12,
            ref Friction c13,
            ref Torque c14,
            ref Impulse c15,
            ref Damping c16
        )
        {
            for (int i = 0; i < Count; i++)
                Assemblage.DeferredCreate(
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
    }

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

        int countBefore = world.CountEntities();

        assemblage.DeferredCreate(new Position() { X = 9 }, new Velocity() { X = 4 });

        Assert.Equal(countBefore, world.CountEntities());

        world.FlushDeferred();

        Assert.Equal(countBefore + 1, world.CountEntities());
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

        ForEachPositionVelocityCount forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
    }

    [Fact]
    public void SettlerQuery_T2_ForEach_CanReadBothComponents()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        SettlerQuery<Position, Velocity> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position() { X = 3, Y = 7 }, new Velocity() { X = 5, Y = 11 });

        ForEachPositionVelocityReadSum forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(8, forEach.SumX);
        Assert.Equal(18, forEach.SumY);
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

        ForEachPositionVelocityMutate forEach = new() { PositionX = 99, VelocityY = 77 };

        query.ForEach(ref forEach);

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

        ForEachEntityPositionVelocityCapture forEach = new();

        query.ForEachEntity(ref forEach);

        Assert.Equal(created._id, forEach.Entity._id);
        Assert.Equal(10, forEach.PositionX);
        Assert.Equal(20, forEach.VelocityX);
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

        ForEachPositionVelocityCount forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
    }

    [Fact]
    public void NomadQuery_T2_ForEach_CanMutateBothComponents()
    {
        World world = new();

        NomadQuery<Position, Velocity> query = world.CreateNomadQuery<Position, Velocity>();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();

        Entity entity = assemblage.Create(new Position() { X = 1 }, new Velocity() { X = 2 });

        ForEachPositionVelocityMutate forEach = new() { PositionX = 55, VelocityX = 66 };

        query.ForEach(ref forEach);

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

        ForEachEntityPositionVelocityCapture forEach = new();

        query.ForEachEntity(ref forEach);

        Assert.Equal(created._id, forEach.Entity._id);
        Assert.Equal(7, forEach.PositionX);
        Assert.Equal(13, forEach.VelocityX);
    }

    [Fact]
    public void NomadQuery_T2_ArchetypeWithSupersetOfComponents_IsMatched()
    {
        World world = new();

        NomadQuery<Position, Velocity> query = world.CreateNomadQuery<Position, Velocity>();

        Assemblage<Position, Velocity> base_ = world.CreateAssemblage<Position, Velocity>();

        Entity entity = base_.Create(new Position() { X = 1 }, new Velocity() { X = 2 });

        world.TryAddComponent<Rotation>(entity);

        ForEachPositionVelocityCount forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(1, forEach.CallCount);
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

        ForEachT4Count forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
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

        ForEachT4ReadPositionVelocity forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(1, forEach.PositionX);
        Assert.Equal(2, forEach.VelocityX);
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

        ForEachT4Count forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
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

        ForEachEntityT4CaptureEntity forEach = new();

        query.ForEachEntity(ref forEach);

        Assert.Equal(created._id, forEach.Entity._id);
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

        ForEachT8Count forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
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

        ForEachT8ReadSampled forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(1, forEach.PositionX);
        Assert.Equal(3f, forEach.Mass);
        Assert.Equal(4, forEach.Age);
        Assert.Equal(5, forEach.Tag);
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

        ForEachT8Count forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
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

        ForEachEntityT8CaptureEntity forEach = new();

        query.ForEachEntity(ref forEach);

        Assert.Equal(created._id, forEach.Entity._id);
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

        ForEachT16Count forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
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

        ForEachT16ReadFirstLast forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(1, forEach.PositionX);
        Assert.Equal(0.9f, forEach.Damping, precision: 4);
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

        ForEachT16Count forEach = new();

        query.ForEach(ref forEach);

        Assert.Equal(count, forEach.CallCount);
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

        ForEachEntityT16CaptureEntity forEach = new();

        query.ForEachEntity(ref forEach);

        Assert.Equal(created._id, forEach.Entity._id);
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

        ForEachT16DeferredCreate forEach = new() { Assemblage = assemblage, Count = initial };

        settlerQuery.ForEach(ref forEach);

        Assert.Equal(initial, world.CountEntities());

        world.FlushDeferred();

        Assert.Equal(initial + (initial * initial), world.CountEntities());
    }
}
