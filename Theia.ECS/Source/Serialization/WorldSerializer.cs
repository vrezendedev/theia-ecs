using System;
using System.Text;
using Theia.ECS.Archetypes;
using Theia.ECS.Components;
using Theia.ECS.Relations;

namespace Theia.ECS.Serialization;

internal sealed class WorldSerializer
{
    private StringBuilder _stringBuilder;
    private readonly WorldDataTransferObject _dto;

    internal WorldSerializer()
    {
        _stringBuilder = new();
        _dto = new() { Version = 1 };
    }

    private string GetTypeName(Type type)
    {
        _stringBuilder.Clear();

        _stringBuilder.Append(type.FullName);
        _stringBuilder.Append(", ");
        _stringBuilder.Append(type.Assembly.GetName().Name);

        return _stringBuilder.ToString();
    }

    internal WorldSerializer AccountComponentsTypes()
    {
        string[] componentsAccounted = new string[ComponentsMeta.Count()];

        for (int i = 0; i < componentsAccounted.Length; i++)
            componentsAccounted[i] = GetTypeName(ComponentsMeta.GetComponentType(i)._type);

        _dto.ComponentsAccounted = componentsAccounted;

        return this;
    }

    internal WorldSerializer AccountRelationsTypes()
    {
        string[] relationsAccounted = new string[RelationsMeta.Count()];

        for (int i = 0; i < relationsAccounted.Length; i++)
            relationsAccounted[i] = GetTypeName(RelationsMeta.GetRelationType(i)._type);

        _dto.RelationsAccounted = relationsAccounted;

        return this;
    }

    internal WorldSerializer AccountComponentSets(ReadOnlySpan<Archetype> archetypes)
    {
        string[][] componentsSets = new string[archetypes.Length][];

        //@TO-DO

        return this;
    }

    internal WorldDataTransferObject Build() => _dto;
}
