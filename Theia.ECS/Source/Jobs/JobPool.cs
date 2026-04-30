using System.Collections.Generic;
using System.Threading;

namespace Theia.ECS.Jobs;

/// <summary>
/// Per-type object pool for <see cref="Job"/> subclasses with a parameterless constructor.
/// <b>Pre-allocates a small batch on first access</b> and hands instances back out via <see cref="Rent"/>;
/// callers return them with <see cref="Return"/> when they are finished.
/// </summary>
/// <remarks>
/// <para>
/// The pool does <b>not</b> reset returned jobs. Whatever state a job carried at <see cref="Return"/>
/// is what the next caller will observe at <see cref="Rent"/>; <b>callers are responsible for
/// re-initializing rented jobs</b> before invoking <see cref="Job.Execute"/>, or for ensuring their
/// jobs are effectively stateless across uses.
/// </para>
/// <para>
/// Each closed generic instantiation has its own static pool, so <c>JobPool&lt;FooJob&gt;</c> and
/// <c>JobPool&lt;BarJob&gt;</c> are independent.
/// <br/>
/// Rent and return are thread-safe; on an empty
/// pool, <see cref="Rent"/> falls through to <c>new TJob()</c> rather than blocking.
/// </para>
/// </remarks>
public static class JobPool<TJob>
    where TJob : Job, new()
{
    private const int InitialCapacity = 4;

    private static readonly Stack<TJob> s_pool = new();
    private static readonly Lock s_lock = new();

    static JobPool()
    {
        for (int i = 0; i < InitialCapacity; i++)
            s_pool.Push(new TJob());
    }

    /// <summary>
    /// Returns a pooled <typeparamref name="TJob"/> if one is available; otherwise allocates a
    /// fresh instance via <c>new TJob()</c>.
    /// </summary>
    public static TJob Rent()
    {
        lock (s_lock)
        {
            if (s_pool.TryPop(out TJob? job))
                return job;
        }

        return new TJob();
    }

    /// <summary>
    /// Returns <paramref name="job"/> to the pool for later reuse. The pool retains whatever
    /// state the job currently holds; callers that care about clearing sensitive or stale data
    /// must do so before calling <see cref="Return"/>.
    /// </summary>
    public static void Return(TJob job)
    {
        lock (s_lock)
            s_pool.Push(job);
    }
}
