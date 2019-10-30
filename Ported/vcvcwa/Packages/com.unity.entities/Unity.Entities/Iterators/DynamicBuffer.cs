using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
	/// <summary>
	/// An array-like data structure that can be used as a component.
	/// </summary>
    /// <example>
    /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.class"/>
    /// </example>
    /// <typeparam name="T">The data type stored in the buffer. Must be a value type.</typeparam>
	[StructLayout(LayoutKind.Sequential)]
	[NativeContainer]
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    [DebuggerTypeProxy(typeof(DynamicBufferDebugView < >))]
    public unsafe struct DynamicBuffer<T> : IEnumerable<T> where T : struct
    {
        [NativeDisableUnsafePtrRestriction]
        BufferHeader* m_Buffer;

        // Stores original internal capacity of the buffer header, so heap excess can be removed entirely when trimming.
        private int m_InternalCapacity;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
	    internal AtomicSafetyHandle m_Safety0;
	    internal AtomicSafetyHandle m_Safety1;
        internal int m_SafetyReadOnlyCount;
        internal int m_SafetyReadWriteCount;
        internal bool m_IsReadOnly;
        internal bool m_useMemoryInitPattern;
        internal byte m_memoryInitPattern;
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal DynamicBuffer(BufferHeader* header, AtomicSafetyHandle safety, AtomicSafetyHandle arrayInvalidationSafety, bool isReadOnly, bool useMemoryInitPattern, byte memoryInitPattern, int internalCapacity)
        {
            m_Buffer = header;
            m_Safety0 = safety;
            m_Safety1 = arrayInvalidationSafety;
            m_SafetyReadOnlyCount = isReadOnly ? 2 : 0;
            m_SafetyReadWriteCount = isReadOnly ? 0 : 2;
            m_IsReadOnly = isReadOnly;
            m_InternalCapacity = internalCapacity;
            m_useMemoryInitPattern = useMemoryInitPattern;
            m_memoryInitPattern = memoryInitPattern;
        }
#else
        internal DynamicBuffer(BufferHeader* header, int internalCapacity)
        {
            m_Buffer = header;
            m_InternalCapacity = internalCapacity;
        }
#endif

        /// <summary>
        /// The number of elements the buffer holds.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.length"/>
        /// </example>
        public int Length
        {
            get
            {
                CheckReadAccess();
                return m_Buffer->Length;
            }
        }

        /// <summary>
        /// The number of elements the buffer can hold.
        /// </summary>
        public int Capacity
        {
            get
            {
                CheckReadAccess();
                return m_Buffer->Capacity;
            }
        }

        /// <summary>
        /// Whether the memory for this dynamic buffer has been allocated.
        /// </summary>
        public bool IsCreated
        {
            get
            {
                return m_Buffer != null;
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckBounds(int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException($"Index {index} is out of range in DynamicBuffer of '{Length}' Length.");
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckReadAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety0);
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety1);
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckWriteAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety0);
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety1);
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckWriteAccessAndInvalidateArrayAliases()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety0);
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety1);
#endif
        }

        /// <summary>
        /// Array-like indexing operator.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.indexoperator"/>
        /// </example>
        /// <param name="index">The zero-based index.</param>
        public T this [int index]
        {
            get
            {
                CheckReadAccess();
                CheckBounds(index);
                return UnsafeUtility.ReadArrayElement<T>(BufferHeader.GetElementPointer(m_Buffer), index);
            }
            set
            {
                CheckWriteAccess();
                CheckBounds(index);
                UnsafeUtility.WriteArrayElement<T>(BufferHeader.GetElementPointer(m_Buffer), index, value);
            }
        }

        /// <summary>
        /// Increases the buffer capacity and length.
        /// </summary>
        /// <remarks>If <paramref name="length"/> is less than the current
        /// length of the buffer, the length of the buffer is reduced while the
        /// capacity remains unchanged.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.resizeuninitialized"/>
        /// </example>
        /// <param name="length">The new length of the buffer.</param>
        public void ResizeUninitialized(int length)
        {
            Reserve(length);
            m_Buffer->Length = length;
        }

        /// <summary>
        /// Increases the buffer capacity without increasing its length.
        /// </summary>
        /// <remarks>If <paramref name="length"/> is greater than the current <see cref="Capacity"/>
        /// of this buffer and greater than the capacity reserved with
        /// <see cref="InternalBufferCapacityAttribute"/>, this function allocates a new memory block
        /// and copies the current buffer to it. The number of elements in the buffer remains
        /// unchanged.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.reserve"/>
        /// </example>
        /// <param name="length">The new buffer capacity.</param>
        public void Reserve(int length)
        {
            CheckWriteAccessAndInvalidateArrayAliases();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            BufferHeader.EnsureCapacity(m_Buffer, length, UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), BufferHeader.TrashMode.RetainOldData, m_useMemoryInitPattern, m_memoryInitPattern);
