using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Theia.ECS.Reflection;

internal sealed class TypeRegistry<T>
    where T : ITypeMeta
{
    private const int DefaultTypeMetaMapCapacity = 16;
    private const int DefaultTypeMetaGrowthFactor = 2;

    private int _count;

    private readonly Dictionary<Type, int> _typeId = new();
    private T[] _typeMetaMap = new T[DefaultTypeMetaMapCapacity];

    private readonly Lock _lock = new();

    internal int Account()
    {
        lock (_lock)
        {
            int currentLength = _typeMetaMap.Length;

            int index = _count;

            if (index == currentLength)
                Array.Resize(ref _typeMetaMap, currentLength * DefaultTypeMetaGrowthFactor);

            _count++;

            return index;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(int index, in T typeMeta)
    {
        _typeMetaMap[index] = typeMeta;
        _typeId[typeMeta.Get()] = index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetTypeId(Type type) => _typeId[type];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T GetTypeMeta(int index) => _typeMetaMap[index];

    internal int Count() => Volatile.Read(ref _count);
}
