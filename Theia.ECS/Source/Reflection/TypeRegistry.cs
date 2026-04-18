using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Theia.ECS.Extensions;

namespace Theia.ECS.Reflection;

internal sealed class TypeRegistry<T>
    where T : ITypeMeta
{
    private const int DefaultTypeMetaMapCapacity = 16;
    private const int DefaultTypeMetaGrowthFactor = 2;

    private readonly StringBuilder _stringBuilder = new();
    private int _count;

    private readonly Dictionary<Type, int> _typeId = new();
    private readonly Dictionary<string, int> _typeNameId = new();
    private T[] _typeMetaMap = new T[DefaultTypeMetaMapCapacity];

    private readonly Lock _lock = new();

    internal int Account()
    {
        lock (_lock)
        {
            int index = _count;

            Array.TryResize(ref _typeMetaMap, index, DefaultTypeMetaGrowthFactor);

            _count++;

            return index;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, in T typeMeta)
    {
        lock (_lock)
        {
            Type type = typeMeta.Get();

            string typeName = GetTypeName(type);
            typeMeta.SetTypeName(typeName);

            _typeMetaMap[index] = typeMeta;
            _typeId[type] = index;
            _typeNameId[typeName] = index;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetTypeId(Type type) => _typeId[type];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetTypeId(string typeName) =>
        _typeNameId.GetAlternateLookup<ReadOnlySpan<char>>()[typeName];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T GetTypeMeta(int index) => _typeMetaMap[index];

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

    internal int Count() => Volatile.Read(ref _count);

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
