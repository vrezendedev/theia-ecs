namespace Theia.ECS.Contracts;

internal readonly ref struct SignatureMeta
{
    internal readonly required int _size { get; init; }
    internal readonly required int _maxId { get; init; }
    internal readonly required int _maskLength { get; init; }
}
