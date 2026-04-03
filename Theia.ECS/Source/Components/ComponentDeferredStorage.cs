using Theia.ECS.Archetypes;
using Theia.ECS.Entities;

namespace Theia.ECS.Components;

internal abstract class ComponentDeferredStorage
{
    internal abstract void SetWithNext(in EntityMeta entityMeta, Archetype to);
    internal abstract void DiscardNext();
}
