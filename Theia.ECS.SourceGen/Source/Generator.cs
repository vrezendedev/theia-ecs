using System.Linq;

namespace Theia.ECS.SourceGen;

internal static class Generator
{
    internal static string Generics(int count, string prefix) =>
        $"<{string.Join(", ", Enumerable.Range(1, count).Select(i => $"{prefix}{i}"))}>";

    internal static string Arguments(
        int count,
        string argScope,
        string genericPrefix,
        string paramPrefix
    ) =>
        string.Join(
            ", ",
            Enumerable
                .Range(1, count)
                .Select(i => $"{argScope} {genericPrefix}{i} {paramPrefix}{i}")
        );

    internal static string Constraints(int count, string prefix, string constraint) =>
        string.Join(
            "\n",
            Enumerable.Range(1, count).Select(i => $"    where {prefix}{i} : {constraint}")
        );

    internal static string Params(int count, string paramScope, string paramPrefix) =>
        string.Join(", ", Enumerable.Range(1, count).Select(i => $"{paramScope} {paramPrefix}{i}"));
}
