using System.Collections.Generic;
using System.Threading;

namespace Theia.ECS.Jobs;

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

    public static TJob Rent()
    {
        lock (s_lock)
        {
            if (s_pool.TryPop(out TJob? job))
                return job;
        }

        return new TJob();
    }

    public static void Return(TJob job)
    {
        lock (s_lock)
            s_pool.Push(job);
    }
}
