namespace Theia.ECS.Components;

/// <summary>
/// Callback contract for mutating the underlying value held in a
/// <see cref="Unique{TComponent}"/> under its internal lock. Implementations may be
/// <c>ref struct</c>, allowing the callback to capture spans or other stack-only state.
/// </summary>
/// <typeparam name="TComponent">The component type held by the target <see cref="Unique{TComponent}"/>.</typeparam>
/// <remarks>
/// Mirrors the <c>IForEach&lt;T&gt;</c> pattern used by entity queries. The framework holds the
/// lock and invokes <see cref="Execute"/> with a <c>ref</c> to the underlying value, so the callback
/// can read and write atomically without ever seeing a raw lock object.
/// </remarks>
public interface IUniqueQuery<TComponent>
    where TComponent : struct
{
    /// <summary>Called with a reference to the unique value while its owning lock is held.</summary>
    public void Execute(ref TComponent component);
}
