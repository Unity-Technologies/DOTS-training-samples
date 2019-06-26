using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using System.Diagnostics;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
    /// <summary>
    /// An unmanaged, resizable list.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    [StructLayout (LayoutKind.Sequential)]
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}")]
	[DebuggerTypeProxy(typeof(NativeListDebugView < >))]
	public unsafe struct NativeList<T> : IDisposable
        where T : struct
	{

#if ENABLE_UNITY_COLLECTIONS_CHECKS
	    internal AtomicSafetyHandle m_Safety;
        [NativeSetClassTypeToNullOnSchedule]
        DisposeSentinel m_DisposeSentinel;
#endif
        [NativeDisableUnsafePtrRestriction]
        internal UnsafeList* m_ListData;
        private Allocator m_Allocator;


        /// <summary>
        /// Constructs a new list using the specified type of memory allocation.
        /// </summary>
        /// <param name="i_label">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        /// <remarks>The list initially has a capacity of one. To avoid reallocating memory for the list, specify
        /// sufficient capacity up front.</remarks>
        public unsafe NativeList(Allocator i_label) : this (1, i_label, 2) { }
	    /// <summary>
	    /// Constructs a new list with the specified initial capacity and type of memory allocation.
	    /// </summary>
	    /// <param name="capacity">The initial capacity of the list. If the list grows larger than its capacity,
	    /// the internal array is copied to a new, larger array.</param>
	    /// <param name="i_label">A member of the
	    /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
	    public unsafe NativeList(int capacity, Allocator i_label) : this (capacity, i_label, 2) { }

	    unsafe NativeList(int capacity, Allocator i_label, int stackDepth)
        {
            var totalSize = UnsafeUtility.SizeOf<T>() * (long)capacity;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            // Native allocation is only valid for Temp, Job and Persistent.
            if (i_label <= Allocator.None)
                throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(i_label));
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be >= 0");

            CollectionHelper.CheckIsUnmanaged<T>();

            // Make sure we cannot allocate more than int.MaxValue (2,147,483,647 bytes)
            // because the underlying UnsafeUtility.Malloc is expecting a int.
            // TODO: change UnsafeUtility.Malloc to accept a UIntPtr length instead to match C++ API
            if (totalSize > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(capacity), $"Capacity * sizeof(T) cannot exceed {int.MaxValue} bytes");
#endif
            m_Allocator = i_label;
            m_ListData = (UnsafeList*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<UnsafeList>(), UnsafeUtility.AlignOf<UnsafeList>(), m_Allocator);
            UnsafeUtility.MemClear(m_ListData,UnsafeUtility.SizeOf<UnsafeList>());
            m_ListData->SetCapacity<T>(capacity, m_Allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, stackDepth, m_Allocator);
#endif
	    }

	    /// <summary>
	    /// Retrieve a member of the list by index.
	    /// </summary>
	    /// <param name="index">The zero-based index into the list.</param>
	    /// <value>The list item at the specified index.</value>
	    /// <exception cref="IndexOutOfRangeException">Thrown if index is negative or >= to <see cref="Length"/>.</exception>
	    public T this [int index]
		{
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
                if ((uint)index >= (uint)m_ListData->Length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range in NativeList of '{m_ListData->Length}' Length.");
#endif
                return UnsafeUtility.ReadArrayElement<T>(m_ListData->Ptr, index);
            }
	        set
	        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
	            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
                if ((uint)index >= (uint)m_ListData->Length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range in NativeList of '{m_ListData->Length}' Length.");
#endif
                UnsafeUtility.WriteArrayElement(m_ListData->Ptr, index, value);
	        }
		}

	    /// <summary>
	    /// The current number of items in the list.
	    /// </summary>
	    /// <value>The item count.</value>
	    public int Length
	    {
	        get
	        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
	            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return m_ListData->Length;
	        }
	    }

	    /// <summary>
	    /// The number of items that can fit in the list.
	    /// </summary>
	    /// <value>The number of items that the list can hold before it resizes its internal storage.</value>
	    /// <remarks>Capacity specifies the number of items the list can currently hold. You can change Capacity
	    /// to fit more or fewer items. Changing Capacity creates a new array of the specified size, copies the
	    /// old array to the new one, and then deallocates the original array memory. You cannot change the Capacity
	    /// to a size smaller than <see cref="Length"/> (remove unwanted elements from the list first).</remarks>
	    /// <exception cref="ArgumentException">Thrown if Capacity is set smaller than Length.</exception>
	    public int Capacity
	    {
	        get
	        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
	            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return m_ListData->Capacity;
	        }

	        set
	        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
	            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
                if (value < m_ListData->Length)
                    throw new ArgumentException("Capacity must be larger than the length of the NativeList.");
