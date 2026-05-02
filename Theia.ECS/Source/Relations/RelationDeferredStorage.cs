namespace Theia.ECS.Relations;

/// <summary>
/// Non-generic base for the typed free-list storage that stages relation payloads enqueued
/// during query execution and applies them at flush time. The deferred-command system uses this
/// as the value-side counterpart to the command queue's record side.
/// </summary>
internal abstract class RelationDeferredStorage
{
    /// <summary>
    /// Applies the previously-staged relation value at <paramref name="storageIndex"/> to
    /// <paramref name="relation"/>'s slot at <paramref name="compositeKey"/>, then frees the
    /// staging slot for reuse.
    /// </summary>
    internal abstract void SetWith(int storageIndex, Relation relation, int compositeKey);
}
