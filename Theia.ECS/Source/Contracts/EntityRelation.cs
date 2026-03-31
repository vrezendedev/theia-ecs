using System;
using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

public readonly ref struct EntityEvaluatedRelations<T>
    where T : struct
{
    public readonly ReadOnlySpan<Entity> Entities;
    public readonly Span<T> Data;

    public EntityEvaluatedRelations(ReadOnlySpan<Entity> entities, Span<T> data)
    {
        Entities = entities;
        Data = data;
    }
}
