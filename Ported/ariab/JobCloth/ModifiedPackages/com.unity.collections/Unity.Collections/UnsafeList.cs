using System;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace Unity.Collections.LowLevel.Unsafe
{
    public unsafe struct UnsafeList
    {
        public void* Ptr;
        public int Length;
        public int Capacity;
        
        public void Dispose(Allocator allocator = Allocator.Persistent) 
        {
            UnsafeUtility.Free(Ptr, allocator);
            Ptr = null;
            Length = 0;
            Capacity = 0;
        }
        
        public void Resize<T>(int targetSize, Allocator allocator = Allocator.Persistent) where T : struct
        {
            SetCapacity<T>(targetSize, allocator);
            Length = targetSize;
        }

        void SetCapacity(int sizeOf, int alignOf, int targetCapacity, Allocator allocator)
        {
            if (targetCapacity > 0)
            {    
                var itemsPerCacheLine = 64 / sizeOf;
                if(targetCapacity < itemsPerCacheLine)
                    targetCapacity = itemsPerCacheLine;
                targetCapacity = CollectionHelper.CeilPow2(targetCapacity);
            }
            var newCapacity = targetCapacity; 
            if (newCapacity == Capacity)
                return;
            void* newPointer = null;
            if (newCapacity > 0)
            {
                var bytesToMalloc = sizeOf * newCapacity;
                newPointer = UnsafeUtility.Malloc(bytesToMalloc, alignOf, allocator );
                if (Capacity > 0)
                {
                    var itemsToCopy = newCapacity < Capacity ? newCapacity : Capacity;
                    var bytesToCopy = itemsToCopy * sizeOf;                        
                    UnsafeUtility.MemCpy(newPointer, Ptr, bytesToCopy);
                }
            }
            if (Capacity > 0)
                UnsafeUtility.Free(Ptr, allocator);
            Ptr = newPointer;
            Capacity = newCapacity;
            if (Length > Capacity)
                Length = Capacity;
        }
        
        public void SetCapacity<T>(int targetCapacity, Allocator allocator = Allocator.Persistent) where T : struct
        {
            SetCapacity(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), targetCapacity, allocator);
        }
        public int IndexOf<T>(T t) where T : struct, IEquatable<T>
        {
            for(int i = Length - 1; i >= 0; --i)
                if(UnsafeUtility.ReadArrayElement<T>(Ptr, i).Equals(t))
                    return i;
            return -1;                        
        }
        
        public bool Contains<T>(T t) where T : struct, IEquatable<T>
        {
            return IndexOf(t) != -1;
        }
        
        public void Add<T>(T t, Allocator allocator = Allocator.Persistent) where T : struct
        {
            Resize<T>(Length + 1, allocator);
            UnsafeUtility.WriteArrayElement(Ptr,Length - 1, t);            
        }
        public void AddRange<T>(void* t, int count, Allocator allocator = Allocator.Persistent) where T : struct
        {
            int previousSize = Length; 
            Resize<T>(previousSize + count, allocator);
            int sizeOf = UnsafeUtility.SizeOf<T>();
            UnsafeUtility.MemCpy((byte*)Ptr + previousSize * sizeOf, t, count * sizeOf);
        }

        void RemoveRangeSwapBack(int sizeOf, int begin, int end)
        {
            int itemsToRemove = end - begin;
            Assert.IsTrue(itemsToRemove > 0);
            void* d = (byte*)Ptr + begin * sizeOf;
            void* s = (byte*)Ptr + (Length - itemsToRemove) * sizeOf;
            UnsafeUtility.MemCpy(d, s, itemsToRemove * sizeOf);
            Length -= itemsToRemove;
        }

        public void RemoveRangeSwapBack<T>(int begin, int end) where T : struct
        {
            RemoveRangeSwapBack(UnsafeUtility.SizeOf<T>(), begin, end);
        }

        public void RemoveAtSwapBack<T>(int index, T t) where T : struct, IEquatable<T>
        {
            Assert.IsTrue(index >= 0 && index < Length);
            Assert.IsTrue(UnsafeUtility.ReadArrayElement<T>(Ptr, index).Equals(t));
            RemoveAtSwapBack<T>(index);
        }

        public void RemoveAtSwapBack<T>(int index) where T : struct
        {
            RemoveRangeSwapBack<T>(index, index + 1);
        }

        void Append(int sizeOf, UnsafeList src)
        {
            var itemsToAppend = src.Length;
            var oldDestSize = Length;
            var newDestSize = Length + itemsToAppend; 
            Assert.IsTrue(Capacity >= newDestSize);
            Length = newDestSize;
            void* d = (byte*)Ptr + oldDestSize * sizeOf;
            void* s = (byte*)src.Ptr;
            UnsafeUtility.MemCpy(d, s, itemsToAppend * sizeOf);
        }
        
        public void Append<T>(UnsafeList src) where T : struct
        {
            Append(UnsafeUtility.SizeOf<T>(), src);
        }
    }
    
    sealed class UnsafePtrListDebugView
    {
        private UnsafePtrList m_UnsafePtrList;
        public UnsafePtrListDebugView(UnsafePtrList UnsafePtrList)
        {
            m_UnsafePtrList = UnsafePtrList;
        }
        public unsafe IntPtr[] Items
        {
            get
            {
                IntPtr[] result = new IntPtr[m_UnsafePtrList.m_size];
                for (var i = 0; i < result.Length; ++i)
                    result[i] = (IntPtr)m_UnsafePtrList.m_pointer[i];
                return result;
            }
        }
    }
    
    [DebuggerTypeProxy(typeof(UnsafePtrListDebugView))]
    public unsafe struct UnsafePtrList
    {
        public void** m_pointer;
        public int m_size;
        public int m_capacity;

        public ref UnsafeList GetUnsafeList()
        {
            return ref *(UnsafeList*)UnsafeUtility.AddressOf(ref this);
        }        
        
        public void Dispose(Allocator allocator = Allocator.Persistent)
        {
            SetCapacity(0, allocator);
        }

        public void Resize(int targetSize, Allocator allocator = Allocator.Persistent)
        {
            GetUnsafeList().Resize<IntPtr>(targetSize, allocator);
        }

        public void SetCapacity(int targetCapacity, Allocator allocator = Allocator.Persistent)
        {
            GetUnsafeList().SetCapacity<IntPtr>(targetCapacity, allocator);
        }
        public int IndexOf(void *t)
        {
            for(int i = m_size - 1; i >= 0; --i)
                if(m_pointer[i] == t)
                    return i;
            return -1;                        
        }
        
        public bool Contains(void* t)
        {
            return IndexOf(t) != -1;
        }
        
        public void Add(void* t, Allocator allocator = Allocator.Persistent)
        {
            Resize(m_size + 1, allocator);
            m_pointer[m_size - 1] = t;
        }

        public void RemoveRangeSwapBack(int begin, int end)
        {
            GetUnsafeList().RemoveRangeSwapBack<IntPtr>(begin, end);
        }

        public void RemoveAtSwapBack(int index, void* expectedValue)
        {
            Assert.IsTrue(index >= 0 && index < m_size);
            Assert.IsTrue(m_pointer[index] == expectedValue);
            RemoveAtSwapBack(index);
        }

        public void RemoveAtSwapBack(int index)
        {
            RemoveRangeSwapBack(index, index + 1);
        }

        public void Append(UnsafePtrList src)
        {
            GetUnsafeList().Append<IntPtr>(src.GetUnsafeList());
        }
    }
    
}
