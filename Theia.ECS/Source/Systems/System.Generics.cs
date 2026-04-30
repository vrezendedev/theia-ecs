using System.Runtime.CompilerServices;
using Theia.ECS.Queries;

namespace Theia.ECS.Systems;

/// <summary>
/// System bound to a single <see cref="Query"/>. Override <see cref="ExecuteQueryT1"/> and use
/// the supplied query to iterate over its matched entities, typically with an
/// <see cref="IForEach{T}"/> or <see cref="IForEachEntity{T}"/> struct or ref struct that holds the callback.
/// </summary>
/// <param name="query">The query whose results will be supplied to <see cref="ExecuteQueryT1"/> on every tick.</param>
public abstract class System<QueryT1>(in QueryT1 query) : BaseSystem
    where QueryT1 : Query
{
    private readonly QueryT1 _query = query;

    internal override void Run() => ExecuteQueryT1(in _query);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void ExecuteQueryT1(in QueryT1 query);
}

/// <summary>
/// System bound to two <see cref="Query"/> instances that share a per-tick state struct of type
/// <typeparamref name="TSharedData"/>. Each tick, the scheduler creates a fresh
/// <typeparamref name="TSharedData"/> via <see cref="CreateData"/> and passes it by reference
/// through <see cref="ExecuteQueryT1"/> and then <see cref="ExecuteQueryT2"/>, so each phase
/// can read or update state produced by the previous one.
/// </summary>
/// <typeparam name="TSharedData">
/// Per-tick state passed by reference between the execute phases. May be a <c>ref struct</c>,
/// allowing the shared state to hold spans or other stack-only references.
/// </typeparam>
/// <typeparam name="QueryT1">Type of the first bound query.</typeparam>
/// <typeparam name="QueryT2">Type of the second bound query.</typeparam>
/// <param name="queryT1">The query supplied to <see cref="ExecuteQueryT1"/> each tick.</param>
/// <param name="queryT2">The query supplied to <see cref="ExecuteQueryT2"/> each tick.</param>
/// <remarks>
/// Use this variant when two queries cooperate, for example: the first phase accumulates
/// per-entity results into the shared struct, and the second phase consumes them. Each phase
/// drives its query the same way as <see cref="System{QueryT1}"/>, typically via an
/// <see cref="IForEach{T}"/> or <see cref="IForEachEntity{T}"/> struct or ref struct, but with the shared state available to read and mutate.
/// </remarks>
public abstract class System<TSharedData, QueryT1, QueryT2>(in QueryT1 queryT1, in QueryT2 queryT2)
    : BaseSystem
    where TSharedData : struct, allows ref struct
    where QueryT1 : Query
    where QueryT2 : Query
{
    private readonly QueryT1 _queryT1 = queryT1;
    private readonly QueryT2 _queryT2 = queryT2;

    internal override void Run()
    {
        TSharedData sharedData = CreateData();

        ExecuteQueryT1(ref sharedData, in _queryT1);
        ExecuteQueryT2(ref sharedData, in _queryT2);
    }

    /// <summary>
    /// Produces the per-tick shared-state value passed to the execute phases. The default
    /// returns <see langword="default"/>; override to seed a non-default initial state.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual TSharedData CreateData() => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void ExecuteQueryT1(ref TSharedData sharedData, in QueryT1 query);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void ExecuteQueryT2(ref TSharedData sharedData, in QueryT2 query);
}
