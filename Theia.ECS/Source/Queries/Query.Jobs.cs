using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Jobs;

namespace Theia.ECS.Queries;

internal sealed class ForEachEntityJob<TForEach, ComponentT1> : Job
    where TForEach : struct, IForEachEntity<ComponentT1>
    where ComponentT1 : struct
{
    internal TForEach ForEach;
    internal Indexer? Indexer;
    internal Storage<ComponentT1>? StoragesComponentT1;
    internal int Count;

    public override void Execute()
    {
        int count = Count;
        TForEach forEach = ForEach;

        Span<Entity> entities = Indexer!.GetValues();
        Span<ComponentT1> storage = StoragesComponentT1!.GetValues(count);

        for (int i = 0; i < count; i++)
            forEach.Execute(entities[i], ref storage[i]);
    }
}

internal sealed class ForEachJob<TForEach, ComponentT1> : Job
    where TForEach : struct, IForEach<ComponentT1>
    where ComponentT1 : struct
{
    internal TForEach ForEach;
    internal Storage<ComponentT1>? StoragesComponentT1;
    internal int Count;

    public override void Execute()
    {
        int count = Count;
        TForEach forEach = ForEach;

        Span<ComponentT1> storage = StoragesComponentT1!.GetValues(count);

        for (int i = 0; i < count; i++)
            forEach.Execute(ref storage[i]);
    }
}
