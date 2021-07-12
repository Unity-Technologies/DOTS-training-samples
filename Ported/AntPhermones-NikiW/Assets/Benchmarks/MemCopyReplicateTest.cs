using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;


public class MemCopyReplicateTest : MonoBehaviour
{
    [SerializeField]
    private int m_max = 999;
    [SerializeField]
    private int m_stride = 8;
    [SerializeField]
    private int m_innerloopBatchCount = 16;

    [SerializeField]
    private bool m_assert;

    private const float ResetValue = -1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var arr = new NativeArray<float>(m_max, Allocator.TempJob, NativeArrayOptions.ClearMemory);
            var elapsedCopy = new NativeArray<double>(m_max, Allocator.Temp);
            var elapsedJobRun = new NativeArray<double>(m_max, Allocator.Temp);
            var elapsedJobSchedule = new NativeArray<double>(m_max, Allocator.Temp);
            var elapsedJobBatchedRun = new NativeArray<double>(m_max, Allocator.Temp);
            var elapsedJobBatchedSchedule = new NativeArray<double>(m_max, Allocator.Temp);

            ValidateAndReset(arr, 0f, arr.Length);

            // Warmup:
            float destValue = -2f;
            for (int i = 0; i < 3; i++)
            {
                arr.Copy(-2f);
                new Blit<float>
                {
                    Arr = arr,
                    TargetValue = destValue,
                }.Schedule(arr.Length, m_innerloopBatchCount).Complete();
                new Blit<float>
                {
                    Arr = arr,
                    TargetValue = destValue,
                }.Run(arr.Length);
                new BlitBatched<float>
                {
                    Arr = arr,
                    TargetValue = destValue,
                }.RunBatch(arr.Length);
                new BlitBatched<float>
                {
                    Arr = arr,
                    TargetValue = destValue,
                }.ScheduleBatch(arr.Length, m_innerloopBatchCount).Complete();;
            }
            ValidateAndReset(arr, destValue, arr.Length);

            // Tests:
            for (int count = 0; count < m_max; count += m_stride)
            {
                // NW: Checking different array lengths to see how it impacts relative speeds.
                destValue = count;
                var tempArr = arr.GetSubArray(0, count);
                double start = 0;

                //.
                start = Time.realtimeSinceStartupAsDouble;
                tempArr.Copy(destValue);
                elapsedCopy[count] = (Time.realtimeSinceStartupAsDouble - start) * 1000;
                ValidateAndReset(arr, destValue, count);

                //.
                start = Time.realtimeSinceStartupAsDouble;
                new Blit<float>
                {
                    Arr = arr,
                    TargetValue = destValue,
                }.Run(tempArr.Length);
                elapsedJobRun[count] = (Time.realtimeSinceStartupAsDouble - start) * 1000;
                ValidateAndReset(arr, destValue, count);

                //.
                start = Time.realtimeSinceStartupAsDouble;
                new Blit<float>
                {
                    Arr = tempArr,
                    TargetValue = destValue,
                }.Schedule(tempArr.Length, m_innerloopBatchCount).Complete();
                elapsedJobSchedule[count] = (Time.realtimeSinceStartupAsDouble - start) * 1000;
                ValidateAndReset(arr, destValue, count);


                //.
                start = Time.realtimeSinceStartupAsDouble;
                new BlitBatched<float>
                {
                    Arr = tempArr,
                    TargetValue = destValue,
                }.RunBatch(tempArr.Length);
                elapsedJobBatchedRun[count] = (Time.realtimeSinceStartupAsDouble - start) * 1000;
                ValidateAndReset(arr, destValue, count);

                //.
                start = Time.realtimeSinceStartupAsDouble;
                new BlitBatched<float>
                {
                    Arr = tempArr,
                    TargetValue = destValue,
                }.ScheduleBatch(tempArr.Length, m_innerloopBatchCount).Complete();
                elapsedJobBatchedSchedule[count] = (Time.realtimeSinceStartupAsDouble - start) * 1000;
                ValidateAndReset(arr, destValue, count);
            }

