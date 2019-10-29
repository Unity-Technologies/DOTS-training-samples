using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
    /// <summary>
    /// An unmanaged, untyped, resizable list, without any thread safety check features.
    /// </summary>
    public unsafe struct UnsafeList
    {
        public void* Ptr;
        public int Length;
        public int Capacity;
        public Allocator Allocator;

        /// <summary>
        /// Constructs a new list using the specified type of memory allocation.
        /// </summary>
        /// <param name="allocator">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        /// <remarks>The list initially has a capacity of one. To avoid reallocating memory for the list, specify
        /// sufficient capacity up front.</remarks>
        public unsafe UnsafeList(Allocator allocator)
        {
            Ptr = null;
            Length = 0;
            Capacity = 0;
            Allocator = allocator;
        }

        /// <summary>
        /// Constructs list as view into memory.
        /// </summary>
        public unsafe UnsafeList(void* ptr, int length)
        {
            Ptr = ptr;
            Length = length;
            Capacity = length;
            Allocator = Allocator.Invalid;
        }

        /// <summary>
        /// Constructs a new list with the specified initial capacity and type of memory allocation.
        /// </summary>
        /// <param name="sizeOf">Size of element.</param>
        /// <param name="alignOf">Alignment of element.</param>
        /// <param name="initialCapacity">The initial capacity of the list. If the list grows larger than its capacity,
        /// the internal array is copied to a new, larger array.</param>
        /// <param name="allocator">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        /// <param name="options">Memory should be cleared on allocation or left uninitialized.</param>
        public unsafe UnsafeList(int sizeOf, int alignOf, int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            Allocator = allocator;
            Ptr = null;
            Length = 0;
            Capacity = 0;

            if (initialCapacity != 0)
            {
                SetCapacity(sizeOf, alignOf, initialCapacity);
            }

            if (options == NativeArrayOptions.ClearMemory
            && Ptr != null)
            {
                UnsafeUtility.MemClear(Ptr, Capacity * sizeOf);
            }
        }

        /// <summary>
        /// Creates a new list with the specified initial capacity and type of memory allocation.
        /// </summary>
        /// <param name="sizeOf">Size of element.</param>
        /// <param name="alignOf">Alignment of element.</param>
        /// <param name="initialCapacity">The initial capacity of the list. If the list grows larger than its capacity,
        /// the internal array is copied to a new, larger array.</param>
        /// <param name="allocator">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        /// <param name="options">Memory should be cleared on allocation or left uninitialized.</param>
        public static UnsafeList* Create(int sizeOf, int alignOf, int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            UnsafeList* listData = (UnsafeList*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<UnsafeList>(), UnsafeUtility.AlignOf<UnsafeList>(), allocator);
            UnsafeUtility.MemClear(listData, UnsafeUtility.SizeOf<UnsafeList>());

            listData->Allocator = allocator;

            if (initialCapacity != 0)
            {
                listData->SetCapacity(sizeOf, alignOf, initialCapacity);
            }

            if (options == NativeArrayOptions.ClearMemory
            && listData->Ptr != null)
            {
                UnsafeUtility.MemClear(listData->Ptr, listData->Capacity * sizeOf);
            }

            return listData;
        }

        /// <summary>
        /// Destroys list.
        /// </summary>
        public static void Destroy(UnsafeList* listData)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (listData == null)
            {
                throw new Exception("UnsafeList has yet to be created or has been destroyed!");
            }
#endif
            var allocator = listData->Allocator;
            listData->Dispose();
            UnsafeUtility.Free(listData, allocator);
        }

        /// <summary>
        /// Disposes of this container and deallocates its memory immediately.
        /// </summary>
        public void Dispose()
        {
            if (Allocator != Allocator.Invalid)
            {
                UnsafeUtility.Free(Ptr, Allocator);
                Allocator = Allocator.Invalid;
            }

            Ptr = null;
            Length = 0;
            Capacity = 0;
        }

        /// <summary>
        /// Safely disposes of this container and deallocates its memory when the jobs that use it have completed.
        /// </summary>
        /// <remarks>You can call this function dispose of the container immediately after scheduling the job. Pass
        /// the [JobHandle](https://docs.unity3d.com/ScriptReference/Unity.Jobs.JobHandle.html) returned by
        /// the [Job.Schedule](https://docs.unity3d.com/ScriptReference/Unity.Jobs.IJobExtensions.Schedule.html)
        /// method using the `jobHandle` parameter so the job scheduler can dispose the container after all jobs
        /// using it have run.</remarks>
        /// <param name="jobHandle">The job handle or handles for any scheduled jobs that use this container.</param>
        /// <returns>A new job handle containing the prior handles as well as the handle for the job that deletes
        /// the container.</returns>
        public JobHandle Dispose(JobHandle inputDeps)
        {
            if (Allocator != Allocator.Invalid)
            {
                var jobHandle = new DisposeJob { Ptr = Ptr, Allocator = Allocator }.Schedule(inputDeps);

                Ptr = null;
                Length = 0;
                Capacity = 0;
                Allocator = Allocator.Invalid;

                return jobHandle;
            }

            Ptr = null;
            Length = 0;
            Capacity = 0;

            return default;
        }

        [BurstCompile]
        struct DisposeJob : IJob
        {
            [NativeDisableUnsafePtrRestriction]
            public void* Ptr;
            public Allocator Allocator;

            public void Execute()
            {
                UnsafeUtility.Free(Ptr, Allocator);
            }
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        /// <remarks>List Capacity remains unchanged.</remarks>
        public void Clear()
        {
            Length = 0;
        }

        /// <summary>
        /// Changes the list length, resizing if necessary.
        /// </summary>
        /// <param name="sizeOf">Size of element.</param>
        /// <param name="alignOf">Alignment of element.</param>
        /// <param name="length">The new length of the list.</param>
        /// <param name="options">Memory should be cleared on allocation or left uninitialized.</param>
        public void Resize(int sizeOf, int alignOf, int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            var oldLength = Length;

            SetCapacity(sizeOf, alignOf, length);
            Length = length;

            if (options == NativeArrayOptions.ClearMemory
            && oldLength < length)
            {
                var num = length - oldLength;
                byte* ptr = (byte*)Ptr;
                UnsafeUtility.MemClear(ptr + oldLength * sizeOf, num * sizeOf);
            }
        }

        /// <summary>
        /// Changes the list length, resizing if necessary.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="length">The new length of the list.</param>
        /// <param name="options">Memory should be cleared on allocation or left uninitialized.</param>
        public void Resize<T>(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where T : struct
        {
            Resize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), length, options);
        }

        /// <summary>
        /// Set the number of items that can fit in the list.
        /// </summary>
        /// <param name="sizeOf">Size of element.</param>
        /// <param name="alignOf">Alignment of element.</param>
        /// <param name="capacity">The number of items that the list can hold before it resizes its internal storage.</param>
        void SetCapacity(int sizeOf, int alignOf, int capacity)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (Allocator == Allocator.Invalid)
            {
                throw new Exception("UnsafeList is not initialized, it must be initialized with allocator before use.");
            }
