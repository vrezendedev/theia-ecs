using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Theia.ECS.Jobs;

/// <summary>
/// Queue entry pairing a <see cref="Job"/> with the <see cref="CountdownEvent"/> that tracks
/// completion of its enclosing batch. The submitting thread waits on the countdown until every
/// entry it enqueued has signaled.
/// </summary>
internal struct JobEntry
{
    internal Job? _job;
    internal CountdownEvent? _countdown;
}

/// <summary>
/// Process-wide work-stealing-style scheduler that runs batches of <see cref="Job"/> instances
/// across a pool of background worker threads, blocking the caller until the entire batch is
/// finished.
/// </summary>
/// <remarks>
/// <para>
/// On startup the scheduler launches <c>ProcessorCount - 1</c> worker threads (or zero on
/// single-core hosts), each running a loop that waits on a semaphore for available work and
/// drains the shared <see cref="ConcurrentQueue{T}"/> when signaled.
/// <br/>
/// Workers are background threads at <see cref="ThreadPriority.AboveNormal"/>,
/// so they will not keep the process alive on shutdown but will preempt ordinary
/// user work while a batch is in flight.
/// </para>
/// <para>
/// <see cref="Run"/> is the only entry point. The submitting thread participates in draining
/// the queue alongside the workers, so <c>Run</c> contributes its own CPU rather than blocking
/// idle while workers do all the work. Once every job in the batch has signaled the shared
/// <see cref="CountdownEvent"/>, <c>Run</c> returns; the countdown is then recycled into a pool
/// for the next batch.
/// </para>
/// <para>
/// Two fast paths skip the scheduler entirely and run jobs inline on the calling thread: when
/// the host has no worker threads, and when the batch contains a single job. In both cases the
/// scheduling overhead would dominate the work itself.
/// </para>
/// </remarks>
public static class JobScheduler
{
    private static readonly int s_workersCount;
    private static readonly Thread[] s_workers;

    private static readonly ConcurrentQueue<JobEntry> s_queue = new();
    private static readonly SemaphoreSlim s_workAvailable = new(0);

    private static readonly Stack<CountdownEvent> s_countdownPool = new();
    private static readonly Lock s_countdownPoolLock = new();

    static JobScheduler()
    {
        s_workersCount = Math.Max(0, Environment.ProcessorCount - 1);
        s_workers = new Thread[s_workersCount];

        for (int i = 0; i < s_workersCount; i++)
        {
            Thread thread = new(WorkerLoop)
            {
                IsBackground = true,
                Name = $"Theia.ECS.Jobs.JobWorker[{i}]",
                Priority = ThreadPriority.AboveNormal,
            };

            s_workers[i] = thread;

            thread.Start();
        }
    }

    private static bool ShouldSkipScheduling(int jobCount) => s_workersCount == 0 || jobCount == 1;

    private static void WorkerLoop()
    {
        while (true)
        {
            s_workAvailable.Wait();
            DrainQueue();
        }
    }

    private static void DrainQueue()
    {
        while (s_queue.TryDequeue(out JobEntry entry))
        {
            entry._job!.Execute();
            entry._countdown!.Signal();
        }
    }

    /// <summary>
    /// Runs <paramref name="jobs"/> in parallel and blocks until every job has finished. An
    /// empty span returns immediately; a single-job span and the no-workers case run inline on
    /// the calling thread without involving the queue.
    /// </summary>
    /// <remarks>
    /// The submitting thread participates in draining the queue, so it contributes work rather
    /// than parking. <see cref="CountdownEvent"/> instances are pooled across calls.
    /// </remarks>
    public static void Run(ReadOnlySpan<Job> jobs)
    {
        int count = jobs.Length;

        if (count == 0)
            return;

        if (ShouldSkipScheduling(count))
        {
            for (int i = 0; i < count; i++)
                jobs[i].Execute();

            return;
        }

        CountdownEvent countdown = RentCountdown(count);

        for (int i = 0; i < count; i++)
            s_queue.Enqueue(new JobEntry { _job = jobs[i], _countdown = countdown });

        s_workAvailable.Release(Math.Min(count, s_workersCount));

        DrainQueue();

        countdown.Wait();

        ReturnCountdown(countdown);
    }

    private static CountdownEvent RentCountdown(int count)
    {
        lock (s_countdownPoolLock)
        {
            if (s_countdownPool.TryPop(out CountdownEvent? countdown))
            {
                countdown.Reset(count);
                return countdown;
            }
        }

        return new CountdownEvent(count);
    }

    private static void ReturnCountdown(CountdownEvent countdown)
    {
        lock (s_countdownPoolLock)
            s_countdownPool.Push(countdown);
    }
}