#endif
                m_ListData->SetCapacity<T>(value, m_Allocator);
            }
	    }

		/// <summary>
		/// Adds an element to the list.
		/// </summary>
		/// <param name="element">The struct to be added at the end of the list.</param>
		/// <remarks>If the list has reached its current capacity, it copies the original, internal array to
		/// a new, larger array, and then deallocates the original.
		/// </remarks>
		public void Add(T element)
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
		    AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            m_ListData->Add(element, m_Allocator);
        }

        /// <summary>
        /// Adds the elements of a NativeArray to this list.
        /// </summary>
        /// <param name="elements">The items to add.</param>
        public void AddRange(NativeArray<T> elements)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            AddRange(elements.GetUnsafeReadOnlyPtr(), elements.Length);
        }

        /// <summary>
        /// Adds elements from a buffer to this list.
        /// </summary>
        /// <param name="elements">A pointer to the buffer.</param>
        /// <param name="count">The number of elements to add to the list.</param>
        public unsafe void AddRange(void* elements, int count)
        {
            m_ListData->AddRange<T>(elements, count, m_Allocator);
        }

        /// <summary>
		/// Truncates the list by replacing the item at the specified index with the last item in the list. The list
		/// is shortened by one.
		/// </summary>
		/// <param name="index">The index of the item to delete.</param>
		/// <exception cref="ArgumentOutOfRangeException">If index is negative or >= <see cref="Length"/>.</exception>
		public void RemoveAtSwapBack(int index)
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
		    AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);

            if( index < 0 || index >= Length )
                throw new ArgumentOutOfRangeException(index.ToString());
#endif
            m_ListData->RemoveAtSwapBack<T>(index);
        }

		/// <summary>
		/// Reports whether memory for the list is allocated.
		/// </summary>
		/// <value>True if this list object's internal storage  has been allocated.</value>
		/// <remarks>Note that the list storage is not created if you use the default constructor. You must specify
		/// at least an allocation type to construct a usable NativeList.</remarks>
		public bool IsCreated => m_ListData != null;

	    /// <summary>
	    /// Disposes of this list and deallocates its memory immediately.
	    /// </summary>
	    public void Dispose()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            NativeListUnsafeUtility.DeallocateList(m_ListData, m_Allocator);
            m_ListData = null;
        }

        /// <summary>
        /// Safely disposes of this list and deallocates its memory when the jobs that use it have completed.
        /// </summary>
        /// <remarks>You can call this function dispose of the list immediately after scheduling the job. Pass
        /// the [JobHandle](https://docs.unity3d.com/ScriptReference/Unity.Jobs.JobHandle.html) returned by
        /// the [Job.Schedule](https://docs.unity3d.com/ScriptReference/Unity.Jobs.IJobExtensions.Schedule.html)
        /// method using the `jobHandle` parameter so the job scheduler can dispose the list after all jobs
        /// using it have run.</remarks>
        /// <param name="jobHandle">The job handle or handles for any scheduled jobs that use this list.</param>
        /// <returns>A new job handle containing the prior handles as well as the handle for the job that deletes
        /// the list.</returns>
        public JobHandle Dispose(JobHandle jobHandle)
        {
            // [DeallocateOnJobCompletion] is not supported, but we want the deallocation to happen in a thread.
            // DisposeSentinel needs to be cleared on main thread.
            // AtomicSafetyHandle can be destroyed after the job was scheduled (Job scheduling will check that no jobs are writing to the container)
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Clear(ref m_DisposeSentinel);
#endif
            var jobData = new DisposeListJob { List = this };
            jobHandle = jobData.Schedule(jobHandle);
            m_ListData = null;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif

            return jobHandle;
        }
        
        [BurstCompile]
        struct DisposeListJob : IJob
        {
            public NativeList<T> List;
            public void Execute()
            {
                NativeListUnsafeUtility.DeallocateList(List.m_ListData, List.m_Allocator);
            }
        }

		/// <summary>
		/// Clears the list.
		/// </summary>
		/// <remarks>List <see cref="Capacity"/> remains unchanged.</remarks>
		public void Clear()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
		    AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            m_ListData->Length = 0;
        }

	    /// <summary>
	    /// This list as a [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html).
	    /// </summary>
	    /// <remarks>The array is not a copy; it references the same memory as the original list.</remarks>
	    /// <param name="nativeList">A NativeList instance.</param>
	    /// <returns>A NativeArray containing all the items in the list.</returns>
	    public static implicit operator NativeArray<T> (NativeList<T> nativeList)
	    {
	        return nativeList.AsArray();
	    }

	    /// <summary>
	    /// This list as a [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html).
	    /// </summary>
	    /// <remarks>The array is not a copy; it references the same memory as the original list. You can use the
	    /// NativeArray API to manipulate the list.</remarks>
	    /// <returns>A NativeArray "view" of the list.</returns>
	    public NativeArray<T> AsArray()
	    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
	        AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(m_Safety);
	        var arraySafety = m_Safety;
	        AtomicSafetyHandle.UseSecondaryVersion(ref arraySafety);