#endif
            if (capacity > 0)
            {
                var itemsPerCacheLine = 64 / sizeOf;

                if (capacity < itemsPerCacheLine)
                    capacity = itemsPerCacheLine;

                capacity = CollectionHelper.CeilPow2(capacity);
            }

            var newCapacity = capacity;
            if (newCapacity == Capacity)
                return;

            void* newPointer = null;
            if (newCapacity > 0)
            {
                var bytesToMalloc = sizeOf * newCapacity;
                newPointer = UnsafeUtility.Malloc(bytesToMalloc, alignOf, Allocator);

                if (Capacity > 0)
                {
                    var itemsToCopy = newCapacity < Capacity ? newCapacity : Capacity;
                    var bytesToCopy = itemsToCopy * sizeOf;
                    UnsafeUtility.MemCpy(newPointer, Ptr, bytesToCopy);
                }
            }

            if (Capacity > 0)
                UnsafeUtility.Free(Ptr, Allocator);

            Ptr = newPointer;
            Capacity = newCapacity;
            Length = Math.Min(Length, Capacity);
        }

        /// <summary>
        /// Set the number of items that can fit in the list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="capacity">The number of items that the list can hold before it resizes its internal storage.</param>
        public void SetCapacity<T>(int capacity) where T : struct
        {
            SetCapacity(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), capacity);
        }

        /// <summary>
        /// Searches for the specified element in list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="value"></param>
        /// <returns>The zero-based index of the first occurrence element if found, otherwise returns -1.</returns>
        public int IndexOf<T>(T value) where T : struct, IEquatable<T>
        {
            return NativeArrayExtensions.IndexOf<T, T>(Ptr, Length, value);
        }

        /// <summary>
        /// Determines whether an element is in the list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="value"></param>
        /// <returns>True, if element is found.</returns>
        public bool Contains<T>(T value) where T : struct, IEquatable<T>
        {
            return IndexOf(value) != -1;
        }

        /// <summary>
        /// Adds an element to the list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="value">The value to be added at the end of the list.</param>
        /// <remarks>
        /// If the list has reached its current capacity, internal array won't be resized, and exception will be thrown.
        /// </remarks>
        public void AddNoResize<T>(T value) where T : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (Capacity < Length + 1)
            {
                throw new Exception($"AddNoResize assumes that list capacity is sufficient (Capacity {Capacity}, Lenght {Length})!");
            }
