using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.Serialization;

namespace Unity.Entities
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal unsafe struct BlobAssetOwner : ISharedComponentData, IRefCounted
    {
        [FieldOffset(0)]
        private BlobAssetBatch* BlobAssetBatchPtr;

        public BlobAssetOwner(void* buffer, int expectedTotalDataSize)
        {
            BlobAssetBatchPtr = BlobAssetBatch.CreateFromMemory(buffer, expectedTotalDataSize);
        }

        public void Release()
        {
            if (BlobAssetBatchPtr != null)
                BlobAssetBatch.Release(BlobAssetBatchPtr);
        }
        
        public void Retain()
        {
            if (BlobAssetBatchPtr != null)
                BlobAssetBatch.Retain(BlobAssetBatchPtr);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal unsafe struct BlobAssetBatch
    {
        [FieldOffset(0)]
        private int     TotalDataSize;
        [FieldOffset(4)]
        private int     BlobAssetHeaderCount;
        [FieldOffset(8)]
        private int     RefCount;
        [FieldOffset(12)]
        private int     Padding;

        internal static BlobAssetBatch CreateForSerialize(int blobAssetCount, int totalDataSize)
        {
            return new BlobAssetBatch
            {
                BlobAssetHeaderCount = blobAssetCount,
                TotalDataSize = totalDataSize,
                RefCount = 1,
                Padding = 0
            };
        }


        internal static BlobAssetBatch* CreateFromMemory(void* buffer, int expectedTotalDataSize)
        {
            var batch = (BlobAssetBatch*)buffer; 

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (batch->TotalDataSize != expectedTotalDataSize)
                throw new System.ArgumentException($"TotalSize '{batch->TotalDataSize}' and expected Total size '{expectedTotalDataSize}' are out of sync");
            if (batch->RefCount != 1)
                throw new System.ArgumentException("BlobAssetBatch.Refcount must be 1 on deserialized");

            var header = (BlobAssetHeader*)(batch + 1);
            for (int i = 0; i != batch->BlobAssetHeaderCount; i++)
            { 
                header->ValidationPtr = header + 1;
                if (header->Allocator != Allocator.None)
                    throw new System.ArgumentException("Blob Allocator should be Allocator.None");
                header = (BlobAssetHeader*)(((byte*) (header+1)) + header->Length);
            }
            header--;

            if (header == (byte*) batch + batch->TotalDataSize)
                throw new System.ArgumentException("");
#endif

            return batch;
        }

        public static void Retain(BlobAssetBatch* batch)
        {
            Interlocked.Increment(ref batch->RefCount);
        }

        public static void Release(BlobAssetBatch* batch)
        {
            int newRefCount = Interlocked.Decrement(ref batch->RefCount);
            if (newRefCount <= 0)
            {
                // Debug.Log("Freeing blob");

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (newRefCount < 0)
                    throw new InvalidOperationException("BlobAssetBatch refcount is less than zero. It has been corrupted.");
                
                if (batch->TotalDataSize == 0)
                    throw new InvalidOperationException("BlobAssetBatch has been corrupted. Likely it has already been unloaded or released.");
                
                var header = (BlobAssetHeader*)(batch + 1);
                for (int i = 0; i != batch->BlobAssetHeaderCount; i++)
                {
                    if (header->ValidationPtr != (header + 1))
                        throw new InvalidOperationException("The BlobAssetReference has been corrupted. Likely it has already been unloaded or released.");

                    header->Invalidate();
                    header = (BlobAssetHeader*)(((byte*) (header+1)) + header->Length);
                }
                header--;
                
                if (header == (byte*) batch + batch->TotalDataSize)
                    throw new InvalidOperationException("BlobAssetBatch has been corrupted. Likely it has already been unloaded or released.");

                batch->TotalDataSize = 0;
                batch->BlobAssetHeaderCount = 0;
#endif
                
                UnsafeUtility.Free(batch, Allocator.Persistent);
            }
        }
    }
    
    
    //@TODO: Compress to 8 bytes?
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    unsafe struct BlobAssetHeader
    {
        [FieldOffset(0)]  public void* ValidationPtr;
        [FieldOffset(8)]  public int Length;
        [FieldOffset(12)] public Allocator Allocator;

        internal static BlobAssetHeader CreateForSerialize(int length)
        {
            return new BlobAssetHeader
            {
                ValidationPtr = null,
                Length = length,
                Allocator = Allocator.None
            };
        }


        public void Invalidate()
        {
            ValidationPtr = (void*)0xdddddddddddddddd;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal unsafe struct BlobAssetReferenceData
    {
        [NativeDisableUnsafePtrRestriction]
        [FieldOffset(0)]
        public byte* m_Ptr;
    
        internal BlobAssetHeader* Header
        {
            get { return ((BlobAssetHeader*) m_Ptr) - 1; }
        }

        
        [BurstDiscard]
        void ValidateNonBurst()
        {
            void* validationPtr = null;
            try
            {
                // Try to read ValidationPtr, this might throw if the memory has been unmapped
                validationPtr = Header->ValidationPtr;
            }
            catch(Exception)
            {
            }

            if (validationPtr != m_Ptr)
                throw new InvalidOperationException("The BlobAssetReference is not valid. Likely it has already been unloaded or released.");
        }

        void ValidateBurst()
        {
            void* validationPtr = Header->ValidationPtr;
            if(validationPtr != m_Ptr)
                throw new InvalidOperationException("The BlobAssetReference is not valid. Likely it has already been unloaded or released.");
        }

        
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void ValidateNotNull()
        {
            if(m_Ptr == null)
                throw new InvalidOperationException("The BlobAssetReference is null.");

            ValidateNonBurst();
            ValidateBurst();
        }
        
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void ValidateAllowNull()
        {
            if (m_Ptr == null)
                return;

            ValidateNonBurst();
            ValidateBurst();
        }
    }

    [ChunkSerializable]
    public unsafe struct BlobAssetReference<T> : IDisposable, IEquatable<BlobAssetReference<T>> 
        where T : struct
    {
        internal BlobAssetReferenceData m_data;
        public bool IsCreated
        {
            get { return m_data.m_Ptr != null; }
        }
        
        public void* GetUnsafePtr()
        {
            m_data.ValidateAllowNull();
            return m_data.m_Ptr;
        }

        //[Obsolete("Use Dispose instead")]
        public void Release()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_data.ValidateNotNull();
            var header = m_data.Header;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(header->Allocator == Allocator.None)
                throw new InvalidOperationException("It's not possible to release a blob asset reference that was deserialized. It will be automatically released when the scene is unloaded ");
            m_data.Header->Invalidate();
#endif

            UnsafeUtility.Free(header, header->Allocator);
            m_data.m_Ptr = null;
        }

        public ref T Value
        {
            get
            {
                m_data.ValidateNotNull();
                return ref UnsafeUtilityEx.AsRef<T>(m_data.m_Ptr);
            }
        }

        
        public static BlobAssetReference<T> Create(void* ptr, int length)
        {
            byte* buffer =
                (byte*) UnsafeUtility.Malloc(sizeof(BlobAssetHeader) + length, 16, Allocator.Persistent);
            UnsafeUtility.MemCpy(buffer + sizeof(BlobAssetHeader), ptr, length);

            BlobAssetHeader* header = (BlobAssetHeader*) buffer;
            *header = new BlobAssetHeader();

            header->Length = length;
            header->Allocator = Allocator.Persistent;

            BlobAssetReference<T> blobAssetReference;
            header->ValidationPtr = blobAssetReference.m_data.m_Ptr = buffer + sizeof(BlobAssetHeader);
            return blobAssetReference;
        }
        
        public static BlobAssetReference<T> Create(byte[] data)
        {
            fixed (byte* ptr = &data[0])
            {
                return Create(ptr, data.Length);
            }
        }

        public static BlobAssetReference<T> Create(T value)
        {
            return Create(UnsafeUtility.AddressOf(ref value), UnsafeUtility.SizeOf<T>());
        }

        public static BlobAssetReference<T> Null => new BlobAssetReference<T>();

        public static bool operator ==(BlobAssetReference<T> lhs, BlobAssetReference<T> rhs)
        {
            return lhs.m_data.m_Ptr == rhs.m_data.m_Ptr;
        }

        public static bool operator !=(BlobAssetReference<T> lhs, BlobAssetReference<T> rhs)
        {
            return lhs.m_data.m_Ptr != rhs.m_data.m_Ptr;
        }

        public bool Equals(BlobAssetReference<T> other)
        {
            return m_data.Equals(other.m_data);
        }

        public override bool Equals(object obj)
        {
            return this == (BlobAssetReference<T>)obj;
        }

        public override int GetHashCode()
        {
            return m_data.GetHashCode();
        }
    }

    unsafe public struct BlobPtr<T> where T : struct
    {
        internal int m_OffsetPtr;

        public ref T Value
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if(m_OffsetPtr == 0)
                    throw new System.InvalidOperationException("The accessed BlobPtr hasn't been allocated.");
#endif
                fixed (int* thisPtr = &m_OffsetPtr)
                {
                    return ref UnsafeUtilityEx.AsRef<T>((byte*) thisPtr + m_OffsetPtr);
                }
            }
        }

        public void* GetUnsafePtr()
        {
            if (m_OffsetPtr == 0)
                return null;

            fixed (int* thisPtr = &m_OffsetPtr)
            {
                return (byte*) thisPtr + m_OffsetPtr;
            }
        }
    }

    unsafe public struct BlobArray<T> where T : struct
    {
        internal int m_OffsetPtr;
        internal int m_Length;

        public int Length
        {
            get { return m_Length; }
        }

        public void* GetUnsafePtr()
        {
            // for an unallocated array this will return an invalid pointer which is ok since it
            // should never be accessed as Length will be 0
            fixed (int* thisPtr = &m_OffsetPtr)
            {
                return (byte*) thisPtr + m_OffsetPtr;
            }
        }

        public ref T this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint) index >= (uint) m_Length)
                    throw new System.IndexOutOfRangeException(string.Format("Index {0} is out of range Length {1}",
                        index, m_Length));
