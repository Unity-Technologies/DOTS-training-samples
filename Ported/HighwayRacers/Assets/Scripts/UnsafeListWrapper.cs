using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// NativeList without any safety system and advanced feature
// This allows us to nest native containers easily
// Only meant to be used form main thread... or very carefully otherwise
public unsafe struct UnsafeListWrapper<T> : IDisposable
    where T : struct
{
    [NativeDisableUnsafePtrRestriction]
    internal UnsafeList* m_ListData;

    public UnsafeListWrapper(Allocator allocator)
        : this(1, allocator)
    {
    }

    public UnsafeListWrapper(int initialCapacity, Allocator allocator)
    {
        m_ListData = UnsafeList.Create(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), initialCapacity, allocator);
    }

    public void Dispose()
    {
        UnsafeList.Destroy(m_ListData);
        m_ListData = null;
    }

    public T this[int index]
    {
        get
        {
            return UnsafeUtility.ReadArrayElement<T>(m_ListData->Ptr, index);
        }
        set
        {
            UnsafeUtility.WriteArrayElement(m_ListData->Ptr, index, value);
        }
    }

    public int Length
    {
        get
        {
            return m_ListData->Length;
        }
    }

    public int Capacity
    {
        get
        {
            return m_ListData->Capacity;
        }

        set
        {
            m_ListData->SetCapacity<T>(value);
        }
    }

    public NativeArray<T> AsArray()
    {
        var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(m_ListData->Ptr, m_ListData->Length, Allocator.None);
        return array;
    }

    public void Add(T value)
    {
        m_ListData->Add(value);
    }

    public void RemoveAtSwapBack(int index)
    {
        m_ListData->RemoveAtSwapBack<T>(index);
    }

    public void Clear()
    {
        m_ListData->Clear();
    }

    public void Resize(int length, NativeArrayOptions options)
    {
        m_ListData->Resize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), length, options);
    }

    public bool IsCreated => m_ListData != null;
}
