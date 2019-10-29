using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.Serialization;
using Unity.Mathematics;

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
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    unsafe struct BlobAssetHeader
    {
        [FieldOffset(0)]  public void* ValidationPtr;
        [FieldOffset(8)]  public int Length;
        [FieldOffset(12)] public Allocator Allocator;
        [FieldOffset(16)] public ulong Hash;

        internal static BlobAssetHeader CreateForSerialize(int length, ulong hash)
        {
            return new BlobAssetHeader
            {
                ValidationPtr = null,
                Length = length,
                Allocator = Allocator.None,
                Hash = hash
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

        public void Dispose()
        {
            ValidateNotNull();
            var header = Header;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(header->Allocator == Allocator.None)
                throw new InvalidOperationException("It's not possible to release a blob asset reference that was deserialized. It will be automatically released when the scene is unloaded ");
            Header->Invalidate();
#endif

            UnsafeUtility.Free(header, header->Allocator);
            m_Ptr = null;
        }
    }

    /// <summary>
    /// A reference to a blob asset stored in unmanaged memory.
    /// </summary>
    /// <remarks>Create a blob asset using a <see cref="BlobBuilder"/> or by deserializing a serialized blob asset.</remarks>
    /// <typeparam name="T">The struct data type defining the data structure of the blob asset.</typeparam>
    [ChunkSerializable]
    public unsafe struct BlobAssetReference<T> : IDisposable, IEquatable<BlobAssetReference<T>> 
        where T : struct
    {
        internal BlobAssetReferenceData m_data;
        /// <summary>
        /// Reports whether this instance references a valid blob asset.
        /// </summary>
        /// <value>True, if this instance references a valid blob instance.</value>
        public bool IsCreated
        {
            get { return m_data.m_Ptr != null; }
        }
        
        /// <summary>
        /// Provides an unsafe pointer to the blob asset data.
        /// </summary>
        /// <remarks>You can only use unsafe pointers in <see cref="Unsafe"/> contexts.</remarks>
        /// <returns>An unsafe pointer. The pointer is null for invalid BlobAssetReference instances.</returns>
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

        /// <summary>
        /// Destroys the referenced blob asset and frees its memory.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if you attempt to dispose a blob asset that loaded as
        /// part of a scene or subscene.</exception>
        public void Dispose()
        {
            m_data.Dispose();
        }

        /// <summary>
        /// A reference to the blob asset data.
        /// </summary>
        /// <remarks>The property is a
        /// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/ref-returns">
        /// reference return</see>.</remarks>
        /// <typeparam name="T">The struct type stored in the blob asset.</typeparam>
        /// <value>The root data structure of the blob asset data.</value>
        public ref T Value
        {
            get
            {
                m_data.ValidateNotNull();
                return ref UnsafeUtilityEx.AsRef<T>(m_data.m_Ptr);
            }
        }

        
        /// <summary>
        /// Creates a blob asset from a pointer to data and a specified size.
        /// </summary>
        /// <remarks>The blob asset is created in unmanaged memory. Call <see cref="Dispose"/> to free the asset memory
        /// when it is no longer needed. This function can only be used in an <see cref="Unsafe"/> context.</remarks>
        /// <param name="ptr">A pointer to the buffer containing the data to store in the blob asset.</param>
        /// <param name="length">The length of the buffer in bytes.</param>
        /// <returns>A reference to newly created blob asset.</returns>
        /// <seealso cref="BlobBuilder"/>
        public static BlobAssetReference<T> Create(void* ptr, int length)
        {
            byte* buffer =
                (byte*) UnsafeUtility.Malloc(sizeof(BlobAssetHeader) + length, 16, Allocator.Persistent);
            UnsafeUtility.MemCpy(buffer + sizeof(BlobAssetHeader), ptr, length);

            BlobAssetHeader* header = (BlobAssetHeader*) buffer;
            *header = new BlobAssetHeader();

            header->Length = length;
            header->Allocator = Allocator.Persistent;
            
            // @TODO use 64bit hash
            header->Hash = math.hash(ptr, length);

            BlobAssetReference<T> blobAssetReference;
            header->ValidationPtr = blobAssetReference.m_data.m_Ptr = buffer + sizeof(BlobAssetHeader);
            return blobAssetReference;
        }

        /// <summary>
        /// Creates a blob asset from a byte array.
        /// </summary>
        /// <remarks>The blob asset is created in unmanaged memory. Call <see cref="Dispose"/> to free the asset memory
        /// when it is no longer needed. This function can only be used in an <see cref="Unsafe"/> context.</remarks>
        /// <param name="data">The byte array containing the data to store in the blob asset.</param>
        /// <returns>A reference to newly created blob asset.</returns>
        /// <seealso cref="BlobBuilder"/>
        public static BlobAssetReference<T> Create(byte[] data)
        {
            fixed (byte* ptr = &data[0])
            {
                return Create(ptr, data.Length);
            }
        }

        /// <summary>
        /// Creates a blob asset from an instance of a struct.
        /// </summary>
        /// <remarks>The struct must only contain blittable fields (primitive types, fixed-length arrays, or other structs
        /// meeting these same criteria). The blob asset is created in unmanaged memory. Call <see cref="Dispose"/> to
        /// free the asset memory when it is no longer needed. This function can only be used in an <see cref="Unsafe"/>
        /// context.</remarks>
        /// <param name="value">An instance of <typeparamref name="T"/>.</param>
        /// <returns>A reference to newly created blob asset.</returns>
        /// <seealso cref="BlobBuilder"/>
        public static BlobAssetReference<T> Create(T value)
        {
            return Create(UnsafeUtility.AddressOf(ref value), UnsafeUtility.SizeOf<T>());
        }

        /// <summary>
        /// Construct a BlobAssetReference from the blob data
        /// </summary>
        /// <param name="blobData">The blob data to attach to the returned object</param>
        /// <returns>The created BlobAssetReference</returns>
        internal static BlobAssetReference<T> Create(BlobAssetReferenceData blobData)
        {
            return new BlobAssetReference<T> { m_data = blobData };
        }
        
#if !NET_DOTS
        /// <summary>
        /// Reads bytes from a fileName, validates the expected serialized version, and deserializes them into a new blob asset.
        /// </summary>
        /// <param name="path">The path of the blob data to read.</param>
        /// <param name="version">Expected version number of the blob data.</param>
        /// <param name="result">The resulting BlobAssetReference if the data was read successful.</param>
        /// <returns>A bool if the read was successful or not.</returns>
        public static bool TryRead(string path, int version, out BlobAssetReference<T> result)
        {
            using (var binaryReader = new StreamBinaryReader(path))
            {
                var storedVersion = binaryReader.ReadInt();
                if (storedVersion != version)
                {
                    result = default;
                    return false;
                }

                result = binaryReader.Read<T>();
                return true;
            }
        }

        /// <summary>
        /// Writes the blob data to a path with serialized version.
        /// </summary>
        /// <param name="builder">The BlobBuilder containing the blob to write.</param>
        /// <param name="path">The path to write the blob data.</param>
        /// <param name="version">Serialized version number of the blob data.</param>
        public static void Write(BlobBuilder builder, string path, int verison)
        {
            using (var asset = builder.CreateBlobAssetReference<T>(Allocator.TempJob))
            using (var writer = new StreamBinaryWriter(path))
            {
                writer.Write(verison);
                writer.Write(asset);
            }
        }
#endif

        /// <summary>
        /// A "null" blob asset reference that can be used to test if a BlobAssetReference instance
        /// </summary>
        public static BlobAssetReference<T> Null => new BlobAssetReference<T>();

        /// <summary>
        /// Two BlobAssetReferences are equal when they reference the same data.
        /// </summary>
        /// <param name="lhs">The BlobAssetReference on the left side of the operator.</param>
        /// <param name="rhs">The BlobAssetReference on the right side of the operator.</param>
        /// <returns>True, if both references point to the same data or if both are <see cref="Null"/>.</returns>
        public static bool operator ==(BlobAssetReference<T> lhs, BlobAssetReference<T> rhs)
        {
            return lhs.m_data.m_Ptr == rhs.m_data.m_Ptr;
        }

        /// <summary>
        /// Two BlobAssetReferences are not equal unless they reference the same data.
        /// </summary>
        /// <param name="lhs">The BlobAssetReference on the left side of the operator.</param>
        /// <param name="rhs">The BlobAssetReference on the right side of the operator.</param>
        /// <returns>True, if the references point to different data in memory or if one is <see cref="Null"/>.</returns>
        public static bool operator !=(BlobAssetReference<T> lhs, BlobAssetReference<T> rhs)
        {
            return lhs.m_data.m_Ptr != rhs.m_data.m_Ptr;
        }

        /// <summary>
        /// Two BlobAssetReferences are equal when they reference the same data.
        /// </summary>
        /// <param name="other">The reference to compare to this one.</param>
        /// <returns>True, if both references point to the same data or if both are <see cref="Null"/>.</returns>
        public bool Equals(BlobAssetReference<T> other)
        {
            return m_data.Equals(other.m_data);
        }

        /// <summary>
        /// Two BlobAssetReferences are equal when they reference the same data.
        /// </summary>
        /// <param name="obj">The object to compare to this reference</param>
        /// <returns>True, if the object is a BlobAssetReference instance that references to the same data as this one,
        /// or if both objects are <see cref="Null"/> BlobAssetReference instances.</returns>
        public override bool Equals(object obj)
        {
            return this == (BlobAssetReference<T>)obj;
        }

        /// <summary>
        /// Generates the hash code for this object.
        /// </summary>
        /// <returns>A standard C# value-type hash code.</returns>
        public override int GetHashCode()
        {
            return m_data.GetHashCode();
        }
    }

    /// <summary>
    /// A pointer referencing a struct, array, or field inside a blob asset.
    /// </summary>
    /// <typeparam name="T">The data type of the referenced object.</typeparam>
    /// <seealso cref="BlobBuilder"/>
    unsafe public struct BlobPtr<T> where T : struct
    {
        internal int m_OffsetPtr;

        /// <summary>
        /// The value, of type <typeparamref name="T"/> to which the pointer refers.
        /// </summary>
        /// <remarks>The property is a
        /// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/ref-returns">
        /// reference return</see>.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if the pointer does not reference a valid instance of
        /// a data type.</exception>
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

        /// <summary>
        /// Provides an unsafe pointer to the referenced data.
        /// </summary>
        /// <remarks>You can only use unsafe pointers in <see cref="Unsafe"/> contexts.</remarks>
        /// <returns>An unsafe pointer.</returns>
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

    /// <summary>
    ///  An immutable array of value types stored in a blob asset.
    /// </summary>
    /// <remarks>When creating a blob asset, use the <see cref="BlobBuilderArray{T}"/> provided by a
    /// <see cref="BlobBuilder"/> instance to set the array elements.</remarks>
    /// <typeparam name="T">The data type of the elements in the array. Must be a struct or other value type.</typeparam>
    /// <seealso cref="BlobBuilder"/>
    unsafe public struct BlobArray<T> where T : struct
    {
        internal int m_OffsetPtr;
        internal int m_Length;

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public int Length
        {
            get { return m_Length; }
        }

        /// <summary>
        /// Provides an unsafe pointer to the array data.
        /// </summary>
        /// <remarks>You can only use unsafe pointers in <see cref="Unsafe"/> contexts.</remarks>
        /// <returns>An unsafe pointer.</returns>
        public void* GetUnsafePtr()
        {
            // for an unallocated array this will return an invalid pointer which is ok since it
            // should never be accessed as Length will be 0
            fixed (int* thisPtr = &m_OffsetPtr)
            {
                return (byte*) thisPtr + m_OffsetPtr;
            }
        }

        /// <summary>
        /// The element of the array at the <paramref name="index"/> position.
        /// </summary>
        /// <param name="index">The array index.</param>
        /// <remarks>The array element is a
        /// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/ref-returns">
        /// reference return</see>.</remarks>
        /// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is out of bounds.</exception>
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

        /// <summary>
        /// Copies the elements of the BlobArray<T> to a new managed array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the BlobArray<T>.</returns>
        public T[] ToArray()
        {
            var result = new T[m_Length];
            if (m_Length > 0)
            {
                var src = GetUnsafePtr();
                
                var handle = GCHandle.Alloc(result, GCHandleType.Pinned);
                var addr = handle.AddrOfPinnedObject();

                UnsafeUtility.MemCpy((void*)addr, src, m_Length * UnsafeUtility.SizeOf<T>());

                handle.Free();
            }
            return result;
        }
    }

    /// <summary>
    /// An immutable, variable-length string stored in a blob asset.
    /// </summary>
    /// <seealso cref="BlobBuilder"/>
    unsafe public struct BlobString
    {
        internal BlobArray<char> Data;
        /// <summary>
        /// The length of the string in characters.
        /// </summary>
        public int Length
        {
            get { return Data.Length; }
        }

        /// <summary>
        /// Converts this BlobString to a standard C# <see cref="string"/>.
        /// </summary>
        /// <returns>The C# string.</returns>
        public new string ToString()
        {
            return new string((char*) Data.GetUnsafePtr(), 0, Data.Length);
        }
    }
    
    /// <summary>
    /// Extensions that allow the creation of <see cref="BlobString"/> instances by a <see cref="BlobBuilder"/>.
    /// </summary>
    public static class BlobStringExtensions
    {
        /// <summary>
        /// Allocates memory to store the string in a blob asset and copies the string data into it.
        /// </summary>
        /// <param name="builder">The BlobBuilder instance building the blob asset.</param>
        /// <param name="blobStr">A reference to the field in the blob asset that will store the string. This
        /// function allocates memory for that field and sets the string value.</param>
        /// <param name="value">The string to copy into the blob asset.</param>
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

    /// <summary>
    /// Extensions for supporting serialization and deserialization of blob assets.
    /// </summary>
    public static class BlobAssetSerializeExtensions
    {
        /// <summary>
        /// Serializes the blob asset data and writes the bytes to a <see cref="BinaryWriter"/> instance.
        /// </summary>
        /// <param name="binaryWriter">An implementation of the BinaryWriter interface.</param>
        /// <param name="blob">A reference to the blob asset to serialize.</param>
        /// <typeparam name="T">The blob asset's root data type.</typeparam>
        /// <seealso cref="StreamBinaryWriter"/>
        /// <seealso cref="MemoryBinaryWriter"/>
        unsafe public static void Write<T>(this BinaryWriter binaryWriter, BlobAssetReference<T> blob) where T : struct
        {
            var blobAssetLength = blob.m_data.Header->Length;
            var serializeReadyHeader = BlobAssetHeader.CreateForSerialize(blobAssetLength, blob.m_data.Header->Hash);
            
            binaryWriter.WriteBytes(&serializeReadyHeader, sizeof(BlobAssetHeader));
            binaryWriter.WriteBytes(blob.m_data.Header + 1, blobAssetLength);
        }

        /// <summary>
        /// Reads bytes from a <see cref="BinaryReader"/> instance and deserializes them into a new blob asset.
        /// </summary>
        /// <param name="binaryReader">An implementation of the BinaryReader interface.</param>
        /// <typeparam name="T">The blob asset's root data type.</typeparam>
        /// <returns>A reference to the deserialized blob asset.</returns>
        /// <seealso cref="StreamBinaryReader"/>
        /// <seealso cref="MemoryBinaryReader"/>
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
            
            // @TODO use 64bit hash
            bufferHeader->Hash = header.Hash;
            
            BlobAssetReference<T> blobAssetReference;
            blobAssetReference.m_data.m_Ptr = buffer + sizeof(BlobAssetHeader);

            return blobAssetReference;
        }
    }
}
