using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

//-----------------------------------------------------------------------
// <copyright file="NativeChunkedList.cs" company="Jackson Dunstan">
//     Copyright (c) Jackson Dunstan. See LICENSE.txt.
// </copyright>
//-----------------------------------------------------------------------
namespace AntPheromones_ECS
{
    /// <summary>
    /// A two-dimensional array stored in native memory
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// Type of elements in the array
    /// </typeparam>
    [DebuggerDisplay("Length0 = {Length0}, Length1 = {Length1}")]
    [DebuggerTypeProxy(typeof(NativeArray2DDebugView<>))]
    [NativeContainer]
    [NativeContainerSupportsDeallocateOnJobCompletion]
    public unsafe struct NativeArray2D<T>
        : IDisposable
        , IEnumerable<T>
        , IEquatable<NativeArray2D<T>>
#if CSHARP_7_3_OR_NEWER
        where T : unmanaged
#else
    	where T : struct
#endif
    {
        /// <summary>
        /// An enumerator for this type of array. It enumerates from (0,0) to
        /// (Length0-1,Length1-1) in rows of the first dimension then the second
        /// dimension. For example, an array with Length0=2 and Length1=3 is
        /// enumerated as follows:
        ///   (0, 0)
        ///   (1, 0)
        ///   (0, 1)
        ///   (1, 1)
        ///   (0, 2)
        ///   (1, 3)
        /// </summary>
        [ExcludeFromDocs]
        public struct Enumerator : IEnumerator<T>
        {
            /// <summary>
            /// Array to enumerate
            /// </summary>
            private NativeArray2D<T> m_Array;
            
            /// <summary>
            /// Current index in the first dimension
            /// </summary>
            private int m_Index0;
            
            /// <summary>
            /// Current index in the second dimension
            /// </summary>
            private int m_Index1;

            /// <summary>
            /// Create the enumerator. It's initially just before the first
            /// element of both dimensions.
            /// </summary>
            /// 
            /// <param name="array">
            /// Array to enumerate
            /// </param>
            public Enumerator(ref NativeArray2D<T> array)
            {
                m_Array = array;
                m_Index0 = -1;
                m_Index1 = 0;
            }

            /// <summary>
            /// Dispose of the enumerator. This is a no-op.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Move to the next element of the array. This moves along the
            /// first dimension until the end is hit, at which time the first
            /// dimension index is reset to zero and the second dimension index
            /// is incremented.
            /// </summary>
            /// 
            /// <returns>
            /// If the new indices are within the bounds of the array.
            /// </returns>
            public bool MoveNext()
            {
                m_Index0++;
                if (m_Index0 >= m_Array.Length0)
                {
                    m_Index0 = 0;
                    m_Index1++;
                    return m_Index1 < m_Array.Length1;
                }
                return true;
            }

            /// <summary>
            /// Reset to just before the first element in both dimensions
            /// </summary>
            public void Reset()
            {
                m_Index0 = -1;
                m_Index1 = 0;
            }

            /// <summary>
            /// Get the currently-enumerated element
            /// </summary>
            public T Current
            {
                get
                {
                    return m_Array[m_Index0, m_Index1];
                }
            }

            /// <summary>
            /// Get the currently-enumerated element
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
        }
        
        /// <summary>
        /// Pointer to the memory the array is stored in.
        /// </summary>
        [NativeDisableUnsafePtrRestriction]
        private void* m_Buffer;

        /// <summary>
        /// Length of the array's first dimension
        /// </summary>
        private int m_Length0;

        /// <summary>
        /// Length of the array's second dimension
        /// </summary>
        private int m_Length1;
        
        // These fields are all required when safety checks are enabled
        // They must have these exact types, names, and order
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        /// <summary>
        /// A handle to information about what operations can be safely
        /// performed on the list at any given time.
        /// </summary>
        private AtomicSafetyHandle m_Safety;

        /// <summary>
        /// A handle that can be used to tell if the list has been disposed yet
        /// or not, which allows for error-checking double disposal.
        /// </summary>
        [NativeSetClassTypeToNullOnSchedule]
        private DisposeSentinel m_DisposeSentinel;
#endif
        
        /// <summary>
        /// Allocator used to create <see cref="m_Buffer"/>.
        /// </summary>
        internal Allocator m_Allocator;

        /// <summary>
        /// Create the array and optionally clear it
        /// </summary>
        /// 
        /// <param name="length0">
        /// Length of the array's first dimension. Must be positive.
        /// </param>
        /// 
        /// <param name="length1">
        /// Length of the array's second dimension. Must be positive.
        /// </param>
        /// 
        /// <param name="allocator">
        /// Allocator to allocate native memory with. Must be valid as defined
        /// by <see cref="UnsafeUtility.IsValidAllocator"/>.
        /// </param>
        /// 
        /// <param name="options">
        /// Whether the array should be cleared or not
        /// </param>
        public NativeArray2D(
            int length0,
            int length1,
            Allocator allocator,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory)
        {
            Allocate(length0, length1, allocator, out this);
            if ((options & NativeArrayOptions.ClearMemory)
                == NativeArrayOptions.ClearMemory)
            {
                UnsafeUtility.MemClear(
                    m_Buffer,
                    Length * (long)UnsafeUtility.SizeOf<T>());
            }
        }

        /// <summary>
        /// Create a copy of the given managed array
        /// </summary>
        /// 
        /// <param name="array">
        /// Managed array to copy. Must not be null.
        /// </param>
        /// 
        /// <param name="allocator">
        /// Allocator to allocate native memory with. Must be valid as defined
        /// by <see cref="UnsafeUtility.IsValidAllocator"/>.
        /// </param>
        public NativeArray2D(T[,] array, Allocator allocator)
        {
            int length0 = array.GetLength(0);
            int length1 = array.GetLength(1);
            Allocate(length0, length1, allocator, out this);
            Copy(array, this);
        }

        /// <summary>
        /// Create a copy of the given native array
        /// </summary>
        /// 
        /// <param name="array">
        /// Native array to copy
        /// </param>
        /// 
        /// <param name="allocator">
        /// Allocator to allocate native memory with. Must be valid as defined
        /// by <see cref="UnsafeUtility.IsValidAllocator"/>.
        /// </param>
        public NativeArray2D(NativeArray2D<T> array, Allocator allocator)
        {
            Allocate(array.Length0, array.Length1, allocator, out this);
            Copy(array, this);
        }

        /// <summary>
        /// Get the total number of elements in the array
        /// </summary>
        public int Length
        {
            get
            {
                return m_Length0 * m_Length1;
            }
        }
        
        /// <summary>
        /// Get the length of the array's first dimension
        /// </summary>
        public int Length0
        {
            get
            {
                return m_Length0;
            }
        }
        
        /// <summary>
        /// Get the length of the array's second dimension
        /// </summary>
        public int Length1
        {
            get
            {
                return m_Length1;
            }
        }
        
        /// <summary>
        /// Throw an exception if the array isn't readable
        /// </summary>
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void RequireReadAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
        }

