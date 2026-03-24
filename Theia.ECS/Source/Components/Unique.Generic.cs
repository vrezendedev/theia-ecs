namespace Theia.ECS.Components;

internal sealed class Unique<T> : Unique
    where T : struct
{
    private T _value;

    internal T Read() => _value;

    internal ref T Get() => ref _value;

    internal void Set(in T value) => _value = value;
}
