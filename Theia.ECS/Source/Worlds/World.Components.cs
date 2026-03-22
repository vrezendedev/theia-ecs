using System;
using System.Diagnostics.CodeAnalysis;
using Theia.ECS.Components;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private Unique[] _uniques;

    private Unique<TComponent> AddUnique<TComponent>(int componentId)
        where TComponent : struct
    {
        if (componentId > _uniques.Length - 1)
            Array.Resize(ref _uniques, componentId + 1);

        Unique<TComponent> unique = new Unique<TComponent>();
        _uniques[componentId] = unique;

        return unique;
    }

    public World RegisterUnique<TComponent>(in TComponent component = default)
        where TComponent : struct
    {
        ThrowIfQueriesExecuting();
        ThrowIfFlushingDeferred();

        int componentId = ComponentMeta<TComponent>.s_id;

        if (componentId <= _uniques.Length - 1 && _uniques[componentId] is not null)
            ThrowUniqueAlreadyRegistered<TComponent>();

        Unique<TComponent> unique = AddUnique<TComponent>(componentId);
        unique.Set(component);

        return this;
    }

    public ref TComponent GetUnique<TComponent>()
        where TComponent : struct
    {
        int componentId = ComponentMeta<TComponent>.s_id;

        if (componentId > _uniques.Length - 1 || _uniques[componentId] is null)
            ThrowUniqueUnregistered<TComponent>();

        return ref ((Unique<TComponent>)_uniques[componentId]).Get();
    }

    public void SetUnique<TComponent>(in TComponent component)
        where TComponent : struct
    {
        int componentId = ComponentMeta<TComponent>.s_id;

        if (componentId > _uniques.Length - 1 || _uniques[componentId] is null)
            ThrowUniqueUnregistered<TComponent>();

        ((Unique<TComponent>)_uniques[componentId]).Set(component);
    }

    [DoesNotReturn]
    private static void ThrowUniqueAlreadyRegistered<TComponent>() =>
        throw new InvalidOperationException(
            $"Unique component '{typeof(TComponent).Name}' is already registered on this World. Unique components can only be registered once per World."
        );

    [DoesNotReturn]
    private static void ThrowUniqueUnregistered<TComponent>() =>
        throw new InvalidOperationException(
            $"Unique component '{typeof(TComponent).Name}' is not registered on this World. Register before attempting to access it."
        );
}
