using System;
using System.Diagnostics;

namespace Unity.Collections.LowLevel.Unsafe
{
    public unsafe struct UnsafeAppendBuffer : IDisposable
    {
        public byte* Ptr;
        public int Size;
        public int Capacity;
        public readonly int Alignment;
        public readonly Allocator Allocator;

        public bool IsEmpty => Size == 0;

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckAlignment(int alignment)
        {
            var zeroAlignment = alignment == 0;
            var powTwoAlignment = ((alignment - 1) & alignment) == 0;
            var validAlignment = (!zeroAlignment) && powTwoAlignment;

            if (!validAlignment)
                throw new ArgumentException($"Specified alignment must be non-zero positive power of two. Requested: {alignment}");
        }

        public UnsafeAppendBuffer(int initialCapacity, int alignment, Allocator allocator)
        {
            Alignment = alignment;
            Allocator = allocator;
            Ptr = null;
            Size = 0;
            Capacity = 0;

            CheckAlignment(alignment);
            SetCapacity(initialCapacity);
        }

        public UnsafeAppendBuffer(void* externalBuffer, int capacity)
        {
            Alignment = 0;
            Allocator = Allocator.Invalid;
            Ptr = (byte*)externalBuffer;
            Size = 0;
            Capacity = capacity;
        }
        
        public void Dispose()
        {
            if (Allocator != Allocator.Invalid)
                UnsafeUtility.Free(Ptr, Allocator);
            Ptr = null;
            Size = 0;
            Capacity = 0;
        }

        public void Reset()
        {
            Size = 0;
        }

        public void SetCapacity(int targetCapacity)
        {
            if (targetCapacity <= Capacity)
                return;

            if (targetCapacity < 64)
                targetCapacity = 64;
            else
                targetCapacity = CollectionHelper.CeilPow2(targetCapacity);
            
            var newPtr = (byte*) UnsafeUtility.Malloc(targetCapacity, Alignment, Allocator);
            if (Ptr != null)
            {
                UnsafeUtility.MemCpy(newPtr, Ptr, Size);
                UnsafeUtility.Free(Ptr, Allocator);
            }

            Ptr = newPtr;
            Capacity = targetCapacity;
        }

        public void ResizeUninitialized(int requestedSize)
        {
            if (requestedSize > Capacity)
                SetCapacity(requestedSize);

            Size = requestedSize;
        }

        public void Add<T>(T t) where T : struct
        {
            var structSize = UnsafeUtility.SizeOf<T>();

            SetCapacity(Size + structSize);
            UnsafeUtility.CopyStructureToPtr(ref t, Ptr + Size);
            Size += structSize;
        }

        public void Add(void* t, int structSize)
        {
            SetCapacity(Size + structSize);
            UnsafeUtility.MemCpy(Ptr + Size, t, structSize);
            Size += structSize;
        }

        public void Add<T>(NativeArray<T> value) where T : struct
        {
            Add(value.Length);
            Add(NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(value), UnsafeUtility.SizeOf<T>() * value.Length);
        }

        public void Add(string value)
        {
            if (value != null)
            {
                Add(value.Length);
                fixed (char* ptr = value)
                    Add(ptr, sizeof(char) * value.Length);
            }
            else
            {
                Add(-1);
            }
        }
        
        public T Pop<T>() where T : struct
        {
            int structSize = UnsafeUtility.SizeOf<T>();
            long ptr = (long)Ptr;
            long size = (long)Size;
            long addr = ptr + size - (long)structSize;

            var t = UnsafeUtility.ReadArrayElement<T>((void*)addr, 0);
            Size -= structSize;
            return t;
        }

        public void Pop(void* t, int structSize)
        {
            long ptr = (long)Ptr;
            long size = (long)Size;
            long addr = ptr + size - (long)structSize;

            UnsafeUtility.MemCpy(t, (void*)addr, structSize);
            Size -= structSize;
        }
        
        public byte[] ToBytes()
        {
            var dst = new byte[Size];
            fixed (byte* dstPtr = dst)
            {
                UnsafeUtility.MemCpy(dstPtr, Ptr, Size);
            }
            return dst;
        }

        public Reader AsReader()
        {
            return new Reader(ref this);
        }

        public unsafe struct Reader
        {
            public readonly byte* Ptr;
            public readonly int Size;
            public int Offset;

            public bool EndOfBuffer => Offset == Size;

            public Reader(ref UnsafeAppendBuffer buffer)
            {
                Ptr = buffer.Ptr;
                Size = buffer.Size;
                Offset = 0;
            }
            
            public Reader(void* buffer, int size)
            {
                Ptr = (byte*)buffer;
                Size = size;
                Offset = 0;
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void CheckBounds(int structSize)
            {
                if (Offset + structSize > Size)
                    throw new ArgumentException("Requested value outside bounds of UnsafeAppendOnlyBuffer. Remaining bytes: {Buffer->Size - m_Offset} Requested: {structSize}");
            }

            public void ReadNext<T>(out T value) where T : struct
            {
                var structSize = UnsafeUtility.SizeOf<T>();
                CheckBounds(structSize);

                UnsafeUtility.CopyPtrToStructure<T>(Ptr + Offset, out value);
                Offset += structSize;
            }

            public T ReadNext<T>() where T : struct
            {
                var structSize = UnsafeUtility.SizeOf<T>();
                CheckBounds(structSize);

                T value = UnsafeUtility.ReadArrayElement<T>(Ptr + Offset, 0);
                Offset += structSize;
                return value;
            }

            public void* ReadNext(int structSize)
            {
                CheckBounds(structSize);

                var value = (void*)((IntPtr)Ptr + Offset);
                Offset += structSize;
                return value;
            }

            public void ReadNext<T>(out NativeArray<T> value, Allocator allocator) where T : struct
            {
                var length = ReadNext<int>();
                value = new NativeArray<T>(length, allocator);
                var size = length * UnsafeUtility.SizeOf<T>();
                if (size > 0)
                {
                    var ptr = ReadNext(size);
                    UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafePtr(value), ptr, size);
                }
            }
            
#if !NET_DOTS
            public void ReadNext(out string value)
            {
                int length;
                ReadNext(out length);
                
                if (length != -1)
                {
                    value = new string('0', length);

                    fixed (char* buf = value)
                    {
                        int bufLen = length * sizeof(char);
                        UnsafeUtility.MemCpy(buf, ReadNext(bufLen), bufLen);
                    }
                }
                else
                {
                    value = null;
                }
            }
#endif
        }
    }
}
