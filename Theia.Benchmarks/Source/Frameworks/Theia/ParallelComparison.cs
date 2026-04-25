using System;
using BenchmarkDotNet.Attributes;
using Theia.Benchmarks.Source.Resources;
using Theia.ECS.Assemblages;
using Theia.ECS.Queries;
using Theia.ECS.Worlds;

namespace Theia.Benchmarks.Source.Categories;

/// <summary>
///  Sinusoidal force field with Euler integration.
/// </summary>
file static class ForceFieldIntegrator
{
    private const float Frequency = 0.05f;
    private const float Amplitude = 9.81f;
    private const float Damping = 0.98f;

    public static void Integrate(ref Position position, ref Velocity velocity, float dt)
    {
        float fx =
            MathF.Sin(position.Y * Frequency) * MathF.Cos(position.Z * Frequency) * Amplitude;
        float fy =
            MathF.Sin(position.Z * Frequency) * MathF.Cos(position.X * Frequency) * Amplitude;
        float fz =
            MathF.Sin(position.X * Frequency) * MathF.Cos(position.Y * Frequency) * Amplitude;

        velocity.X = (velocity.X + fx * dt) * Damping;
        velocity.Y = (velocity.Y + fy * dt) * Damping;
        velocity.Z = (velocity.Z + fz * dt) * Damping;

        position.X += velocity.X * dt;
        position.Y += velocity.Y * dt;
        position.Z += velocity.Z * dt;
    }
}

public abstract class TheiaParallelComparison
{
    [Params(16, 256, 512, 1_024, 4_096, 8_192, 16_384, 32_768)]
    public int EntityCount { get; set; }

    protected World World = null!;
    protected SettlerQuery<Position, Velocity> Query = null!;

    [IterationSetup]
    public void Setup()
    {
        World world = new();

        Assemblage<Position, Velocity> assemblage = world.CreateAssemblage<Position, Velocity>();
        Query = world.CreateSettlerQuery(assemblage);

        for (int i = 0; i < EntityCount; i++)
            assemblage.Create(
                new Position()
                {
                    X = i,
                    Y = 1,
                    Z = i,
                },
                new Velocity()
                {
                    X = i,
                    Y = 1,
                    Z = 1,
                }
            );
    }

    [IterationCleanup]
    public void CleanUp()
    {
        World = null!;
        Query = null!;
    }
}

file struct ForEach : IForEach<Position, Velocity>
{
    public void Execute(ref Position pos, ref Velocity velocity) =>
        ForceFieldIntegrator.Integrate(ref pos, ref velocity, 0.16f);
}

[BenchmarkCategory(nameof(TheiaParallelComparison))]
public class TheiaParallelComparisonSequential : TheiaParallelComparison
{
    [Benchmark]
    public void Run()
    {
        ForEach forEach = new();
        Query.ForEach(ref forEach);
    }
}

[BenchmarkCategory(nameof(TheiaParallelComparison))]
public class TheiaParallelComparisonParallel : TheiaParallelComparison
{
    [Benchmark]
    public void Run()
    {
        ForEach forEach = new();
        Query.ForEachParallel(forEach);
    }
}
