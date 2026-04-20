using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Systems;

public abstract class BaseSystem : IDisposable
{
    public virtual void Before() { }

    internal abstract void Run();

    public virtual void After() { }

    public virtual void Dispose() { }
}

public abstract class System : BaseSystem
{
    internal override void Run() => Execute();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Execute();
}
