using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Unique[] _uniques = Array.Empty<Unique>();
    private readonly Lock _uniquesLock = new();

    /// <summary>
    /// Returns the <see cref="Unique"/> for <paramref name="componentId"/>, creating it via the
    /// component's <see cref="ComponentType.CreateUnique"/> factory on first access. Used by
    /// the deserialization path, where the concrete component type is not known statically.
    /// </summary>
    /// <remarks>
    /// Uses double-checked locking: the fast path reads the unique array via a local snapshot
    /// without entering the lock, and the lock is acquired only on first creation.
    /// </remarks>
    internal Unique GetOrCreateUnique(int componentId)
    {
        Unique[] uniques = _uniques;

        if (IsUniqueAlreadyCreated(componentId, uniques))
            return uniques[componentId];

        lock (_uniquesLock)
        {
            if (componentId >= _uniques.Length)
                Array.Resize(ref _uniques, componentId + 1);

            if (_uniques[componentId] is null)
                _uniques[componentId] = ComponentsMeta.GetComponentType(componentId).CreateUnique();

            return _uniques[componentId];
        }
    }

    private Unique<TComponent> GetOrCreateUnique<TComponent>()
        where TComponent : struct
    {
        int componentId = ComponentMeta<TComponent>.s_id;

        Unique[] uniques = _uniques;

        if (IsUniqueAlreadyCreated(componentId, uniques))
            return (Unique<TComponent>)uniques[componentId];

        lock (_uniquesLock)
        {
            if (componentId >= _uniques.Length)
                Array.Resize(ref _uniques, componentId + 1);

            if (_uniques[componentId] is null)
                _uniques[componentId] = new Unique<TComponent>();

            return (Unique<TComponent>)_uniques[componentId];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsUniqueAlreadyCreated(int componentId, Unique[] snapshot) =>
        componentId < snapshot.Length && snapshot[componentId] is not null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<Unique> GetUniques() => _uniques;

    /// <summary>
    /// <b>Returns a copy</b> of the world's unique <typeparamref name="TComponent"/>, creating it
    /// at its default value on first access. Unsynchronized: see <see cref="QueryUnique"/> for
    /// the synchronized alternative.
    /// </summary>
    public TComponent ReadUnique<TComponent>()
        where TComponent : struct => GetOrCreateUnique<TComponent>().Read();

    /// <summary>
    /// <b>Returns a mutable reference</b> to the world's unique <typeparamref name="TComponent"/>,
    /// creating it at its default value on first access. Unsynchronized: callers concurrently
    /// mutating the same unique should use <see cref="QueryUnique"/> instead.
    /// </summary>
    public ref TComponent GetUnique<TComponent>()
        where TComponent : struct => ref GetOrCreateUnique<TComponent>().Get();

    /// <summary>
    /// Replaces the world's unique <typeparamref name="TComponent"/> with
    /// <paramref name="component"/>, creating the unique at its default value first if it does
    /// not yet exist. Unsynchronized.
    /// </summary>
    public void SetUnique<TComponent>(in TComponent component)
        where TComponent : struct => GetOrCreateUnique<TComponent>().Set(component);

    /// <summary>
    /// Mutates the world's unique <typeparamref name="TComponent"/> under the unique's
    /// internal lock by invoking <paramref name="query"/>, which receives a <c>ref</c> to the
    /// unique value. <b>Use this when multiple systems may concurrently mutate the same unique</b>.
    /// </summary>
    /// <typeparam name="TComponent">The unique component type to mutate.</typeparam>
    /// <typeparam name="T">
    /// The callback type, which may be a <c>ref struct</c> to allow capturing spans or other
    /// stack-only state.
    /// </typeparam>
    public void QueryUnique<TComponent, T>(ref T query)
        where TComponent : struct
        where T : struct, IUniqueQuery<TComponent>, allows ref struct =>
        GetOrCreateUnique<TComponent>().Query(ref query);
}
