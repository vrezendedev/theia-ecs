namespace Theia.ECS.Jobs;

/// <summary>
/// Unit of parallel work dispatched through <see cref="JobScheduler"/>. Override
/// <see cref="Execute"/> with the work to run; the scheduler invokes it on a worker thread,
/// independently of any other job in the same batch.
/// </summary>
public abstract class Job
{
    /// <summary>The work performed by this job. Runs on a worker thread.</summary>
    public abstract void Execute();
}
