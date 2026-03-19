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
    private const int DefaultArchetypesGrowthFactor = 2;

    private int _archetypesCount;
    private Archetype[] _archetypes;

    private Archetype CreateArchetype(in Signature signature)
    {
        int currentLength = _archetypes.Length;

        if (_archetypesCount == currentLength)
            Array.Resize(ref _archetypes, currentLength * DefaultArchetypesGrowthFactor);

        int index = _archetypesCount;

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
        Span<ulong> mask = stackalloc ulong[signatureMeta._maskLength];
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
