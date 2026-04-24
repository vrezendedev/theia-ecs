using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Theia.ECS.SourceGen.Assemblagles;

[Generator]
public class AssemblageGeneratorGenerics : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        StringBuilder sb = new();

        sb.Append(
            @"
using System;
using System.Collections.Generic;
using Theia.ECS.Archetypes;
using Theia.ECS.Contracts;
using Theia.ECS.Entities;
using Theia.ECS.Worlds;

namespace Theia.ECS.Assemblages;

"
        );

        for (
            int i = Constants.ComponentSetOverloadInitialIndex;
            i <= Constants.OverloadMaxRange;
            i++
        )
        {
            string generics = Generator.Generics(i, Constants.GenericComponentPrefix);
            string constraints = Generator.Constraints(
                i,
                Constants.GenericComponentPrefix,
                "struct",
                "    "
            );
            string parameters = Generator.Parameters(
                i,
                "in",
                Constants.GenericComponentPrefix,
                Constants.GenericComponentLocalPrefix
            );
            string arguments = Generator.Arguments(i, "in", Constants.GenericComponentLocalPrefix);
            string mapping = Mapping(i, "in", Constants.GenericComponentLocalPrefix);
            string entityCreateDeferredInit = EntityCreateDeferredInit(
                i,
                Constants.GenericComponentLocalPrefix,
                Constants.GenericComponentLocalPrefix
            );
            string deferredInArgs = Generator.Arguments(
                i,
                string.Empty,
                $"deferredCreate._{Constants.GenericComponentLocalPrefix}"
            );

            sb.AppendLine(
                AssemblageTemplate(
                    generics,
                    constraints,
                    parameters,
                    arguments,
                    mapping,
                    entityCreateDeferredInit,
                    deferredInArgs
                )
            );
        }

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource($"Assemblage.Generics.g.cs", sb.ToString())
        );
    }

    private static string Mapping(int count, string paramScope, string paramPrefix) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"        _archetype.Set(mapping[{i - 1}], in entityCreated._entityMeta, {paramScope} {paramPrefix}{i});"
                )
        );

    private static string EntityCreateDeferredInit(
        int count,
        string fieldPrefix,
        string paramPrefix
    ) =>
        string.Join(
            ", ",
            Enumerable.Range(1, count).Select(i => $"_{fieldPrefix}{i} = {paramPrefix}{i}")
        );

    private static string AssemblageTemplate(
        string generics,
        string constraints,
        string inParams,
        string inArgs,
        string mapping,
        string entityCreateDeferredInit,
        string deferredInArgs
    ) =>
        $$"""
public class Assemblage{{generics}} : Assemblage
{{constraints}}
{
    private Queue<EntityCreateDeferred{{generics}}> _deferredCreate;

     internal Assemblage(
        in World world,
        in Archetype archetype,
        ReadOnlySpan<int> componentStorageMapping
    )
        : base(world, archetype, componentStorageMapping) =>
        _deferredCreate = new(World.DefaultDeferredCommandsCapacity);

    public Entity Create({{inParams}})
    {
        _world.ThrowIfQueriesExecuting();
    
        return CreateAndSet({{inArgs}})._entity;
    }

    internal EntityCreated CreateAndSet({{inParams}})
    {
        EntityCreated entityCreated = _world.CreateEntity(_archetype);

        ReadOnlySpan<int> mapping = GetComponentStorageMapping();

{{mapping}}

        InvokeOnEntityCreated(new EntityAssembled(_world, entityCreated._entity, in _archetype));

        return entityCreated;
    }
    
    public void DeferredCreate({{inParams}})
    {
        _world.ThrowIfFlushingDeferred();

        lock (_deferredCreateLock)
        {
            _deferredCreate.Enqueue(
                new EntityCreateDeferred{{generics}}() { {{entityCreateDeferredInit}} }
            );
        }
    }

    public void DeferredCreate<TRelation>(
        {{inParams}},
        DeferredRelationOnCreate<TRelation> deferredRelationOnCreate
    )
        where TRelation : struct
    {
        _world.ThrowIfFlushingDeferred();

        lock (_deferredCreateLock)
        {
            _deferredCreate.Enqueue(
                new EntityCreateDeferred{{generics}}()
                {
                    {{entityCreateDeferredInit}},
                    _relationDeferred = _world.GetAddRelationDeferred(
                            deferredRelationOnCreate.Owner,
                            deferredRelationOnCreate.Relation
                    ),
                }
            );
        }
    }

    internal override void DeferredCreate()
    {
        while (_deferredCreate.Count > 0)
        {
            EntityCreateDeferred{{generics}} deferredCreate = _deferredCreate.Dequeue();

            EntityCreated entityCreated = CreateAndSet({{deferredInArgs}});

            if (
                deferredCreate._relationDeferred._relationId
                == AddRelationDeferred.InvalidRelationId
            )
                continue;

            _world.DeferredAddRelationHandler(
                deferredCreate._relationDeferred with
                {
                    _target = entityCreated._entity,
                }
            );
        }
    }
}

""";
}
