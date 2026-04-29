using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Theia.ECS.Enums.Attributes;
using Theia.ECS.Reflection;

namespace Theia.ECS.Enums;

/// <summary>
/// Per-(<typeparamref name="TEnum"/>, <typeparamref name="TTypeMeta"/>) cache that maps the
/// integer IDs assigned by a <see cref="ITypeMeta"/> registry back to a user-defined
/// <see cref="Enum"/>.
/// </summary>
/// <remarks>
/// <para>
/// The mapping is declared on the enum itself via two mutually exclusive attribute styles:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       <c>Includes&lt;T&gt;</c>: an enum value may carry several of these, grouping multiple
///       struct types under one bucket.
///     </description>
///   </item>
///   <item>
///     <description>
///       <c>Matches&lt;T&gt;</c>: exactly one struct type per enum value, no overlap. Used
///       when the relationship is strictly one-to-one.
///     </description>
///   </item>
/// </list>
/// <para>
/// Mixing the two on the same enum is disallowed and throws at first access. An enum that uses
/// neither attribute is legal and produces an empty map; <see cref="FromStruct"/>, if invalid, will return
/// <see langword="default"/> for any input.
/// </para>
/// <para>
/// Because each closed generic instantiation has its own static state, the mapping is built
/// exactly once per enum/registry pair, on first access, and lookups thereafter are a simple
/// array index.
/// </para>
/// </remarks>
internal static class StructEnum<TEnum, TTypeMeta>
    where TEnum : unmanaged, Enum
    where TTypeMeta : ITypeMeta
{
    private const BindingFlags Flags =
        BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    /// <summary>Internal classification of how an enum declares its mapping to struct types.</summary>
    private enum StructEnumMappingType
    {
        /// <summary>The enum carries no mapping attributes; <see cref="FromStruct"/> always returns <see langword="default"/>.</summary>
        None,

        /// <summary>One or more enum values use <c>Includes&lt;T&gt;</c>; many struct types may share an enum value.</summary>
        Includes,

        /// <summary>One or more enum values use <c>Matches&lt;T&gt;</c>; each struct type maps to a single enum value.</summary>
        Matches,
    }

    private static readonly TEnum[] s_structMap;

    static StructEnum() => s_structMap = Initialize();

    /// <summary>
    /// Inspects the enum's fields to determine which mapping style is in use, throwing if
    /// both <c>Includes&lt;T&gt;</c> and <c>Matches&lt;T&gt;</c> appear on the same enum.
    /// </summary>
    private static StructEnumMappingType ValidateEnum(in TEnum[] enumValues)
    {
        bool hasIncludes = false;
        bool hasMatches = false;

        foreach (TEnum value in enumValues)
        {
            FieldInfo field = typeof(TEnum).GetField(value.ToString(), Flags)!;

            if (field.GetCustomAttributes(typeof(Includes<>), false).Length > 0)
                hasIncludes = true;
            if (field.GetCustomAttributes(typeof(Matches<>), false).Length > 0)
                hasMatches = true;
        }

        if (hasIncludes && hasMatches)
            ThrowMixedAttributes();

        return hasIncludes ? StructEnumMappingType.Includes
            : hasMatches ? StructEnumMappingType.Matches
            : StructEnumMappingType.None;
    }

    private static TEnum[] Initialize()
    {
        TEnum[] enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));

        StructEnumMappingType structEnumMappingType = ValidateEnum(enumValues);

        return structEnumMappingType switch
        {
            StructEnumMappingType.Includes => InitializeIncludes(in enumValues),
            StructEnumMappingType.Matches => InitializeMatches(in enumValues),
            StructEnumMappingType.None or _ => Array.Empty<TEnum>(),
        };
    }

    private static int GetStructMapSize() => TTypeMeta.Count();

    private static int GetStructId(in Type type) => TTypeMeta.GetId(type);

    private static int GetSingleGenericStructId(in Attribute attribute) =>
        GetStructId(attribute.GetType().GenericTypeArguments[0]);

    /// <summary>
    /// Builds the map for an <c>Includes&lt;T&gt;</c>-annotated enum. Each attribute on each
    /// enum value contributes one struct-ID slot pointing back at that value, so several
    /// struct types may land on the same value.
    /// </summary>
    private static TEnum[] InitializeIncludes(in TEnum[] enumValues)
    {
        TEnum[] structMap = new TEnum[GetStructMapSize()];

        foreach (TEnum value in enumValues)
        {
            FieldInfo field = typeof(TEnum).GetField(value.ToString(), Flags)!;
            Attribute[] attributes = (Attribute[])
                field.GetCustomAttributes(typeof(Includes<>), false);

            foreach (Attribute attribute in attributes)
            {
                int structId = GetSingleGenericStructId(attribute);
                structMap[structId] = value;
            }
        }

        return structMap;
    }

    /// <summary>
    /// Builds the map for a <c>Matches&lt;T&gt;</c>-annotated enum. Each enum value contributes
    /// at most one struct-ID slot, since the attribute is single-instance.
    /// </summary>
    private static TEnum[] InitializeMatches(in TEnum[] enumValues)
    {
        TEnum[] structMap = new TEnum[GetStructMapSize()];

        foreach (TEnum value in enumValues)
        {
            FieldInfo field = typeof(TEnum).GetField(value.ToString(), Flags)!;
            Attribute? attribute = field.GetCustomAttribute(typeof(Matches<>), false);

            if (attribute is not null)
            {
                int structId = GetSingleGenericStructId(attribute);
                structMap[structId] = value;
            }
        }

        return structMap;
    }

    /// <summary>
    /// Returns the enum value associated with <paramref name="structId"/>. Returns
    /// <see langword="default"/> when the struct ID has no mapping or when the enum declares
    /// no mapping attributes at all.
    /// </summary>
    internal static TEnum FromStruct(int structId)
    {
        if (structId >= s_structMap.Length)
            return default;

        return s_structMap[structId];
    }

    [DoesNotReturn]
    private static void ThrowMixedAttributes() =>
        throw new InvalidOperationException(
            @$"
Enum '{typeof(TEnum).Name}' mixes Includes<> and Matches<> attributes, which is not allowed.
An enum must either group using Includes<> or map directly using Matches<>, never both.
"
        );
}
