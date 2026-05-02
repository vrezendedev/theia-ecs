using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.ECS.Contracts;

/// <summary>
/// Result of attempting a structural change that transitions an entity between archetypes.
/// Carries a reference to the entity's <see cref="EntityMeta"/> and both the previous and
/// current archetypes so callers can fire transition events and patch downstream state or,
/// when the attempt was rejected, <see cref="Invalid"/> with <see cref="_valid"/> set to
/// <see langword="false"/>.
/// </summary>
internal readonly ref struct EntityReferences
{
    internal readonly bool _valid;
    internal readonly ref EntityMeta _entityMeta;
    internal readonly Archetype? _previousArchetype;
    internal readonly Archetype? _currentArchetype;

    public EntityReferences() => _valid = false;

    internal EntityReferences(
        ref EntityMeta entityMeta,
        Archetype previousArchetype,
        Archetype currentArchetype
    )
    {
        _entityMeta = ref entityMeta;
        _previousArchetype = previousArchetype;
        _currentArchetype = currentArchetype;
        _valid = true;
    }

    /// <summary>The "rejected" sentinel; <see cref="_valid"/> is <see langword="false"/> and the archetype references are <see langword="null"/>.</summary>
    internal static EntityReferences Invalid => new() { };
}
