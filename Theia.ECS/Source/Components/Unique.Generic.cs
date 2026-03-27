using System.Runtime.CompilerServices;
using System.Threading;

namespace Theia.ECS.Components;

internal sealed class Unique<TComponent> : Unique
    where TComponent : struct
{
    private TComponent _value;
    private readonly Lock _uniqueSet = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TComponent Read() => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TComponent Get() => ref _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(TComponent component) => _value = component;

    internal void Update(UpdateUnique<TComponent> update)
    {
        lock (_uniqueSet)
        {
            update(ref _value);
        }
    }
}
