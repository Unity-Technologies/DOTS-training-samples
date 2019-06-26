using System;
using System.Diagnostics;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Collections.LowLevel.Unsafe
{
    public unsafe struct UnsafeAppendOnlyBuffer : IDisposable
    {
        public void* Ptr; 
        public int Size; 
        public int Capacity;
        
        public readonly int Alignment;
        public readonly Allocator Allocator;
        
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")] 
        void CheckAlignment(int alignment)
        {
            var zeroAlignment = alignment == 0;
            var powTwoAlignment = ((alignment - 1) & alignment) == 0;
            var validAlignment = (!zeroAlignment) && powTwoAlignment;
            
            if (!validAlignment)
                throw new ArgumentException($"Specified alignment must be non-zero positive power of two. Requested: {alignment}");
        }

        public UnsafeAppendOnlyBuffer(int initialCapacity, int alignment, Allocator allocator = Allocator.Persistent)
        {
            Alignment = alignment;
            Allocator = allocator;
            Ptr = null;
            Size = 0;
            Capacity = 0;
            
            CheckAlignment(alignment);
            Resize(initialCapacity);
        }
        
        public void Dispose()
        {
            UnsafeUtility.Free(Ptr, Allocator);
            Ptr = null;
            Size = 0;
            Capacity = 0;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")] 
        void CheckGrowOnly(int targetCapacity)
        {
            if (targetCapacity < Capacity)
                throw new ArgumentException("Cannot reduce size of UnsafeAppendOnlyBuffer. Was: {m_Capacity} Requested: {targetCapacity}");
        }

        public void Resize(int requestedCapacity)
        {
            var targetCapacity = CollectionHelper.CeilPow2(CollectionHelper.Align(requestedCapacity, JobsUtility.CacheLineSize));
            if (targetCapacity == Capacity)
                return;
            
            CheckGrowOnly(targetCapacity);
            
            var newPtr = UnsafeUtility.Malloc(targetCapacity, Alignment, Allocator);
            if (Ptr != null)
            {
                UnsafeUtility.MemCpy(newPtr, Ptr, Size);
                UnsafeUtility.Free(Ptr, Allocator);
            }

            Ptr = newPtr;
            Capacity = targetCapacity;
        }

        public void Add<T>(T t) where T : struct
        {
            var structSize = CollectionHelper.Align(UnsafeUtility.SizeOf<T>(), Alignment);

            Resize(Size + structSize);
            
            UnsafeUtility.WriteArrayElement((void*)((IntPtr)Ptr + Size), 0, t);
            Size += structSize;
        }
        
        public void Add(void* t, int size) 
        {
            var structSize = CollectionHelper.Align(size, Alignment);

            Resize(Size + structSize);
            
            UnsafeUtility.MemCpy((void*)((IntPtr)Ptr + Size), t, size);
            Size += structSize;
        }
    }

    public unsafe struct UnsafeAppendOnlyBufferReader
    {
        public readonly UnsafeAppendOnlyBuffer* Buffer; 
        public int Offset;

        public bool EndOfBuffer => Offset == Buffer->Size;

        public UnsafeAppendOnlyBufferReader(UnsafeAppendOnlyBuffer* buffer)
        {
            Buffer = buffer;
            Offset = 0;
        }
        
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")] 
        void CheckBounds(int structSize)
        {
            if (Offset + structSize > Buffer->Size)
                throw new ArgumentException("Requested value outside bounds of UnsafeAppendOnlyBuffer. Remaining bytes: {Buffer->Size - m_Offset} Requested: {structSize}");
        }
        
        public T ReadNext<T>() where T : struct
        {
            var structSize = CollectionHelper.Align(UnsafeUtility.SizeOf<T>(), Buffer->Alignment);
            CheckBounds(structSize);

            T value = UnsafeUtility.ReadArrayElement<T>((void*)((IntPtr)Buffer->Ptr + Offset), 0);
            Offset += structSize;
            return value;
        }
        
        public void* ReadNext(int size) 
        {
            var structSize = CollectionHelper.Align(size, Buffer->Alignment);
            CheckBounds(structSize);

            var value = (void*) ((IntPtr) Buffer->Ptr + Offset);
            Offset += structSize;
            return value;
        }
    }
}