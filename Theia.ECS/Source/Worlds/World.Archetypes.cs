using System;
using System.Runtime.CompilerServices;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Contracts;
using Theia.ECS.Extensions;
using Theia.ECS.Queries;

namespace Theia.ECS.Worlds;

public sealed partial class World
{
    private const int DefaultArchetypesCapacity = 16;
    private const int DefaultArchetypesGrowthFactor = 2;

    private int _archetypesCount;
    private Archetype[] _archetypes;

    private Archetype CreateArchetype(in Signature signature)
    {
        int index = _archetypesCount;

        Array.TryResize(ref _archetypes, index, DefaultArchetypesGrowthFactor);

        Archetype archetype = new Archetype(index, signature);
        _archetypes[index] = archetype;

        _archetypesCount++;

        NomadQuery[] queries = _nomadQueries;

        for (int i = 0; i < queries.Length; i++)
        {
            NomadQuery nomadQuery = queries[i];

            if (nomadQuery._signature.IsSatisfiedBy(archetype._signature))
                nomadQuery.Add(archetype);
        }

        return archetype;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Archetype GetArchetype(int archetypeId) => _archetypes[archetypeId];

    internal Archetype FindOrCreateArchetype(ReadOnlySpan<int> componentIds)
    {
        int componentLength = componentIds.Length;
        SignatureMeta signatureMeta = Signature.GetSignatureMeta(componentIds);

        const int MaxStackSize = 256;
        int length = signatureMeta._maskLength;
        Span<ulong> mask = length <= MaxStackSize ? stackalloc ulong[length] : new ulong[length];
        Signature.SetSignatureMask(componentIds, mask);

        Archetype? targetArchetype = FindEqualArchetype(componentLength, mask);

        if (targetArchetype is null)
            targetArchetype = CreateArchetype(new Signature(componentIds, signatureMeta, mask));

        return targetArchetype;
    }

    private Archetype? FindEqualArchetype(int componentsLength, ReadOnlySpan<ulong> mask)
    {
        Archetype[] archetypes = _archetypes;

        for (int i = 0; i < _archetypesCount; i++)
        {
            Signature archetypeSignature = archetypes[i]._signature;
            ReadOnlySpan<ulong> archetypeMask = archetypeSignature.GetMask();

            if (
                Signature.IsEqual(componentsLength, archetypeSignature._length, mask, archetypeMask)
            )
                return archetypes[i];
        }

        return null;
    }
}
