using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Theia.ECS.Reflection;

/// <summary>
/// Generic registry that assigns stable integer IDs to types implementing <see cref="ITypeMeta"/>
/// and exposes lookup by <see cref="Type"/>, by assembly-qualified name, and by ID.
/// </summary>
/// <remarks>
/// <para>
/// <b>Thread safety:</b> the registry supports concurrent writers and concurrent readers without
/// external synchronization.
/// </para>
/// </remarks>
internal sealed class TypeRegistry<T>
    where T : ITypeMeta
{
    private const int DefaultTypeMetaMapCapacity = 16;
    private const int DefaultTypeMetaGrowthFactor = 2;

    private readonly StringBuilder _stringBuilder = new();
    private int _count;

    private readonly ConcurrentDictionary<Type, int> _typeId = new();
    private readonly ConcurrentDictionary<string, int> _typeNameId = new();
    private T[] _typeMetaMap = new T[DefaultTypeMetaMapCapacity];

    private readonly Lock _writeLock = new();

    /// <summary>
    /// Atomically reserves a slot, populates it with <paramref name="typeMeta"/>, and
    /// publishes the lookups. The slot is not observable to any reader until this method
    /// returns.
    /// </summary>
    internal int Register(in T typeMeta)
    {
        lock (_writeLock)
        {
            Type type = typeMeta.Get();

            if (_typeId.TryGetValue(type, out int existing))
                return existing;

            string typeName = GetTypeName(type);
            typeMeta.SetTypeName(typeName);

            int index = _count;

            T[] current = _typeMetaMap;
            if (index >= current.Length)
            {
                T[] resized = new T[current.Length * DefaultTypeMetaGrowthFactor];
                Array.Copy(current, resized, current.Length);
                Volatile.Write(ref _typeMetaMap, resized);
            }

            Volatile.Read(ref _typeMetaMap)[index] = typeMeta;

            _typeId[type] = index;
            _typeNameId[typeName] = index;

            Volatile.Write(ref _count, index + 1);

            return index;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetTypeId(Type type) => _typeId[type];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetTypeId(string typeName) =>
        _typeNameId.GetAlternateLookup<ReadOnlySpan<char>>()[typeName];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T GetTypeMeta(int index) => Volatile.Read(ref _typeMetaMap)[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetTypeId(Type type, out int index)
    {
        if (_typeId.TryGetValue(type, out index))
            return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetTypeId(string typeName, out int index)
    {
        if (_typeNameId.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(typeName, out index))
            return true;

        return false;
    }

    /// <summary>Returns the number of types currently registered. Safe to call concurrently with writers.</summary>
    internal int Count() => Volatile.Read(ref _count);

    /// <summary>
    /// Builds the canonical assembly-qualified name used as the registry's string key for
    /// <paramref name="type"/>. Format matches <see cref="Type.AssemblyQualifiedName"/> for closed
    /// generics, with each generic argument recursively expanded.
    /// </summary>
    private string GetTypeName(Type type)
    {
        _stringBuilder.Clear();

        AppendTypeName(type);

        return _stringBuilder.ToString();
    }

    private void AppendTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            _stringBuilder.Append(type.GetGenericTypeDefinition().FullName);

            _stringBuilder.Append('[');

            Type[] args = type.GetGenericArguments();

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    _stringBuilder.Append(',');

                _stringBuilder.Append('[');

                AppendTypeName(args[i]);

                _stringBuilder.Append(']');
            }

            _stringBuilder.Append(']');
        }
        else
        {
            _stringBuilder.Append(type.FullName);
        }

        _stringBuilder.Append(", ");

        _stringBuilder.Append(type.Assembly.GetName().Name);
    }

    /// <summary>
    /// Throws a <see cref="TypeLoadException"/> with a diagnostic message that splits
    /// <paramref name="typeName"/> into its type and assembly parts. Used by callers that
    /// fail to resolve a serialized type during deserialization.
    /// </summary>
    [DoesNotReturn]
    internal static void ThrowTypeLoadException(string typeName)
    {
        int separatorIndex = typeName.IndexOf(',');
        string typePart = typeName[..separatorIndex].Trim();
        string assemblyPart = typeName[(separatorIndex + 1)..].Trim();

        throw new TypeLoadException(
            $"Type '{typePart}' could not be resolved from assembly '{assemblyPart}'."
                + $"The type may have been renamed or moved, or '{assemblyPart}' may not be loaded."
                + $"Also, ensure all assemblies containing serialized types are loaded before deserializing."
        );
    }
}