#endif
            UnsafeUtility.WriteArrayElement(Ptr, Length, value);
            Length += 1;
        }

        private void AddRangeNoResize(int sizeOf, int alignOf, void* ptr, int length)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (Capacity < Length + length)
            {
                throw new Exception($"AddRangeNoResize assumes that list capacity is sufficient (Capacity {Capacity}, Lenght {Length})!");
            }
#endif
            void* dst = (byte*)Ptr + Length * sizeOf;
            UnsafeUtility.MemCpy(dst, ptr, length * sizeOf);
            Length += length;
        }

        /// <summary>
        /// Adds elements from a buffer to this list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="ptr">A pointer to the buffer.</param>
        /// <param name="length">The number of elements to add to the list.</param>
        /// <remarks>
        /// If the list has reached its current capacity, internal array won't be resized, and exception will be thrown.
        /// </remarks>
        public void AddRangeNoResize<T>(void* ptr, int length) where T : struct
        {
            AddRangeNoResize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), ptr, length);
        }

        /// <summary>
        /// Adds elements from a list to this list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <remarks>
        /// If the list has reached its current capacity, internal array won't be resized, and exception will be thrown.
        /// </remarks>
        public void AddRangeNoResize<T>(UnsafeList list) where T : struct
        {
            AddRangeNoResize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), list.Ptr, list.Length);
        }

        /// <summary>
        /// Adds an element to the list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="value">The value to be added at the end of the list.</param>
        /// <remarks>
        /// If the list has reached its current capacity, it copies the original, internal array to
        /// a new, larger array, and then deallocates the original.
        /// </remarks>
        public void Add<T>(T value) where T : struct
        {
            Resize<T>(Length + 1);
            UnsafeUtility.WriteArrayElement(Ptr, Length - 1, value);
        }

        private void AddRange(int sizeOf, int alignOf, void* ptr, int length)
        {
            int oldLength = Length;
            Resize(sizeOf, alignOf, oldLength + length);
            void* dst = (byte*)Ptr + oldLength * sizeOf;
            UnsafeUtility.MemCpy(dst, ptr, length * sizeOf);
        }

        /// <summary>
        /// Adds elements from a buffer to this list.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="ptr">A pointer to the buffer.</param>
        /// <param name="length">The number of elements to add to the list.</param>
        public void AddRange<T>(void* ptr, int length) where T : struct
        {
            AddRange(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), ptr, length);
        }

        /// <summary>
        /// Adds elements from a list to this list.
        /// </summary>
        /// <remarks>
        /// If the list has reached its current capacity, it copies the original, internal array to
        /// a new, larger array, and then deallocates the original.
        /// </remarks>
        /// <typeparam name="T">Source type of elements</typeparam>
        public void AddRange<T>(UnsafeList list) where T : struct
        {
            AddRange(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), list.Ptr, list.Length);
        }

        /// <summary>
        /// Truncates the list by replacing the item at the specified index with the last item in the list. The list
        /// is shortened by one.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="index">The index of the item to delete.</param>
        public void RemoveAtSwapBack<T>(int index) where T : struct
        {
            RemoveRangeSwapBack<T>(index, index + 1);
        }

        private void RemoveRangeSwapBack(int sizeOf, int begin, int end)
        {
            int itemsToRemove = end - begin;
            if (itemsToRemove > 0)
            {
                int copyFrom = Math.Max(Length - itemsToRemove, end);
                void* dst = (byte*)Ptr + begin * sizeOf;
                void* src = (byte*)Ptr + copyFrom * sizeOf;
                UnsafeUtility.MemCpy(dst, src, Math.Min(itemsToRemove, Length - copyFrom) * sizeOf);
                Length -= itemsToRemove;
            }
        }

        /// <summary>
        /// Truncates the list by replacing the item at the specified index range with the items from the end the list. The list
        /// is shortened by number of elements in range.
        /// </summary>
        /// <typeparam name="T">Source type of elements</typeparam>
        /// <param name="begin">The first index of the item to delete.</param>
        /// <param name="end">The last index of the item to delete.</param>
        public void RemoveRangeSwapBack<T>(int begin, int end) where T : struct
        {
            RemoveRangeSwapBack(UnsafeUtility.SizeOf<T>(), begin, end);
        }

        /// <summary>
        /// Returns parallel reader instance.
        /// </summary>
        public ParallelReader AsParallelReader()
        {
            return new ParallelReader(Ptr, Length);
        }

        /// <summary>
        /// Implements parallel reader. Use AsParallelReader to obtain it from container.
        /// </summary>
        public unsafe struct ParallelReader
        {
            public readonly void* Ptr;
            public readonly int Length;

            public ParallelReader(void* ptr, int length)
            {
                Ptr = ptr;
                Length = length;
            }

            public int IndexOf<T>(T value) where T : struct, IEquatable<T>
            {
                return NativeArrayExtensions.IndexOf<T, T>(Ptr, Length, value);
            }

            public bool Contains<T>(T value) where T : struct, IEquatable<T>
            {
                return IndexOf(value) != -1;
            }
        }
    }

    /// <summary>
    /// An unmanaged, resizable list, without any thread safety check features.
    /// </summary>
    [DebuggerTypeProxy(typeof(UnsafePtrListDebugView))]
    public unsafe struct UnsafePtrList
    {
        public readonly void** Ptr;
        public readonly int Length;
        public readonly int Capacity;
        public readonly Allocator Allocator;

        private ref UnsafeList ListData { get { return ref *(UnsafeList*)UnsafeUtility.AddressOf(ref this); } }

        /// <summary>
        /// Constructs list as view into memory.
        /// </summary>
        public unsafe UnsafePtrList(void** ptr, int length)
        {
            Ptr = ptr;
            Length = length;
            Capacity = length;
            Allocator = Allocator.Invalid;
        }

        /// <summary>
        /// Constructs a new list using the specified type of memory allocation.
        /// </summary>
        /// <param name="allocator">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        /// <param name="options">Memory should be cleared on allocation or left uninitialized.</param>
        /// <remarks>The list initially has a capacity of one. To avoid reallocating memory for the list, specify
        /// sufficient capacity up front.</remarks>
        public unsafe UnsafePtrList(int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            Ptr = null;
            Length = 0;
            Capacity = 0;
            Allocator = Allocator.Invalid;

            var sizeOf = IntPtr.Size;
            ListData = new UnsafeList(sizeOf, sizeOf, initialCapacity, allocator, options);
        }

        /// <summary>
        /// </summary>
        public static UnsafePtrList* Create(void** ptr, int length)
        {
            UnsafePtrList* listData = (UnsafePtrList*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<UnsafePtrList>(), UnsafeUtility.AlignOf<UnsafePtrList>(), Allocator.Persistent);
            *listData = new UnsafePtrList(ptr, length);
            return listData;
        }

        /// <summary>
        /// Creates a new list with the specified initial capacity and type of memory allocation.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list. If the list grows larger than its capacity,
        /// the internal array is copied to a new, larger array.</param>
        /// <param name="allocator">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        /// <param name="options">Memory should be cleared on allocation or left uninitialized.</param>
        public static UnsafePtrList* Create(int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            UnsafePtrList* listData = (UnsafePtrList*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<UnsafePtrList>(), UnsafeUtility.AlignOf<UnsafePtrList>(), allocator);
            *listData = new UnsafePtrList(initialCapacity, allocator, options);
            return listData;
        }

        /// <summary>
        /// Destroys list.
        /// </summary>
        public static void Destroy(UnsafePtrList* listData)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (listData == null)
            {
                throw new Exception("UnsafeList has yet to be created or has been destroyed!");
            }
#endif
            var allocator = listData->ListData.Allocator == Allocator.Invalid ? Allocator.Persistent : listData->ListData.Allocator;
            listData->Dispose();
            UnsafeUtility.Free(listData, allocator);
        }

        /// <summary>
        /// Disposes of this container and deallocates its memory immediately.
        /// </summary>
        public void Dispose()
        {
            ListData.Dispose();
        }

        /// <summary>
        /// Safely disposes of this container and deallocates its memory when the jobs that use it have completed.
        /// </summary>
        /// <remarks>You can call this function dispose of the container immediately after scheduling the job. Pass
        /// the [JobHandle](https://docs.unity3d.com/ScriptReference/Unity.Jobs.JobHandle.html) returned by
        /// the [Job.Schedule](https://docs.unity3d.com/ScriptReference/Unity.Jobs.IJobExtensions.Schedule.html)
        /// method using the `jobHandle` parameter so the job scheduler can dispose the container after all jobs
        /// using it have run.</remarks>
        /// <param name="jobHandle">The job handle or handles for any scheduled jobs that use this container.</param>
        /// <returns>A new job handle containing the prior handles as well as the handle for the job that deletes
        /// the container.</returns>
        public JobHandle Dispose(JobHandle inputDeps)
        {
            return ListData.Dispose(inputDeps);
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        /// <remarks>List Capacity remains unchanged.</remarks>
        public void Clear()
        {
            ListData.Clear();
        }

        /// <summary>
        /// Changes the list length, resizing if necessary.
        /// </summary>
        /// <param name="length">The new length of the list.</param>
        /// <param name="options">Memory should be cleared on allocation or left uninitialized.</param>
        public void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            ListData.Resize<IntPtr>(length, options);
        }

        /// <summary>
        /// Set the number of items that can fit in the list.
        /// </summary>
        /// <param name="capacity">The number of items that the list can hold before it resizes its internal storage.</param>
        public void SetCapacity(int capacity)
        {
            ListData.SetCapacity<IntPtr>(capacity);
        }

        /// <summary>
        /// Searches for the specified element in list.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The zero-based index of the first occurrence element if found, otherwise returns -1.</returns>
        public int IndexOf(void* value)
        {
            for (int i = 0; i < Length; ++i)
            {
                if (Ptr[i] == value) return i;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether an element is in the list.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True, if element is found.</returns>
        public bool Contains(void* value)
        {
            return IndexOf(value) != -1;
        }

        /// <summary>
        /// Adds an element to the list.
        /// </summary>
        /// <param name="value">The struct to be added at the end of the list.</param>
        public void Add(void* value)
        {
            Resize(Length + 1);
            Ptr[Length - 1] = value;
        }

        /// <summary>
        /// Adds the elements of a UnsafePtrList to this list.
        /// </summary>
        /// <param name="list">The items to add.</param>
        public void AddRange(UnsafePtrList list)
        {
            ListData.AddRange<IntPtr>(list.ListData);
        }

        /// <summary>
        /// Truncates the list by replacing the item at the specified index with the last item in the list. The list
        /// is shortened by one.
        /// </summary>
        /// <param name="index">The index of the item to delete.</param>
        public void RemoveAtSwapBack(int index)
        {
            RemoveRangeSwapBack(index, index + 1);
        }

        /// <summary>
        /// Truncates the list by replacing the item at the specified index range with the items from the end the list. The list
        /// is shortened by number of elements in range.
        /// </summary>
        /// <param name="begin">The first index of the item to delete.</param>
        /// <param name="end">The last index of the item to delete.</param>
        public void RemoveRangeSwapBack(int begin, int end)
        {
            ListData.RemoveRangeSwapBack<IntPtr>(begin, end);
        }

        /// <summary>
        /// Returns parallel reader instance.
        /// </summary>
        public ParallelReader AsParallelReader()
        {
            return new ParallelReader(Ptr, Length);
        }

        /// <summary>
        /// Implements parallel reader. Use AsParallelReader to obtain it from container.
        /// </summary>
        public unsafe struct ParallelReader
        {
            public readonly void** Ptr;
            public readonly int Length;

            public ParallelReader(void** ptr, int length)
            {
                Ptr = ptr;
                Length = length;
            }

            public int IndexOf(void* value)
            {
                for (int i = 0; i < Length; ++i)
                {
                    if (Ptr[i] == value) return i;
                }

                return -1;
            }

            public bool Contains(void* value)
            {
                return IndexOf(value) != -1;
            }
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
                IntPtr[] result = new IntPtr[m_UnsafePtrList.Length];

                for (var i = 0; i < result.Length; ++i)
                {
                    result[i] = (IntPtr)m_UnsafePtrList.Ptr[i];
                }

                return result;
            }
        }
    }
}
