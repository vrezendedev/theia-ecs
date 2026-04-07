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

    internal static string Constraints(int count, string prefix, string constraint, string tabs) =>
        string.Join(
            "\n",
            Enumerable.Range(1, count).Select(i => $"{tabs}where {prefix}{i} : {constraint}")
        );

    internal static string Params(int count, string paramScope, string paramPrefix) =>
        string.Join(", ", Enumerable.Range(1, count).Select(i => $"{paramScope} {paramPrefix}{i}"));

    internal static string VariablesDefinitionComponentsIds(
        int count,
        string variablePrefix,
        string genericPrefix
    ) =>
        string.Join(
            "\n",
            Enumerable
                .Range(1, count)
                .Select(i =>
                    $"        int {variablePrefix}{i}{Constants.ComponentIdVariableSuffix} = ComponentMeta<{genericPrefix}{i}>.s_id;"
                )
        );

    internal static string VariablesAccessComponentsIds(int count, string variablePrefix) =>
        string.Join(
            ", ",
            Enumerable
                .Range(1, count)
                .Select(i => $"{variablePrefix}{i}{Constants.ComponentIdVariableSuffix}")
        );
}
