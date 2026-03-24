using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Theia.ECS.Reflection;

internal sealed class TypeRegistry<T>
    where T : TypeMeta
{
    private const int DefaultTypeMetaMapCapacity = 16;
    private const int DefaultTypeMetaGrowthFactor = 2;

    private int _count;

    private readonly Dictionary<Type, int> _typeId = new();
    private T[] _typeMetaMap = new T[DefaultTypeMetaMapCapacity];

    private readonly Lock s_lock = new();

    internal int Account()
    {
        lock (s_lock)
        {
            int currentLength = _typeMetaMap.Length;

            if (_count == currentLength)
                Array.Resize(ref _typeMetaMap, currentLength * DefaultTypeMetaGrowthFactor);

            int index = _count;

            _count++;

            return index;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, in T typeMeta)
    {
        _typeMetaMap[index] = typeMeta;
        _typeId[typeMeta._type] = index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetTypeId(Type type) => _typeId[type];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T GetTypeMeta(int index) => _typeMetaMap[index];

    internal int Count() => Volatile.Read(ref _count);
}
