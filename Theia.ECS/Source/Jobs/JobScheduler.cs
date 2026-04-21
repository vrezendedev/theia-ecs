using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Theia.ECS.Jobs;

internal static class JobScheduler
{
    private const int MinJobsPerWorker = 1;
    internal const int MaxJobs = 4_096;

    private static readonly int s_workersCount;
    private static readonly Thread[] s_workers;

    private static readonly Job?[] s_jobs;

    private static readonly SemaphoreSlim s_workAvailable;
    private static readonly ManualResetEventSlim s_batchComplete;

    private static volatile int s_batchCount;
    private static volatile int s_nextJob;
    private static volatile int s_activeWorkers;

    static JobScheduler()
    {
        s_workersCount = Math.Max(0, Environment.ProcessorCount - 1);
        s_workers = new Thread[s_workersCount];

        s_jobs = new Job[MaxJobs];

        s_workAvailable = new(0);
        s_batchComplete = new(true);

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

    private static void WorkerLoop()
    {
        while (true)
        {
            s_workAvailable.Wait();

            ClaimJob();

            if (Interlocked.Decrement(ref s_activeWorkers) == 0)
                s_batchComplete.Set();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClaimJob()
    {
        int claimed;

        while ((claimed = Interlocked.Increment(ref s_nextJob) - 1) < s_batchCount)
            s_jobs[claimed]!.Execute();
    }

    internal static bool ShouldSkipScheduling(int jobCount) =>
        s_workersCount == 0 || jobCount < (s_workersCount * MinJobsPerWorker);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Schedule(int index, Job job) => s_jobs[index] = job;

    internal static void Run(int jobCount)
    {
        if (jobCount == 0)
            return;

        s_batchCount = jobCount;
        s_nextJob = 0;
        s_activeWorkers = s_workersCount + 1;

        s_batchComplete.Reset();
        s_workAvailable.Release(s_workersCount);

        ClaimJob();

        if (Interlocked.Decrement(ref s_activeWorkers) == 0)
            s_batchComplete.Set();

        s_batchComplete.Wait();
    }
}
