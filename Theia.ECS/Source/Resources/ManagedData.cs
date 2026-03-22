using System;
using System.Runtime.CompilerServices;

namespace Theia.ECS.Resources;

public static class ManagedData<TKey, TData>
    where TKey : unmanaged, Enum
    where TData : class
{
    private static TData[][] s_data;

    static ManagedData()
    {
        if (Enum.GetUnderlyingType(typeof(TKey)) != typeof(int))
            throw new InvalidOperationException(
                $"'{typeof(TKey).Name}' must have an underlying type of int. Declare the enum as 'enum {typeof(TKey).Name} : int'."
            );

        s_data = Array.Empty<TData[]>();
    }

    public static void Register(TKey key, TData[] data)
    {
        int index = Unsafe.As<TKey, int>(ref key);

        if (index <= s_data.Length - 1 && s_data[index] is not null)
            throw new InvalidOperationException(
                $"Key '{typeof(TKey).Name}.{key}' is already registered. Use Set() to replace existing data."
            );

        if (index > s_data.Length - 1)
            Array.Resize(ref s_data, index + 1);

        s_data[index] = data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TData[] Get(TKey key) => s_data[GetKeyIndexed(key)];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TData Get(TKey key, int index) => s_data[GetKeyIndexed(key)][index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(TKey key, TData[] data) => s_data[GetKeyIndexed(key)] = data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(TKey key, int index, TData data) =>
        s_data[GetKeyIndexed(key)][index] = data;

    private static int GetKeyIndexed(TKey key)
    {
        int index = Unsafe.As<TKey, int>(ref key);

        if (index < 0 || index > s_data.Length - 1 || s_data[index] is null)
            throw new InvalidOperationException(
                $"Key '{typeof(TKey).Name}.{key}' is not registered. Register before attempting to access it."
            );

        return index;
    }
}