#else
            BufferHeader.EnsureCapacity(m_Buffer, length, UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), BufferHeader.TrashMode.RetainOldData, false, 0);
#endif
        }

        /// <summary>
        /// Sets the buffer length to zero.
        /// </summary>
        /// <remarks>The capacity of the buffer remains unchanged. Buffer memory
        /// is not overwritten.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.clear"/>
        /// </example>
        public void Clear()
        {
            CheckWriteAccessAndInvalidateArrayAliases();

            m_Buffer->Length = 0;
        }

        /// <summary>
        /// Removes any excess capacity in the buffer.
        /// </summary>
        /// <remarks>Sets the buffer capacity to the current length.
        /// If the buffer memory size changes, the current contents
        /// of the buffer are copied to a new block of memory and the
        /// old memory is freed. If the buffer now fits in the space in the
        /// chunk reserved with <see cref="InternalBufferCapacityAttribute"/>,
        /// then the buffer contents are moved to the chunk.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.trimexcess"/>
        /// </example>
        public void TrimExcess()
        {
            CheckWriteAccessAndInvalidateArrayAliases();

            byte* oldPtr = m_Buffer->Pointer;
            int length = m_Buffer->Length;

            if (length == Capacity || oldPtr == null)
                return;

            int elemSize = UnsafeUtility.SizeOf<T>();
            int elemAlign = UnsafeUtility.AlignOf<T>();

            bool isInternal;
            byte* newPtr;

            // If the size fits in the internal buffer, prefer to move the elements back there.
            if (length <= m_InternalCapacity)
            {
                newPtr = (byte*) (m_Buffer + 1);
                isInternal = true;
            }
            else
            {
                newPtr = (byte*) UnsafeUtility.Malloc((long) elemSize * length, elemAlign, Allocator.Persistent);
                isInternal = false;
            }

            UnsafeUtility.MemCpy(newPtr, oldPtr, (long)elemSize * length);

            m_Buffer->Capacity = Math.Max(length, m_InternalCapacity);
            m_Buffer->Pointer = isInternal ? null : newPtr;

            UnsafeUtility.Free(oldPtr, Allocator.Persistent);
        }

        /// <summary>
        /// Adds an element to the end of the buffer, resizing as necessary.
        /// </summary>
        /// <remarks>The buffer is resized if it has no additional capacity.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.add"/>
        /// </example>
        /// <param name="elem">The element to add to the buffer.</param>
        /// <returns>The new length of the buffer.</returns>
        public int Add(T elem)
        {
            CheckWriteAccess();
            int length = Length;
            ResizeUninitialized(length + 1);
            this[length] = elem;
            return length;
        }

        /// <summary>
        /// Inserts an element at the specified index, resizing as necessary.
        /// </summary>
        /// <remarks>The buffer is resized if it has no additional capacity.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.insert"/>
        /// </example>
        /// <param name="index">The position at which to insert the new element.</param>
        /// <param name="elem">The element to add to the buffer.</param>
        public void Insert(int index, T elem)
        {
            CheckWriteAccess();
            int length = Length;
            ResizeUninitialized(length + 1);
            CheckBounds(index); //CheckBounds after ResizeUninitialized since index == length is allowed
            int elemSize = UnsafeUtility.SizeOf<T>();
            byte* basePtr = BufferHeader.GetElementPointer(m_Buffer);
            UnsafeUtility.MemMove(basePtr + (index + 1) * elemSize, basePtr + index * elemSize, (long)elemSize * (length - index));
            this[index] = elem;
        }

        /// <summary>
        /// Adds all the elements from <paramref name="newElems"/> to the end
        /// of the buffer, resizing as necessary.
        /// </summary>
        /// <remarks>The buffer is resized if it has no additional capacity.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.addrange"/>
        /// </example>
        /// <param name="newElems">The native array of elements to insert.</param>
        public void AddRange(NativeArray<T> newElems)
        {
            CheckWriteAccess();
            int elemSize = UnsafeUtility.SizeOf<T>();
            int oldLength = Length;
            ResizeUninitialized(oldLength + newElems.Length);

            byte* basePtr = BufferHeader.GetElementPointer(m_Buffer);
            UnsafeUtility.MemCpy(basePtr + (long)oldLength * elemSize, newElems.GetUnsafeReadOnlyPtr<T>(), (long)elemSize * newElems.Length);
        }

        /// <summary>
        /// Removes the specified number of elements, starting with the element at the specified index.
        /// </summary>
        /// <remarks>The buffer capacity remains unchanged.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.removerange"/>
        /// </example>
        /// <param name="index">The first element to remove.</param>
        /// <param name="count">How many elements tot remove.</param>
        public void RemoveRange(int index, int count)
        {
            CheckWriteAccess();
            CheckBounds(index + count - 1);

            int elemSize = UnsafeUtility.SizeOf<T>();
            byte* basePtr = BufferHeader.GetElementPointer(m_Buffer);

            UnsafeUtility.MemMove(basePtr + index * elemSize, basePtr + (index + count) * elemSize, (long)elemSize * (Length - count - index));

            m_Buffer->Length -= count;
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.removeat"/>
        /// </example>
        /// <param name="index">The index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            RemoveRange(index, 1);
        }

        /// <summary>
        /// Gets an <see langword="unsafe"/> pointer to the contents of the buffer.
        /// </summary>
        /// <remarks>This function can only be called in unsafe code contexts.</remarks>
        /// <returns>A typed, unsafe pointer to the first element in the buffer.</returns>
        public void* GetUnsafePtr()
        {
            CheckWriteAccess();
            return BufferHeader.GetElementPointer(m_Buffer);
        }

        /// <summary>
        /// Returns a dynamic buffer of a different type, pointing to the same buffer memory.
        /// </summary>
        /// <remarks>No memory modification occurs. The reinterpreted type must be the same size
        /// in memory as the original type.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.reinterpret"/>
        /// </example>
        /// <typeparam name="U">The reinterpreted type.</typeparam>
        /// <returns>A dynamic buffer of the reinterpreted type.</returns>
        /// <exception cref="InvalidOperationException">If the reinterpreted type is a different
        /// size than the original.</exception>
        public DynamicBuffer<U> Reinterpret<U>() where U: struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (UnsafeUtility.SizeOf<U>() != UnsafeUtility.SizeOf<T>())
                throw new InvalidOperationException($"Types {typeof(U)} and {typeof(T)} are of different sizes; cannot reinterpret");
