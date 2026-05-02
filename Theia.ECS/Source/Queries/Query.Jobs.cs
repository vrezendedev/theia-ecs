using System;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Entities;
using Theia.ECS.Jobs;

namespace Theia.ECS.Queries;

/// <summary>
/// Pooled <see cref="Job"/> wrapper that runs an <see cref="IForEachEntity{ComponentT1}"/>
/// callback over a single chunk's entities and component storage.
/// <br/>
/// <b>One job is enqueued per non-empty chunk</b> by parallel query iteration.
/// </summary>
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
        Span<ComponentT1> storageT1 = StoragesComponentT1!.GetValues(count);

        for (int i = 0; i < count; i++)
            forEach.Execute(entities[i], ref storageT1[i]);
    }
}

/// <summary>
/// Pooled <see cref="Job"/> wrapper that runs an <see cref="IForEach{ComponentT1}"/> callback
/// over a single chunk's component storage, without loading entity handles.
/// <br/>
/// <b>One job is enqueued per non-empty chunk</b> by parallel query iteration.
/// </summary>
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

        Span<ComponentT1> storageT1 = StoragesComponentT1!.GetValues(count);

        for (int i = 0; i < count; i++)
            forEach.Execute(ref storageT1[i]);
    }
}
