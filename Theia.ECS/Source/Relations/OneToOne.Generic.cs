using System.Runtime.CompilerServices;
using Theia.ECS.Entities;

namespace Theia.ECS.Relations;

internal class OneToOne<TRelation> : Relation
    where TRelation : struct
{
    private Entity _a;
    private Entity _b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Relate(Entity a, Entity b)
    {
        _a = a;
        _b = b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Entity To(Entity e) => e == _a ? _b : _a;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Reset()
    {
        _a = default;
        _b = default;
    }
}
