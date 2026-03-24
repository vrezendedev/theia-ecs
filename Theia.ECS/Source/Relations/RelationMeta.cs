using System;
using System.Threading;

namespace Theia.ECS.Relations;

internal enum RelationCardinality
{
    OneToMany,
    ManyToMany,
}

internal enum RelationKind
{
    Tag,
    Data,
}

internal static class RelationsMeta
{
    private const int DefaultRelationTypeMapCapacity = 16;
    private const int DefaultRelationTypeGrowthFactor = 2;

    internal static int s_count { get; private set; }
    private static RelationType[] s_relationTypesMap = new RelationType[
        DefaultRelationTypeMapCapacity
    ];

    internal static readonly Lock s_lock = new();

    private static int RegisterRelation<TRelation>(RelationCardinality cardinality)
        where TRelation : struct
    {
        lock (s_lock)
        {
            int currentLength = s_relationTypesMap.Length;

            if (s_count == currentLength)
                Array.Resize(
                    ref s_relationTypesMap,
                    currentLength * DefaultRelationTypeGrowthFactor
                );

            int index = s_count;

            s_relationTypesMap[index] = new RelationType<TRelation>(typeof(TRelation), cardinality);

            s_count++;

            return index;
        }
    }
}