            // Sum:
            double avgCopy = 0, avgJobRun = 0, avgJobSchedule = 0, avgJobBatchedRun = 0, avgJobBatchedSchedule = 0;
            int numResultsCount = 0;
            for (int i = 0; i < elapsedJobSchedule.Length; i += m_stride)
            {
                numResultsCount++;
                avgCopy += elapsedCopy[i];
                avgJobBatchedRun += elapsedJobBatchedRun[i];
                avgJobBatchedSchedule += elapsedJobBatchedSchedule[i];
                avgJobRun += elapsedJobRun[i];
                avgJobSchedule += elapsedJobSchedule[i];
            }
            avgCopy /= numResultsCount;
            avgJobBatchedRun /= numResultsCount;
            avgJobBatchedSchedule /= numResultsCount;
            avgJobRun /= numResultsCount;
            avgJobSchedule /= numResultsCount;

            // Draw:
            string result = $"Elapsed [in ms] :\nArray Length | Copy ({avgCopy:0.000_000}) | ParallelForBatched.Run ({avgJobBatchedRun:0.000_000}) | ParallelForBatched.Schedule ({avgJobBatchedSchedule:0.000_000}) | ParallelFor.Run ({avgJobRun:0.000_000}) | ParallelFor.Schedule ({avgJobSchedule:0.000_000})";
            for (int i = 0; i < elapsedJobSchedule.Length; i += m_stride)
            {
                result += $"\n{i:000_000}: {elapsedCopy[i]:0.000_000} | {elapsedJobBatchedRun[i]:0.000_000} | {elapsedJobBatchedSchedule[i]:0.000_000} | {elapsedJobRun[i]:0.000_000} | {elapsedJobSchedule[i]:0.000_000}";
            }

            arr.Dispose();
            elapsedCopy.Dispose();
            elapsedJobRun.Dispose();
            elapsedJobSchedule.Dispose();
            elapsedJobBatchedRun.Dispose();
            elapsedJobBatchedSchedule.Dispose();

            Debug.Log(result);
            string filePath = $"{Application.persistentDataPath}/{GetType().Name}_{DateTime.UtcNow:yyyy-MM-dd_hh-mm-ss}.csv";
            Debug.Log(filePath);
            File.WriteAllText(filePath, result);
        }
    }

    private void ValidateAndReset(NativeArray<float> arr, float expectedValue, int writeCount)
    {
        if (m_assert)
        {
            for (int index = 0; index < arr.Length; index++)
            {
                Assert.AreEqual(index < writeCount ? expectedValue : ResetValue, arr[index]);
            }
        }

        // NW: Use the fastest method to reset:
        arr.Copy(ResetValue);
    }
}


[BurstCompile]
public struct BlitBatched<T> : IJobParallelForBatch
    where T : struct
{
    [NoAlias]
    [WriteOnly]
    public NativeArray<T> Arr;

    [ReadOnly]
    public T TargetValue;

    public void Execute(int startIndex, int count)
    {
        for (int end = startIndex + count; startIndex < end; startIndex++)
        {
            Arr[startIndex] = TargetValue;
        }
    }
}

[BurstCompile]
public struct Blit<T> : IJobParallelFor
    where T : struct
{
    [NoAlias]
    [WriteOnly]
    public NativeArray<T> Arr;

    [ReadOnly]
    public T TargetValue;

    public void Execute(int index)
    {
        Arr[index] = TargetValue;
    }
}

public static unsafe class MemCpyUtils
{
    public static void Copy<T>(this NativeArray<T> arr, T value)
        where T : unmanaged
    {
        if (arr.Length == 0) return;
        arr[0] = value;
        if (arr.Length == 1) return;

        int size = Marshal.SizeOf<T>();
        var ptr = arr.GetUnsafePtr();
        UnsafeUtility.MemCpyReplicate(ptr, ptr, size, arr.Length);
    }
}
