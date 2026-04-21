using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Theia.ECS.Jobs;

internal struct JobEntry
{
    internal Job? Job;
    internal CountdownEvent? Countdown;
}

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
            entry.Job!.Execute();
            entry.Countdown!.Signal();
        }
    }

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
            s_queue.Enqueue(new JobEntry { Job = jobs[i], Countdown = countdown });

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
