using System;
using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

public readonly struct EntityRelation<T>
    where T : struct
{
    internal readonly Entity _target;
    internal readonly T _data;

    public EntityRelation(Entity entity, T data = default)
    {
        _target = entity;
        _data = data;
    }
}

public ref struct EntityEvaluatedRelation<T>
    where T : struct
{
    public readonly Entity Target;
    public ref T Value;

    public EntityEvaluatedRelation(Entity target, ref T data)
    {
        Target = target;
        Value = ref data;
    }
}

public readonly ref struct EntityEvaluatedRelations<T>
    where T : struct
{
    public readonly ReadOnlySpan<Entity> Entities;
    public readonly Span<T> Values;

    public EntityEvaluatedRelations(ReadOnlySpan<Entity> entities, Span<T> data)
    {
        Entities = entities;
        Values = data;
    }
}

public readonly ref struct EntityEvaluatedRelationsReadOnly<T>
    where T : struct
{
    public readonly ReadOnlySpan<Entity> Entities;
    public readonly ReadOnlySpan<T> Values;

    public EntityEvaluatedRelationsReadOnly(ReadOnlySpan<Entity> entities, ReadOnlySpan<T> data)
    {
        Entities = entities;
        Values = data;
    }
}