#endif

	        var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T> (m_ListData->Ptr, m_ListData->Length, Collections.Allocator.Invalid);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
	        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, arraySafety);
#endif
	        return array;
	    }


	    [Obsolete("Please use AsDeferredJobArray")]
	    public NativeArray<T> ToDeferredJobArray()
	    {
	        return AsDeferredJobArray();
	    }

	    /// <summary>
	    /// Provides a NativeArray that you can pass into a job whose contents can be modified by a previous job.
	    /// </summary>
	    /// <remarks>Pass a deferred array to a job when the list is populated or modified by a previous job. Using a
	    /// deferred array allows you to schedule both jobs at the same time. (Without a deferred array, you would
	    /// have to wait for the results of the first job before you scheduling the second.)</remarks>
	    /// <returns>A [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html) that
	    /// can be passed to one job as a "promise" that is fulfilled by a previous job.</returns>
	    /// <example>
	    /// The following example populates a list with integers in one job and passes that data to a second job as
	    /// a deferred array. If you tried to pass the list directly to the second job, that job would get the contents
	    /// of the list at the time you schedule the job and would not see any modifications made to the list by the
	    /// first job.
	    /// <code>
	    /// using UnityEngine;
	    /// using Unity.Jobs;
	    /// using Unity.Collections;
        ///
	    /// public class DeferredArraySum : MonoBehaviour
	    ///{
	    ///    public struct ListPopulatorJob : IJob
	    ///    {
	    ///        public NativeList&lt;int&gt; list;
        ///
	    ///        public void Execute()
	    ///        {
	    ///            for (int i = list.Length; i &lt; list.Capacity; i++)
	    ///            {
	    ///                list.Add(i);
	    ///            }
	    ///        }
	    ///    }
        ///
	    ///    public struct ArraySummerJob : IJob
	    ///    {
	    ///        [ReadOnly] public NativeArray&lt;int&gt; deferredArray;
	    ///        public NativeArray&lt;int&gt; sum;
        ///
	    ///        public void Execute()
	    ///        {
	    ///            sum[0] = 0;
	    ///            for (int i = 0; i &lt; deferredArray.Length; i++)
	    ///            {
	    ///                sum[0] += deferredArray[i];
	    ///            }
	    ///        }
	    ///    }
        ///
	    ///    void Start()
	    ///    {
	    ///        var deferredList = new NativeList&lt;int&gt;(100, Allocator.TempJob);
        ///
	    ///        var populateJob = new ListPopulatorJob()
	    ///        {
	    ///            list = deferredList
	    ///        };
        ///
	    ///        var output = new NativeArray&lt;int&gt;(1, Allocator.TempJob);
	    ///        var sumJob = new ArraySummerJob()
	    ///        {
	    ///            deferredArray = deferredList.AsDeferredJobArray(),
	    ///            sum = output
	    ///        };
        ///
	    ///        var populateJobHandle = populateJob.Schedule();
	    ///        var sumJobHandle = sumJob.Schedule(populateJobHandle);
        ///
	    ///        sumJobHandle.Complete();
        ///
	    ///        Debug.Log("Result: " + output[0]);
        ///
	    ///        deferredList.Dispose();
	    ///        output.Dispose();
	    ///    }
	    /// }
	    /// </code>
	    /// </example>
	    public unsafe NativeArray<T> AsDeferredJobArray()
	    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
	        AtomicSafetyHandle.CheckExistsAndThrow(m_Safety);
