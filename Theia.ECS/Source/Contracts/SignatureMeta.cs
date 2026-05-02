namespace Theia.ECS.Contracts;

/// <summary>
/// Precomputed shape information for a <see cref="Components.Signature"/>: the sum of component
/// byte sizes, the largest component ID in the set, and the number of <see cref="ulong"/> buckets
/// needed to hold the signature's bitmask.
/// </summary>
internal readonly ref struct SignatureMeta
{
    internal readonly required int _size { get; init; }
    internal readonly required int _maxId { get; init; }
    internal readonly required int _maskLength { get; init; }
}
