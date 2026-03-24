using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Theia.ECS.Components;
using Theia.ECS.Enums.Attributes;
using Theia.ECS.Reflection;
using Theia.ECS.Relations;

namespace Theia.ECS.Enums;

internal static class StructEnum<TEnum, TOverload>
    where TEnum : unmanaged, Enum
    where TOverload : TypeMeta
{
    private const BindingFlags Flags =
        BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private enum StructEnumMappingType
    {
        None,
        Includes,
        Matches,
    }

    private static readonly TEnum[] s_structMap;

    static StructEnum() => s_structMap = Initialize();

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

    private static int GetStructMapSize()
    {
        if (typeof(TOverload) == typeof(ComponentType))
            return ComponentsMeta.Count();
        else if (typeof(TOverload) == typeof(RelationType))
            return RelationsMeta.Count();
        else
            return 0;
    }

    private static int GetStructId(in Type type)
    {
        if (typeof(TOverload) == typeof(ComponentType))
            return ComponentsMeta.GetComponentId(type);
        else if (typeof(TOverload) == typeof(RelationType))
            return RelationsMeta.GetRelationId(type);
        else
            return 0;
    }

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

    private static int GetSingleGenericStructId(in Attribute attribute) =>
        GetStructId(attribute.GetType().GenericTypeArguments[0]);

    internal static TEnum FromStruct(int structId)
    {
        if ((uint)structId >= (uint)s_structMap.Length)
            return default;

        return s_structMap[structId];
    }

    [DoesNotReturn]
    private static void ThrowMixedAttributes() =>
        throw new InvalidOperationException(
            @$"
Enum '{typeof(TEnum).Name}' mixes Includes<> and Matches<> attributes, which is not allowed.
An enum must either group components/relations using Includes<> or map components/relations directly using Matches<>, never both.
"
        );
}
