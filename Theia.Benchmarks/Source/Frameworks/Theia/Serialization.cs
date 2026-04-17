using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Categories;

public abstract class TheiaSerialization
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    [IterationSetup]
    public abstract void Setup();

    [IterationCleanup]
    public abstract void CleanUp();

    protected static World CreatePopulatedWorld(int entityCount)
    {
        World world = new();

        Assemblage<Position> positionOnly = world.CreateAssemblage<Position>();
        Assemblage<Health> healthOnly = world.CreateAssemblage<Health>();
        Assemblage<Inventory> inventoryOnly = world.CreateAssemblage<Inventory>();

        world.SetUnique(new WorldTick { Value = 1 });

        Entity hero = positionOnly.Create(
            new Position
            {
                X = 1f,
                Y = 0f,
                Z = 1f,
            }
        );
        world.TryAddComponent(hero, new Health { Current = 100, Max = 100 });
        world.TryAddComponent(
            hero,
            new Velocity
            {
                X = 1f,
                Y = 0f,
                Z = 0f,
            }
        );
        world.TryAddComponent(hero, new Rotation { Yaw = 45f });
        world.TryAddComponent(hero, new Level { Value = 10 });
        world.TryAddComponent(hero, new Inventory { Gold = 999, Items = 20 });

        Entity[] enemies = new Entity[entityCount];
        Random rng = new(42);

        for (int i = 0; i < entityCount; i++)
        {
            enemies[i] = healthOnly.Create(new Health { Current = rng.Next(20, 100), Max = 100 });

            world.TryAddComponent(
                enemies[i],
                new Position { X = rng.NextSingle() * 100f, Z = rng.NextSingle() * 100f }
            );

            world.TryAddComponent(
                enemies[i],
                new Velocity { X = rng.NextSingle() * 2f - 1f, Z = rng.NextSingle() * 2f - 1f }
            );

            if (i % 3 == 0)
                world.TryAddComponent(enemies[i], new Level { Value = rng.Next(1, 10) });
        }

        int itemCount = Math.Min(entityCount, 64);
        Entity[] items = new Entity[itemCount];

        for (int i = 0; i < itemCount; i++)
            items[i] = inventoryOnly.Create(new Inventory { Gold = i * 10, Items = 1 });

        for (int i = 0; i < itemCount; i++)
            world.TryAddTagRelation<Owns>(hero, items[i]);

        int followerCount = Math.Min(entityCount / 4, 32);

        for (int i = 1; i < followerCount; i++)
            world.TryAddTagRelation<Follows>(enemies[i], enemies[0]);

        return world;
    }
}

[BenchmarkCategory(nameof(TheiaSerialize))]
public class TheiaSerialize : TheiaSerialization
{
    private const string Path = "serialize_bench_world.bin";
    private World _world = null!;

    public override void Setup()
    {
        _world = CreatePopulatedWorld(EntityCount);
    }

    public override void CleanUp()
    {
        if (File.Exists(Path))
            File.Delete(Path);
    }

    [Benchmark]
    public void Run() => _world.Serialize(Path);
}

[BenchmarkCategory(nameof(TheiaDeserialize))]
public class TheiaDeserialize : TheiaSerialization
{
    private const string Path = "deserialize_bench_world.bin";
    private World _world = null!;

    public override void Setup()
    {
        CreatePopulatedWorld(EntityCount).Serialize(Path);

        _world = new World();
    }

    public override void CleanUp()
    {
        if (File.Exists(Path))
            File.Delete(Path);
    }

    [Benchmark]
    public void Run() => _world.Deserialize(Path);
}
