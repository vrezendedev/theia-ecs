using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Theia.ECS.Components;
using Theia.ECS.Enums.Attributes;

namespace Theia.ECS.Enums;

internal static class ComponentEnum<TEnum>
    where TEnum : unmanaged, Enum
{
    private const BindingFlags Flags =
        BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private enum ComponentEnumMappingType
    {
        None,
        Includes,
        Matches,
    }

    private static readonly TEnum[] s_componentMap;

    static ComponentEnum() => s_componentMap = TryInitialize();

    private static ComponentEnumMappingType ValidateEnum(in TEnum[] enumValues)
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

        return hasIncludes ? ComponentEnumMappingType.Includes
            : hasMatches ? ComponentEnumMappingType.Matches
            : ComponentEnumMappingType.None;
    }

    private static TEnum[] TryInitialize()
    {
        TEnum[] enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));

        ComponentEnumMappingType componentEnumMappingType = ValidateEnum(enumValues);

        return componentEnumMappingType switch
        {
            ComponentEnumMappingType.Includes => InitializeIncludes(in enumValues),
            ComponentEnumMappingType.Matches => InitializeMatches(in enumValues),
            ComponentEnumMappingType.None or _ => Array.Empty<TEnum>(),
        };
    }

    private static TEnum[] InitializeIncludes(in TEnum[] enumValues)
    {
        TEnum[] componentMap = new TEnum[ComponentsMeta.s_count];

        foreach (TEnum value in enumValues)
        {
            FieldInfo field = typeof(TEnum).GetField(value.ToString(), Flags)!;
            Attribute[] attributes = (Attribute[])
                field.GetCustomAttributes(typeof(Includes<>), false);

            foreach (Attribute attribute in attributes)
            {
                int componentId = GetSingleGenericComponentId(attribute);
                componentMap[componentId] = value;
            }
        }

        return componentMap;
    }

    private static TEnum[] InitializeMatches(in TEnum[] enumValues)
    {
        TEnum[] componentMap = new TEnum[ComponentsMeta.s_count];

        foreach (TEnum value in enumValues)
        {
            FieldInfo field = typeof(TEnum).GetField(value.ToString(), Flags)!;
            Attribute? attribute = field.GetCustomAttribute(typeof(Matches<>), false);

            if (attribute is not null)
            {
                int componentId = GetSingleGenericComponentId(attribute);
                componentMap[componentId] = value;
            }
        }

        return componentMap;
    }

    private static int GetSingleGenericComponentId(in Attribute attribute) =>
        ComponentsMeta.GetComponentId(attribute.GetType().GenericTypeArguments[0]);

    internal static TEnum FromComponent(int componentId)
    {
        if ((uint)componentId >= (uint)s_componentMap.Length)
            return default;

        return s_componentMap[componentId];
    }

    [DoesNotReturn]
    private static void ThrowMixedAttributes() =>
        throw new InvalidOperationException(
            @$"
Enum '{typeof(TEnum).Name}' mixes Includes<> and Matches<> attributes, which is not allowed.
An enum must either group components using Includes<> or map components directly using Matches<>, never both.
"
        );
}
