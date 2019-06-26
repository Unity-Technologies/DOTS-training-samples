using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Jobs
{
    [JobProducerType(typeof(IJobParallelForBatchExtensions.ParallelForBatchJobStruct<>))]
    public interface IJobParallelForBatch
    {
        void Execute(int startIndex, int count);
    }

    public static class IJobParallelForBatchExtensions
    {
        internal struct ParallelForBatchJobStruct<T> where T : struct, IJobParallelForBatch
        {
#if UNITY_AVOID_REFLECTION
            public static JobsUtility.ManagedJobForEachDelegate ExecuteDelegate;
            public static GCHandle ExecuteHandle;
            public static IntPtr ExecuteFunctionPtr;

            public static JobsUtility.ManagedJobDelegate CleanupDelegate;
            public static GCHandle CleanupHandle;
            public static IntPtr CleanupFunctionPtr;

            public T JobData;
            public JobRanges Ranges;

            public static unsafe void Execute(void* structPtr, int jobIndex)
            {
                var jobStruct = UnsafeUtility.AsRef<ParallelForBatchJobStruct<T>>(structPtr);
                var ranges = jobStruct.Ranges;
                var jobData = jobStruct.JobData;

                while (true)
                {
                    if (!JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out int begin, out int end))
                        break;

                    #if ENABLE_UNITY_COLLECTIONS_CHECKS
                    JobsUtility.PatchBufferMinMaxRanges(IntPtr.Zero, UnsafeUtility.AddressOf(ref jobData), begin, end - begin);
                    #endif

                    jobData.Execute(begin, end - begin);
                    break;
                }
            }

            public static unsafe void Cleanup(void* structPtr)
            {
                var jobStruct = UnsafeUtility.AsRef<ParallelForBatchJobStruct<T>>(structPtr);
                var jobData = jobStruct.JobData;
                DoDeallocateOnJobCompletion(jobData);
                UnsafeUtility.Free(structPtr, Allocator.TempJob);
            }
#else
            static public IntPtr                            jobReflectionData;

            public static IntPtr Initialize()
            {
                if (jobReflectionData == IntPtr.Zero)
                    jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), JobType.ParallelFor, (ExecuteJobFunction)Execute);
                return jobReflectionData;
            }
            public delegate void ExecuteJobFunction(ref T data, System.IntPtr additionalPtr, System.IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);
            public unsafe static void Execute(ref T jobData, System.IntPtr additionalPtr, System.IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
            {
                while (true)
                {
                    int begin;
                    int end;

                    if (!JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out begin, out end))
                        return;

                    #if ENABLE_UNITY_COLLECTIONS_CHECKS
                    JobsUtility.PatchBufferMinMaxRanges(bufferRangePatchData, UnsafeUtility.AddressOf(ref jobData), begin, end - begin);
                    #endif

                    jobData.Execute(begin, end - begin);
                }
            }
#endif
        }

#if UNITY_DOTSPLAYER
        static internal void DoDeallocateOnJobCompletion(object jobData)
        {
            throw new CodegenShouldReplaceException();
        }
#endif

        public static unsafe JobHandle ScheduleBatch<T>(this T jobData, int arrayLength, int minIndicesPerJobCount, JobHandle dependsOn = new JobHandle()) where T : struct, IJobParallelForBatch
        {
#if UNITY_AVOID_REFLECTION
            // Protect against garbage collection
            if (!ParallelForBatchJobStruct<T>.ExecuteHandle.IsAllocated)
            {
                ParallelForBatchJobStruct<T>.ExecuteDelegate = ParallelForBatchJobStruct<T>.Execute;
                ParallelForBatchJobStruct<T>.ExecuteHandle = GCHandle.Alloc(ParallelForBatchJobStruct<T>.ExecuteDelegate);
                ParallelForBatchJobStruct<T>.ExecuteFunctionPtr = Marshal.GetFunctionPointerForDelegate(ParallelForBatchJobStruct<T>.ExecuteDelegate);
            }

            // Protect against garbage collection
            if (!ParallelForBatchJobStruct<T>.CleanupHandle.IsAllocated)
            {
                ParallelForBatchJobStruct<T>.CleanupDelegate = ParallelForBatchJobStruct<T>.Cleanup;
                ParallelForBatchJobStruct<T>.CleanupHandle = GCHandle.Alloc(ParallelForBatchJobStruct<T>.CleanupDelegate);
                ParallelForBatchJobStruct<T>.CleanupFunctionPtr = Marshal.GetFunctionPointerForDelegate(ParallelForBatchJobStruct<T>.CleanupDelegate);
            }

            var jobFunctionPtr = ParallelForBatchJobStruct<T>.ExecuteFunctionPtr;
            var completionFuncPtr = ParallelForBatchJobStruct<T>.CleanupFunctionPtr;

            var jobStruct = new ParallelForBatchJobStruct<T>()
            {
                JobData = jobData,
                Ranges = new JobRanges()
                {
                    ArrayLength = arrayLength,
                    IndicesPerPhase = JobsUtility.GetDefaultIndicesPerPhase(arrayLength)
                },
            };

            var jobDataPtr = UnsafeUtility.Malloc(UnsafeUtility.SizeOf<ParallelForBatchJobStruct<T>>(),
                UnsafeUtility.AlignOf<ParallelForBatchJobStruct<T>>(), Allocator.TempJob);
            UnsafeUtility.CopyStructureToPtr(ref jobStruct, jobDataPtr);

            return JobsUtility.ScheduleJobForEach(jobFunctionPtr, completionFuncPtr, new IntPtr(jobDataPtr),
                arrayLength, minIndicesPerJobCount, dependsOn);
#else
            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), ParallelForBatchJobStruct<T>.Initialize(), dependsOn, ScheduleMode.Batched);
            return JobsUtility.ScheduleParallelFor(ref scheduleParams, arrayLength, minIndicesPerJobCount);
#endif
        }

        public static unsafe void RunBatch<T>(this T jobData, int arrayLength) where T : struct, IJobParallelForBatch
        {
#if UNITY_AVOID_REFLECTION
            ScheduleBatch(jobData, arrayLength, arrayLength).Complete();
#else
            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), ParallelForBatchJobStruct<T>.Initialize(), new JobHandle(), ScheduleMode.Run);
            JobsUtility.ScheduleParallelFor(ref scheduleParams, arrayLength, arrayLength);
#endif
        }
    }
}
