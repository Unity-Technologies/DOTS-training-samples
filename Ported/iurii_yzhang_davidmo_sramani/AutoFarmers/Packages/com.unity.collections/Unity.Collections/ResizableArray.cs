using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
    // this is a fixed 64 byte buffer, whose interface is a resizable array of T.
    // for times when you need a struct member that is a small but resizable array of T,
    // but you don't want to visit the heap or do everything by hand with naked primitives.
    [PublicAPI]
    public unsafe struct ResizableArray64Byte<T> where T : struct
    {
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckElementAccess(int index)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException($"Index {index} is out of range of '{Length}' Length.");
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckResize(int newCount)
        {
            if (newCount < 0 || newCount > Capacity)
                throw new IndexOutOfRangeException($"NewCount {newCount} is out of range of '{Capacity}' Capacity.");
        }

        [WriteAccessRequired]
        public void* GetUnsafePointer()
        {
            fixed (void* b = m_Buffer)
                return b;
        }

        const int k_TotalSizeInBytes = 64;
        const int k_BufferSizeInBytes = k_TotalSizeInBytes - sizeof(int);
        const int k_BufferSizeInInts = k_BufferSizeInBytes / sizeof(int);

        int m_Length;
#pragma warning disable 649
        fixed int m_Buffer[k_BufferSizeInInts];
#pragma warning restore 649

        public int Length
        {
            get => m_Length;
            [WriteAccessRequired]
            set
            {
                CheckResize(value);
                m_Length = value;
            }
        }

        int LengthBytes =>
            m_Length * UnsafeUtility.SizeOf<T>();

        public int Capacity =>
            k_BufferSizeInBytes / UnsafeUtility.SizeOf<T>();

        public T this[int index]
        {
            get
            {
                CheckElementAccess(index);
                fixed (void* b = m_Buffer)
                    return UnsafeUtility.ReadArrayElement<T>(b, index);
            }
            [WriteAccessRequired]
            set
            {
                CheckElementAccess(index);
                fixed (void* b = m_Buffer)
                    UnsafeUtility.WriteArrayElement(b, index, value);
            }
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            fixed (void* b = m_Buffer)
                return (int)CollectionHelper.Hash(b, LengthBytes);
        }

        public override bool Equals(object other) =>
            throw new InvalidOperationException("Calling this function is a sign of inadvertent boxing");

        public bool Equals(ref ResizableArray64Byte<T> other)
        {
            if (m_Length != other.m_Length)
                return false;

            fixed (void* ba = m_Buffer, bb = other.m_Buffer)
                return UnsafeUtility.MemCmp(ba, bb, LengthBytes) == 0;
        }

        [WriteAccessRequired]
        public void Add(T value) =>
            this[Length++] = value;

        public ResizableArray64Byte(T value)
        {
            m_Length = 1;
            CheckResize(1);
            fixed (void* b = m_Buffer)
                UnsafeUtility.WriteArrayElement(b, 0, value);
        }

        public ResizableArray64Byte(T value0, T value1)
        {
            m_Length = 2;
            CheckResize(2);
            fixed (void* b = m_Buffer)
            {
                UnsafeUtility.WriteArrayElement(b, 0, value0);
                UnsafeUtility.WriteArrayElement(b, 1, value1);
            }
        }

        public ResizableArray64Byte(T value0, T value1, T value2)
        {
            m_Length = 3;
            CheckResize(3);
            fixed (void* b = m_Buffer)
            {
                UnsafeUtility.WriteArrayElement(b, 0, value0);
                UnsafeUtility.WriteArrayElement(b, 1, value1);
                UnsafeUtility.WriteArrayElement(b, 2, value2);
            }
        }

        public ResizableArray64Byte(T value0, T value1, T value2, T value3)
        {
            m_Length = 4;
            CheckResize(4);
            fixed (void* b = m_Buffer)
            {
                UnsafeUtility.WriteArrayElement(b, 0, value0);
                UnsafeUtility.WriteArrayElement(b, 1, value1);
                UnsafeUtility.WriteArrayElement(b, 2, value2);
                UnsafeUtility.WriteArrayElement(b, 3, value3);
            }
        }

        public ResizableArray64Byte(T value0, T value1, T value2, T value3, T value4)
        {
            m_Length = 5;
            CheckResize(5);
            fixed (void* b = m_Buffer)
            {
                UnsafeUtility.WriteArrayElement(b, 0, value0);
                UnsafeUtility.WriteArrayElement(b, 1, value1);
                UnsafeUtility.WriteArrayElement(b, 2, value2);
                UnsafeUtility.WriteArrayElement(b, 3, value3);
                UnsafeUtility.WriteArrayElement(b, 4, value4);
            }
        }
    }
}
