using System.Collections.Generic;
using Theia.ECS.Assemblages;
using Theia.ECS.Queries;
using Theia.ECS.Systems;
using Theia.ECS.Worlds;
using Theia.Tests.Resources;

namespace Theia.Tests.ECS.Systems;

public sealed class SystemsTest
{
    [Fact]
    public void System_Execute_IsCalledOnRootExecute()
    {
        TrackingSystem system = new();

        SystemsRoot root = new(system);

        root.Execute();

        Assert.Equal(1, system.ExecuteCallCount);
    }

    [Fact]
    public void System_Execute_IsCalledEveryTick()
    {
        TrackingSystem system = new();

        SystemsRoot root = new(system);

        const int ticks = 5;

        for (int i = 0; i < ticks; i++)
            root.Execute();

        Assert.Equal(ticks, system.ExecuteCallCount);
    }

    [Fact]
    public void System_BeforeAndAfter_AreCalledInOrder()
    {
        OrderTrackingSystem system = new();

        SystemsRoot root = new(system);

        root.Execute();

        Assert.Equal(new[] { "Before", "Execute", "After" }, system.CallOrder);
    }

    [Fact]
    public void SystemWithQuery_ExecuteQueryT1_IsCalledOnRootExecute()
    {
        World world = new();

        Assemblage<Position> assemblage = world.CreateAssemblage<Position>();
        SettlerQuery<Position> query = world.CreateSettlerQuery(assemblage);

        assemblage.Create(new Position { X = 1 });

        QueryTrackingSystem system = new(query);

        SystemsRoot root = new(system);

        root.Execute();

        Assert.Equal(1, system.ForEachCallCount);
    }

    [Fact]
    public void SystemsRoot_ExecutesSystemsInRegistrationOrder()
    {
        List<int> order = new();

        OrderedSystem systemA = new(order, 1);
        OrderedSystem systemB = new(order, 2);
        OrderedSystem systemC = new(order, 3);

        SystemsRoot root = new(systemA, systemB, systemC);

        root.Execute();

        Assert.Equal(new[] { 1, 2, 3 }, order);
    }

    [Fact]
    public void SystemsRoot_Dispose_CallsDisposeOnAllSystems()
    {
        DisposableSystem systemA = new();
        DisposableSystem systemB = new();

        SystemsRoot root = new(systemA, systemB);

        root.Dispose();

        Assert.True(systemA.Disposed);
        Assert.True(systemB.Disposed);
    }
}

file sealed class TrackingSystem : Theia.ECS.Systems.System
{
    public int ExecuteCallCount;

    public override void Execute() => ExecuteCallCount++;
}

file sealed class OrderTrackingSystem : Theia.ECS.Systems.System
{
    public List<string> CallOrder = new();

    public override void Before() => CallOrder.Add("Before");

    public override void Execute() => CallOrder.Add("Execute");

    public override void After() => CallOrder.Add("After");
}

file sealed class QueryTrackingSystem : System<SettlerQuery<Position>>
{
    public int ForEachCallCount;

    public QueryTrackingSystem(SettlerQuery<Position> query)
        : base(query) { }

    private ref struct TrackingForEach : IForEach<Position>
    {
        public int CallCount;

        public void Execute(ref Position position) => CallCount++;
    }

    public override void ExecuteQueryT1(in SettlerQuery<Position> query)
    {
        TrackingForEach forEach = new();
        query.ForEach(ref forEach);
        ForEachCallCount += forEach.CallCount;
    }
}

file sealed class OrderedSystem : Theia.ECS.Systems.System
{
    private readonly List<int> _order;
    private readonly int _id;

    public OrderedSystem(List<int> order, int id)
    {
        _order = order;
        _id = id;
    }

    public override void Execute() => _order.Add(_id);
}

file sealed class DisposableSystem : Theia.ECS.Systems.System
{
    public bool Disposed;

    public override void Execute() { }

    public override void Dispose() => Disposed = true;
}
