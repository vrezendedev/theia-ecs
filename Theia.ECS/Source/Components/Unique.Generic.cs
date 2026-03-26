using System.Runtime.CompilerServices;
using System.Threading;

namespace Theia.ECS.Components;

internal sealed class Unique<TComponent> : Unique
    where TComponent : struct
{
    private TComponent _value;
    private readonly Lock _uniqueQueryLock = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TComponent Read() => _value;

    internal void Set(SetUnique<TComponent> set)
    {
        lock (_uniqueQueryLock)
        {
            set(ref _value);
        }
    }
}
