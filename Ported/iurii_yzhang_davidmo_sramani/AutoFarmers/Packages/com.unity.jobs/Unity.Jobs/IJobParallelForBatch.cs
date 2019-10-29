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
#if !UNITY_DOTSPLAYER || !UNITY_SINGLETHREADED_JOBS
            static IntPtr JobReflectionData;

            public static unsafe IntPtr Initialize()
            {
                if (JobReflectionData == IntPtr.Zero)
                {
#if UNITY_DOTSPLAYER
                    JobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), JobType.ParallelFor,
                        (JobsUtility.ManagedJobForEachDelegate)Execute,
                        (JobsUtility.ManagedJobDelegate)Cleanup);

#else
                    JobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), JobType.ParallelFor, (ExecuteJobFunction)Execute);
#endif
                }
                return JobReflectionData;
            }
#endif

#if UNITY_DOTSPLAYER && !UNITY_SINGLETHREADED_JOBS
            public T JobData;
            public JobRanges Ranges;

            static unsafe void Execute(void* structPtr, int jobIndex)
            {
                var jobStruct = UnsafeUtility.AsRef<ParallelForBatchJobStruct<T>>(structPtr);
                var ranges = jobStruct.Ranges;
                var jobData = jobStruct.JobData;
                jobData.SetupReflection_Gen(jobIndex);

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

            static unsafe void Cleanup(void* structPtr)
            {
                var jobStruct = UnsafeUtility.AsRef<ParallelForBatchJobStruct<T>>(structPtr);
                var jobData = jobStruct.JobData;
                DoDeallocateOnJobCompletion(jobData);
                UnsafeUtility.Free(structPtr, Allocator.TempJob);
            }
#elif !UNITY_DOTSPLAYER
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
        static void DoDeallocateOnJobCompletion(object jobData)
        {
            throw new CodegenShouldReplaceException();
        }

        public static void SetupReflection_Gen<T>(this T job, int jobIndex) where T : struct, IJobParallelForBatch
        {
            throw new CodegenShouldReplaceException();
        }
#endif

        public static unsafe JobHandle ScheduleBatch<T>(this T jobData, int arrayLength, int minIndicesPerJobCount, JobHandle dependsOn = new JobHandle()) where T : struct, IJobParallelForBatch
        {
#if UNITY_SINGLETHREADED_JOBS
            jobData.Execute(0, arrayLength);
            DoDeallocateOnJobCompletion(jobData);
            return new JobHandle();
#elif UNITY_DOTSPLAYER
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

            var scheduleParams = new JobsUtility.JobScheduleParameters(jobDataPtr, ParallelForBatchJobStruct<T>.Initialize(),
                dependsOn, ScheduleMode.Batched);
            return JobsUtility.ScheduleParallelFor(ref scheduleParams, arrayLength, minIndicesPerJobCount);
#else
            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), ParallelForBatchJobStruct<T>.Initialize(), dependsOn, ScheduleMode.Batched);
            return JobsUtility.ScheduleParallelFor(ref scheduleParams, arrayLength, minIndicesPerJobCount);
#endif
        }

        public static unsafe void RunBatch<T>(this T jobData, int arrayLength) where T : struct, IJobParallelForBatch
        {
#if UNITY_DOTSPLAYER
            ScheduleBatch(jobData, arrayLength, arrayLength).Complete();
#else
            var scheduleParams = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf(ref jobData), ParallelForBatchJobStruct<T>.Initialize(), new JobHandle(), ScheduleMode.Run);
            JobsUtility.ScheduleParallelFor(ref scheduleParams, arrayLength, arrayLength);
#endif
        }
    }
}