#endif

                fixed (int* thisPtr = &m_OffsetPtr)
                {
                    return ref UnsafeUtilityEx.ArrayElementAsRef<T>((byte*) thisPtr + m_OffsetPtr, index);
                }
            }
        }
    }

    unsafe public struct BlobString
    {
        internal BlobArray<char> Data;
        public int Length
        {
            get { return Data.Length; }
        }

        public new string ToString()
        {
            return new string((char*) Data.GetUnsafePtr(), 0, Data.Length);
        }
    }
    
    public static class BlobStringExtensions
    {
        unsafe public static void AllocateString(ref this BlobBuilder builder, ref BlobString blobStr, string value)
        {
            var res = builder.Allocate(ref blobStr.Data, value.Length);
            var len = value.Length;
            fixed (char* p = value)
            {
                UnsafeUtility.MemCpy(res.GetUnsafePtr(), p, sizeof(char) * len);
            }
        }
    }

    public static class BlobAssetSerializeExtensions
    {
        unsafe public static void Write<T>(this BinaryWriter binaryWriter, BlobAssetReference<T> blob) where T : struct
        {
            binaryWriter.WriteBytes(blob.m_data.Header, blob.m_data.Header->Length + sizeof(BlobAssetHeader));
        }

        unsafe public static BlobAssetReference<T> Read<T>(this BinaryReader binaryReader) where T : struct
        {
            BlobAssetHeader header;
            binaryReader.ReadBytes(&header, sizeof(BlobAssetHeader));
            
            var buffer = (byte*) UnsafeUtility.Malloc(sizeof(BlobAssetHeader) + header.Length, 16, Allocator.Persistent);
            binaryReader.ReadBytes(buffer + sizeof(BlobAssetHeader), header.Length);

            var bufferHeader = (BlobAssetHeader*) buffer;
            bufferHeader->Allocator = Allocator.Persistent;
            bufferHeader->Length = header.Length;
            bufferHeader->ValidationPtr = buffer + sizeof(BlobAssetHeader);
            
            BlobAssetReference<T> blobAssetReference;
            blobAssetReference.m_data.m_Ptr = buffer + sizeof(BlobAssetHeader);
            
            return blobAssetReference;
        }
    }
}