        /// <summary>
        /// Throw an exception if the list isn't writable
        /// </summary>
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void RequireWriteAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
        }
        
        /// <summary>
        /// Throw an exception if an index is out of bounds
        /// </summary>
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void RequireIndexInBounds(int index0, int index1)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index0 < 0 || index0 >= m_Length0)
            {
                throw new IndexOutOfRangeException();
            }
            if (index1 < 0 || index1 >= m_Length1)
            {
                throw new IndexOutOfRangeException();
            }
#endif
        }

        /// <summary>
        /// Throw an exception when the given allocator is invalid as defined
        /// by <see cref="UnsafeUtility.IsValidAllocator"/>.
        /// </summary>
        /// 
        /// <param name="allocator">
        /// Allocator to check.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the given allocator is invalid.
        /// </exception>
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void RequireValidAllocator(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!UnsafeUtility.IsValidAllocator(allocator))
            {
                throw new InvalidOperationException(
                    "The NativeArray2D can not be Disposed because it was " +
                    "not allocated with a valid allocator.");
            }
#endif
        }

        /// <summary>
        /// Index into the array to read or write an element
        /// </summary>
        /// 
        /// <param name="index0">
        /// Index of the first dimension
        /// </param>
        /// 
        /// <param name="index1">
        /// Index of the second dimension
        /// </param>
        public T this[int index0, int index1]
        {
            get
            {
                RequireReadAccess();
                RequireIndexInBounds(index0, index1);
                
                int index = index1 * m_Length0 + index0;
                return UnsafeUtility.ReadArrayElement<T>(m_Buffer, index);
            }
            
            [WriteAccessRequired]
            set
            {
                RequireWriteAccess();
                RequireIndexInBounds(index0, index1);

                int index = index1 * m_Length0 + index0;
                UnsafeUtility.WriteArrayElement(m_Buffer, index, value);
            }
        }

        /// <summary>
        /// Check if the underlying unmanaged memory has been created and not
        /// freed via a call to <see cref="Dispose"/>.
        /// 
        /// This operation has no access requirements.
        ///
        /// This operation is O(1).
        /// </summary>
        /// 
        /// <value>
        /// Initially true when a non-default constructor is called but
        /// initially false when the default constructor is used. After
        /// <see cref="Dispose"/> is called, this becomes false. Note that
        /// calling <see cref="Dispose"/> on one copy of this object doesn't
        /// result in this becoming false for all copies if it was true before.
        /// This property should <i>not</i> be used to check whether the object
        /// is usable, only to check whether it was <i>ever</i> usable.
        /// </value>
        public bool IsCreated
        {
            get
            {
                return (IntPtr)m_Buffer != IntPtr.Zero;
            }
        }
        
        /// <summary>
        /// Release the object's unmanaged memory. Do not use it after this. Do
        /// not call <see cref="Dispose"/> on copies of the object either.
        /// 
        /// This operation requires write access.
        /// 
        /// This complexity of this operation is O(1) plus the allocator's
        /// deallocation complexity.
        /// </summary>
        [WriteAccessRequired]
        public void Dispose()
        {
            RequireWriteAccess();
            RequireValidAllocator(m_Allocator);

// Make sure we're not double-disposing
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#if UNITY_2018_3_OR_NEWER
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#else
    		DisposeSentinel.Dispose(m_Safety, ref m_DisposeSentinel);
#endif
#endif

            UnsafeUtility.Free(m_Buffer, m_Allocator);
            m_Buffer = null;
            m_Length0 = 0;
            m_Length1 = 0;
        }

        /// <summary>
        /// Copy the elements of a managed array to this array
        /// </summary>
        /// 
        /// <param name="array">
        /// Array to copy from. Must not be null. Must have the same dimensions
        /// as this array.
        /// </param>
        [WriteAccessRequired]
        public void CopyFrom(T[,] array)
        {
            Copy(array, this);
        }

        /// <summary>
        /// Copy the elements of a native array to this array
        /// </summary>
        /// 
        /// <param name="array">
        /// Array to copy from. Must have the same dimensions as this array.
        /// </param>
        [WriteAccessRequired]
        public void CopyFrom(NativeArray2D<T> array)
        {
            Copy(array, this);
        }

        /// <summary>
        /// Copy the elements of this array to a managed array
        /// </summary>
        /// 
        /// <param name="array">
        /// Array to copy to. Must not be null. Must have the same dimensions
        /// as this array.
        /// </param>
        public void CopyTo(T[,] array)
        {
            Copy(this, array);
        }

        /// <summary>
        /// Copy the elements of this array to a native array
        /// </summary>
        /// 
        /// <param name="array">
        /// Array to copy to. Must have the same dimensions
        /// as this array.
        /// </param>
        public void CopyTo(NativeArray2D<T> array)
        {
            Copy(this, array);
        }

        /// <summary>
        /// Copy the elements of this array to a newly-created managed array
        /// </summary>
        /// 
        /// <returns>
        /// A newly-created managed array with the elements of this array.
        /// </returns>
        public T[,] ToArray()
        {
            T[,] dst = new T[m_Length0, m_Length1];
            Copy(this, dst);
            return dst;
        }

        /// <summary>
        /// Get an enumerator for this array
        /// </summary>
        /// 
        /// <returns>
        /// An enumerator for this array.
        /// </returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        /// <summary>
        /// Get an enumerator for this array
        /// </summary>
        /// 
        /// <returns>
        /// An enumerator for this array.
        /// </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        /// <summary>
        /// Get an enumerator for this array
        /// </summary>
        /// 
        /// <returns>
        /// An enumerator for this array.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Check if this array points to the same native memory as another
        /// array. 
        /// </summary>
        /// 
        /// <param name="other">
        /// Array to check against.
        /// </param>
        /// 
        /// <returns>
        /// If this array points to the same native memory as the given array.
        /// </returns>
        public bool Equals(NativeArray2D<T> other)
        {
            return m_Buffer == other.m_Buffer
                   && m_Length0 == other.m_Length0
                   && m_Length1 == other.m_Length1;
        }

        /// <summary>
        /// Check if this array points to the same native memory as another
        /// array. 
        /// </summary>
        /// 
        /// <param name="other">
        /// Array to check against.
        /// </param>
        /// 
        /// <returns>
        /// If this array points to the same native memory as the given array.
        /// </returns>
        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return other is NativeArray2D<T> && Equals((NativeArray2D<T>)other);
        }

        /// <summary>
        /// Get a hash code for this array
        /// </summary>
        /// 
        /// <returns>
        /// A hash code for this array
        /// </returns>
        public override int GetHashCode()
        {
            int result = (int)m_Buffer;
            result = (result * 397) ^ m_Length0;
            result = (result * 397) ^ m_Length1;
            return result;
        }

        /// <summary>
        /// Check if two arrays point to the same native memory.
        /// </summary>
        /// 
        /// <param name="a">
        /// First array to check
        /// </param>
        ///
        /// <param name="b">
        /// Second array to check
        /// </param>
        /// 
        /// <returns>
        /// If the given arrays point to the same native memory.
        /// </returns>
        public static bool operator ==(NativeArray2D<T> a, NativeArray2D<T> b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Check if two arrays don't point to the same native memory.
        /// </summary>
        /// 
        /// <param name="a">
        /// First array to check
        /// </param>
        ///
        /// <param name="b">
        /// Second array to check
        /// </param>
        /// 
        /// <returns>
        /// If the given arrays don't point to the same native memory.
        /// </returns>
        public static bool operator !=(NativeArray2D<T> a, NativeArray2D<T> b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Allocate memory for the array
        /// </summary>
        /// 
        /// <param name="length0">
        /// Length of the array's first dimension. Must be positive.
        /// </param>
        /// 
        /// <param name="length1">
        /// Length of the array's second dimension. Must be positive.
        /// </param>
        /// 
        /// <param name="allocator">
        /// Allocator to allocate native memory with. Must be valid as defined
        /// by <see cref="UnsafeUtility.IsValidAllocator"/>.
        /// </param>
        /// 
        /// <param name="array">
        /// Array to write to once allocated
        /// </param>
        private static void Allocate(
            int length0,
            int length1,
            Allocator allocator,
            out NativeArray2D<T> array)
        {
            RequireValidAllocator(allocator);
            
#if !CSHARP_7_3_OR_NEWER
            if (!UnsafeUtility.IsUnmanaged<T>())
            {
                throw new InvalidOperationException(
                    "Only unmanaged types are supported");
            }
#endif
            
            int length = length0 * length1;
            if (length <= 0)
            {
                throw new InvalidOperationException(
                    "Total number of elements must be greater than zero");
            }
            
            array = new NativeArray2D<T>
            {
                m_Buffer = UnsafeUtility.Malloc(
                    length * UnsafeUtility.SizeOf<T>(),
                    UnsafeUtility.AlignOf<T>(),
                    allocator),
                m_Length0 = length0,
                m_Length1 = length1,
                m_Allocator = allocator
            };
            DisposeSentinel.Create(
                out array.m_Safety,
                out array.m_DisposeSentinel,
                1,
                allocator);
        }

        /// <summary>
        /// Copy a native array's elements to another native array
        /// </summary>
        /// 
        /// <param name="src">
        /// Array to copy from
        /// </param>
        /// 
        /// <param name="dest">
        /// Array to copy to
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If the arrays have different sizes
        /// </exception>
        private static void Copy(NativeArray2D<T> src, NativeArray2D<T> dest)
        {
            src.RequireReadAccess();
            dest.RequireWriteAccess();
            
            if (src.Length0 != dest.Length0
                || src.Length1 != dest.Length1)
            {
                throw new ArgumentException("Arrays must have the same size");
            }
            
            for (int index0 = 0; index0 < src.Length0; ++index0)
            {
                for (int index1 = 0; index1 < src.Length1; ++index1)
                {
                    dest[index0, index1] = src[index0, index1];
                }
            }
        }

        /// <summary>
        /// Copy a managed array's elements to a native array
        /// </summary>
        /// 
        /// <param name="src">
        /// Array to copy from
        /// </param>
        /// 
        /// <param name="dest">
        /// Array to copy to
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If the arrays have different sizes
        /// </exception>
        private static void Copy(T[,] src, NativeArray2D<T> dest)
        {
            dest.RequireWriteAccess();
            
            if (src.GetLength(0) != dest.Length0
                || src.GetLength(1) != dest.Length1)
            {
                throw new ArgumentException("Arrays must have the same size");
            }
            
            for (int index0 = 0; index0 < dest.Length0; ++index0)
            {
                for (int index1 = 0; index1 < dest.Length1; ++index1)
                {
                    dest[index0, index1] = src[index0, index1];
                }
            }
        }

        /// <summary>
        /// Copy a native array's elements to a managed array
        /// </summary>
        /// 
        /// <param name="src">
        /// Array to copy from
        /// </param>
        /// 
        /// <param name="dest">
        /// Array to copy to
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If the arrays have different sizes
        /// </exception>
        private static void Copy(NativeArray2D<T> src, T[,] dest)
        {
            src.RequireReadAccess();
            
            if (src.Length0 != dest.GetLength(0)
                || src.Length1 != dest.GetLength(1))
            {
                throw new ArgumentException("Arrays must have the same size");
            }

            for (int index0 = 0; index0 < src.Length0; ++index0)
            {
                for (int index1 = 0; index1 < src.Length1; ++index1)
                {
                    dest[index0, index1] = src[index0, index1];
                }
            }
        }
    }
    
    /// <summary>
    /// A debugger view of the array type
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// Type of elements in the array
    /// </typeparam>
    internal sealed class NativeArray2DDebugView<T>
#if CSHARP_7_3_OR_NEWER
        where T : unmanaged
#else
    	where T : struct
#endif
    {
        /// <summary>
        /// The array to view
        /// </summary>
        private readonly NativeArray2D<T> m_Array;

        /// <summary>
        /// Create the view
        /// </summary>
        /// 
        /// <param name="array">
        /// The array to view
        /// </param>
        public NativeArray2DDebugView(NativeArray2D<T> array)
        {
            m_Array = array;
        }

        /// <summary>
        /// Get the elements of the array as a managed array
        /// </summary>
        public T[,] Items => m_Array.ToArray();
    }
}