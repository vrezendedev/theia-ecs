using System;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Relations;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private RelationsIndexer[] _relationsIndexer;
    private RelationStorage[] _relationStorage;

    public bool TryAddRelation<TRelation>(Entity relationOwner, Entity target)
    {
        throw new NotImplementedException();
    }

    public bool TryAddEvaluatedRelation<TRelation>(
        Entity relationOwner,
        Entity target,
        TRelation value
    )
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public bool TryAddRelations<TRelation>(
        Entity relationOwner,
        params ReadOnlySpan<Entity> targets
    )
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public bool TryAddEvaluatedRelations<TRelation>(
        Entity relationOwner,
        params ReadOnlySpan<EntityRelation<TRelation>> targets
    )
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public bool TryRemoveRelation<TRelation>(Entity entityOwner)
    {
        throw new NotImplementedException();
    }

    public bool TryRemoveRelation<TRelation>(Entity entityOwner, Entity entity)
    {
        throw new NotImplementedException();
    }

    public Entity GetRelation<TRelation>(Entity relationOwner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public EntityEvaluatedRelation<TRelation> GetEvaluatedRelation<TRelation>(Entity relationOwner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public TRelation ReadEvaluatedRelation<TRelation>(Entity relationOwner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<Entity> GetRelations<TRelation>(Entity relationOwner)
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public EntityEvaluatedRelations<TRelation> GetEvaluatedRelations<TRelation>(
        Entity relationOwner
    )
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public EntityEvaluatedRelationsReadOnly<TRelation> ReadEvaluatedRelations<TRelation>(
        Entity relationOwner
    )
        where TRelation : struct
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<Entity> GetLinkedRelations<T>(Entity entity)
        where T : struct
    {
        throw new NotImplementedException();
    }

    public bool AreRelated<T>(Entity relationOwner, Entity target)
        where T : struct
    {
        throw new NotImplementedException();
    }

    public bool HasRelation<T>(Entity owner)
    {
        throw new NotImplementedException();
    }

    public void UpdateRelation(Entity relationOwner, UpdateRelation update)
    {
        throw new NotImplementedException();
    }

    public void UpdateRelation<TRelation>(Entity relationOwner, UpdateRelation<TRelation> update)
    {
        throw new NotImplementedException();
    }
}