#endif
            // NOTE: We're forwarding the internal capacity along to this aliased, type-punned buffer.
            // That's OK, because if mutating operations happen they are all still the same size.
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new DynamicBuffer<U>(m_Buffer, m_Safety0, m_Safety1, m_IsReadOnly, m_useMemoryInitPattern, m_memoryInitPattern, m_InternalCapacity);
#else
            return new DynamicBuffer<U>(m_Buffer, m_InternalCapacity);
#endif
        }

        /// <summary>
        /// Return a native array that aliases the original buffer contents.
        /// </summary>
        /// <remarks>You can only access the native array as long as the
        /// the buffer memory has not been reallocated. Several dynamic buffer operations,
        /// such as <see cref="Add"/> and <see cref="TrimExcess"/> can result in
        /// buffer reallocation.</remarks>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.asnativearray"/>
        /// </example>
        public NativeArray<T> AsNativeArray()
        {
            CheckReadAccess();

            var shadow = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(BufferHeader.GetElementPointer(m_Buffer), Length, Allocator.Invalid);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var handle = m_Safety1;
            AtomicSafetyHandle.UseSecondaryVersion(ref handle);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref shadow, handle);

#endif
            return shadow;
        }

        /// <summary>
        /// Provides an enumerator for iterating over the buffer elements.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.getenumerator"/>
        /// </example>
        /// <returns>The enumerator.</returns>
        public NativeArray<T>.Enumerator GetEnumerator()
        {
            var array = AsNativeArray();
            return new NativeArray<T>.Enumerator(ref array);
        }

        IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() { throw new NotImplementedException(); }

        /// <summary>
        /// Copies the buffer into a new native array.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.tonativearray"/>
        /// </example>
        /// <param name="allocator">The type of memory allocation to use when creating the
        /// native array.</param>
        /// <returns>A native array containing copies of the buffer elements.</returns>
        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            return new NativeArray<T>(AsNativeArray(), allocator);
        }

        /// <summary>
        /// Copies all the elements from the specified native array into this dynamic buffer.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.copyfrom.nativearray"/>
        /// </example>
        /// <param name="v">The native array containing the elements to copy.</param>
        public void CopyFrom(NativeArray<T> v)
        {
            ResizeUninitialized(v.Length);
            AsNativeArray().CopyFrom(v);
        }

        /// <summary>
        /// Copies all the elements from another dynamic buffer.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.copyfrom.dynamicbuffer"/>
        /// </example>
        /// <param name="v">The dynamic buffer containing the elements to copy.</param>
        public void CopyFrom(DynamicBuffer<T> v)
        {
            ResizeUninitialized(v.Length);

            v.CheckReadAccess();
            CheckWriteAccess();

            UnsafeUtility.MemCpy(BufferHeader.GetElementPointer(m_Buffer),
                BufferHeader.GetElementPointer(v.m_Buffer), Length * UnsafeUtility.SizeOf<T>());
        }

        /// <summary>
        /// Copies all the elements from an array.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeSamples.Tests/DynamicBufferExamples.cs" language="csharp" region="dynamicbuffer.copyfrom.array"/>
        /// </example>
        /// <param name="v">A C# array containing the elements to copy.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void CopyFrom(T[] v)
        {
            if(v == null)
                throw new ArgumentNullException(nameof(v));

#if NET_DOTS
            Clear();
            foreach (var d in v)
            {
                Add(d);
            }
#else
            ResizeUninitialized(v.Length);
            CheckWriteAccess();

            GCHandle gcHandle = GCHandle.Alloc((object) v, GCHandleType.Pinned);
            IntPtr num = gcHandle.AddrOfPinnedObject();

            UnsafeUtility.MemCpy(BufferHeader.GetElementPointer(m_Buffer),
                (void*)num, Length * UnsafeUtility.SizeOf<T>());
            gcHandle.Free();
#endif
        }
    }

    internal sealed class DynamicBufferDebugView<T>  where T : struct
    {
#if !NET_DOTS
        private DynamicBuffer<T> _buffer;
        public DynamicBufferDebugView(DynamicBuffer<T> source)
        {
            _buffer = source;
        }

        public T[] Items => _buffer.AsNativeArray().ToArray();
#endif
    }
}
