using System;

namespace Theia.ECS.Relations.Attributes;

/// <summary>
/// Marks a struct as a relation type, making it eligible for registration with
/// <see cref="RelationsMeta"/>. Required on every relation; structs without this attribute
/// will fail validation in <see cref="RelationMeta{TRelation}"/>'s static constructor.
/// </summary>
/// <remarks>
/// The attribute carries no data; its presence alone is the signal. It serves as the
/// disambiguator between a fieldless component (which is rejected by
/// <see cref="Components.ComponentMeta{TComponent}"/>) and a tag relation (which is valid
/// precisely because it is fieldless). Without this attribute, the framework has no way to
/// tell which a fieldless struct is meant to be, so the attribute is what makes the empty
/// struct legal in the relation case.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
public sealed class Relationship : Attribute { }