#endif

	        byte* buffer = (byte*)m_ListData;
	        // We use the first bit of the pointer to infer that the array is in list mode
	        // Thus the job scheduling code will need to patch it.
	        buffer += 1;
	        var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T> (buffer, 0, Allocator.Invalid);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
	        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, m_Safety);
#endif

	        return array;
	    }


		/// <summary>
		/// A copy of this list as a [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html).
		/// </summary>
		/// <returns>A NativeArray containing copies of all the items in the list.</returns>
		public T[] ToArray()
		{
		    return AsArray().ToArray();
		}

		/// <summary>
		/// A copy of this list as a [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html)
		/// allocated with the specified type of memory.
		/// </summary>
		/// <param name="allocator">A member of the
		/// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
		/// <returns>A NativeArray containing copies of all the items in the list.</returns>
		public NativeArray<T> ToArray(Allocator allocator)
		{
		    NativeArray<T> result = new NativeArray<T>(Length, allocator, NativeArrayOptions.UninitializedMemory);
		    result.CopyFrom(this);
		    return result;
		}

		/// <summary>
		/// Overwrites this list with the elements of an array.
		/// </summary>
		/// <remarks>The array to be copied must have a length equal to or greater than the current list.</remarks>
		/// <param name="array">A managed array or
		/// [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html) to copy
		/// into this list.</param>
		public void CopyFrom(T[] array)
		{
		    //@TODO: Thats not right... This doesn't perform a resize
		    Capacity = array.Length;
		    NativeArray<T> nativeArray = this;
		    nativeArray.CopyFrom(array);
		}

		/// <summary>
		/// Changes the list length, resizing if necessary, without initializing memory.
		/// </summary>
		/// <param name="length">The new length of the list.</param>
		public void ResizeUninitialized(int length)
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
		    AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            Capacity = Math.Max(length, Capacity);
            m_ListData->Length = length;
		}
    }


    sealed class NativeListDebugView<T> where T : struct
    {
        NativeList<T> m_Array;

        public NativeListDebugView(NativeList<T> array)
        {
            m_Array = array;
        }

        public T[] Items => m_Array.ToArray();
    }
}
namespace Unity.Collections.LowLevel.Unsafe
{
    /// <summary>
    /// Utilities for unsafe access to a <see cref="NativeList{T}"/>.
    /// </summary>
    public unsafe static class NativeListUnsafeUtility
    {
        internal static void DeallocateList(UnsafeList* data, Allocator allocator)
        {
            if (data != null)
            {
                data->Dispose(allocator);
                UnsafeUtility.Free(data,allocator);
            }
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            else
                throw new Exception("NativeList has yet to be allocated or has been dealocated!");
#endif            
        }
        
        /// <summary>
        /// Gets a pointer to the memory buffer containing the list items.
        /// </summary>
        /// <param name="nativeList">The NativeList containing the buffer.</param>
        /// <typeparam name="T">The type of list element.</typeparam>
        /// <returns>A pointer to the memory buffer.</returns>
        public static void* GetUnsafePtr<T>(this NativeList<T> nativeList) where T : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(nativeList.m_Safety);
#endif
            var data = nativeList.m_ListData;
            return data->Ptr;
        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        /// <summary>
        /// Gets the [AtomicSafetyHandle](https://docs.unity3d.com/ScriptReference/Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle.html)
        /// used by the C# Job system to validate safe access to the list.
        /// </summary>
        /// <param name="nativeList">The NativeList.</param>
        /// <typeparam name="T">The type of list element.</typeparam>
        /// <returns>The atomic safety handle for the list.</returns>
        /// <remarks>The symbol, `ENABLE_UNITY_COLLECTIONS_CHECKS` must be defined for this function to be available.</remarks>
        public static AtomicSafetyHandle GetAtomicSafetyHandle<T>(ref NativeList<T> nativeList) where T : struct
        {
            return nativeList.m_Safety;
        }
#endif

        /// <summary>
        /// Gets a pointer to the internal list data (without checking for safe access).
        /// </summary>
        /// <param name="nativeList">The NativeList.</param>
        /// <typeparam name="T">The type of list element.</typeparam>
        /// <returns>A pointer to the list data.</returns>
        public static void* GetInternalListDataPtrUnchecked<T>(ref NativeList<T> nativeList) where T : struct
        {
            return nativeList.m_ListData;
        }
    }
}
