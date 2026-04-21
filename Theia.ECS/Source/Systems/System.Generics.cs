using System.Runtime.CompilerServices;
using Theia.ECS.Queries;

namespace Theia.ECS.Systems;

public abstract class System<QueryT1>(in QueryT1 query) : BaseSystem
    where QueryT1 : Query
{
    private readonly QueryT1 _query = query;

    internal override void Run() => ExecuteQueryT1(in _query);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void ExecuteQueryT1(in QueryT1 query);
}

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual TSharedData CreateData() => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void ExecuteQueryT1(ref TSharedData sharedData, in QueryT1 query);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void ExecuteQueryT2(ref TSharedData sharedData, in QueryT2 query);
}
