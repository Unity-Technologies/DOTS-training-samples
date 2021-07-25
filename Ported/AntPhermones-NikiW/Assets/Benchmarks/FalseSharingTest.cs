using System;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// https://preshing.com/20120515/memory-reordering-caught-in-the-act/
public class FalseSharingTest : MonoBehaviour
{
    [SerializeField]
    int[] m_NumIterationsForTestId;
    
    [SerializeField]
    Allocator m_Allocator;

    unsafe void Update()
    {
        if (JobsUtility.JobWorkerCount < 2) throw new InvalidOperationException("This test requires at least 2 worker threads!");

        if (Input.GetKeyDown(KeyCode.Space) || Time.frameCount == 3)
        {
            const int bufferSize = 32;
            m_Allocator = Allocator.TempJob;
            var tests = new[]
            {
                new Benchmarker("OverlappingCacheline", (seed, _, length) =>
                {
                    // NW: Hack to ensure we test overlapping cachelines.
                    NativeArray<byte> tempA;
                    NativeArray<byte> tempB;
                    while(true)
                    {
                        tempA = new NativeArray<byte>(bufferSize, m_Allocator);
                        tempB = new NativeArray<byte>(bufferSize, m_Allocator);
                        if (CacheLinesOverlap(tempA, bufferSize, tempB, bufferSize))
                        {
                            break;
                        }
                        tempA.Dispose();
                        tempB.Dispose();
                    }

                    var jobA = new ArbitraryReadWriteJob
                    {
                        Buffer = tempA,
                        NumIterations = length,
                        Length = bufferSize,
                    }.Schedule();
                    var jobB = new ArbitraryReadWriteJob
                    {
                        Buffer = tempB,
                        NumIterations = length,
                        Length = bufferSize,
                    }.Schedule();
                    
                    var combineDependencies = JobHandle.CombineDependencies(jobA, jobB);
                    var start = Time.realtimeSinceStartupAsDouble;
                    JobHandle.ScheduleBatchedJobs();
                    combineDependencies.Complete();
                    var elapsed = Time.realtimeSinceStartupAsDouble - start;

                    var resultInfo = new NativeArray<float>(1, Allocator.Temp);
                    var sharesCacheLines = CacheLinesOverlap(tempA, bufferSize, tempB, bufferSize);
                    Debug.Assert(sharesCacheLines, $"OverlappingCacheline: {(IntPtr)tempA.GetUnsafePtr()} vs {(IntPtr)tempB.GetUnsafePtr()}");
                    resultInfo[0] = sharesCacheLines ? 1f : 0f;
                    return (elapsed, resultInfo);
                }),
                new Benchmarker("Padded", (seed, _, length) =>
                {
                    using var tempA = new NativeArray<byte>(PadToCacheLine(bufferSize), m_Allocator);
                    using var tempB = new NativeArray<byte>(PadToCacheLine(bufferSize), m_Allocator);

                    var jobA = new ArbitraryReadWriteJob
                    {
                        Buffer = tempA,
                        NumIterations = length,
                        Length = bufferSize,
                    }.Schedule();
                    var jobB = new ArbitraryReadWriteJob
                    {
                        Buffer = tempB,
                        NumIterations = length,
                        Length = bufferSize,
                    }.Schedule();
                    
                    var combineDependencies = JobHandle.CombineDependencies(jobA, jobB);
                    var start = Time.realtimeSinceStartupAsDouble;
                    JobHandle.ScheduleBatchedJobs();
                    combineDependencies.Complete();
                    var elapsed = Time.realtimeSinceStartupAsDouble - start;

                    var resultInfo = new NativeArray<float>(1, Allocator.Temp);
                    var sharesCacheLines = CacheLinesOverlap(tempA, bufferSize, tempB, bufferSize);
                    Debug.Assert(! sharesCacheLines, $"Padded: {(IntPtr)tempA.GetUnsafePtr()} vs {(IntPtr)tempB.GetUnsafePtr()}");
                    resultInfo[0] = sharesCacheLines ? 1f : 0f;
                    return (elapsed, resultInfo);
                }),
            };

            Benchmarker.RunAndOutputTests(GetType().Name, m_NumIterationsForTestId, tests);
        }
    }

    public static int PadToCacheLine(int bufferSize)
    {
        return bufferSize + (JobsUtility.CacheLineSize - bufferSize % JobsUtility.CacheLineSize);
    }

    /// <summary>
    ///     We use <see cref="aUtalizedLength"/> and <see cref="bUtalizedLength"/> because we may intentionally pad our array.
    /// </summary>
    public static unsafe bool CacheLinesOverlap<T1, T2>(NativeArray<T1> a, int aUtalizedLength, NativeArray<T2> b, int bUtalizedLength) where   T1 : unmanaged where T2 : unmanaged
    {
        if (aUtalizedLength < 0 || aUtalizedLength > a.Length) throw new ArgumentOutOfRangeException();
        if (bUtalizedLength < 0 || bUtalizedLength > b.Length) throw new ArgumentOutOfRangeException();

        var aStart64 = ((IntPtr)a.GetUnsafePtr()).ToInt64();
        var bStart64 = ((IntPtr)b.GetUnsafePtr()).ToInt64();
        var aEnd64 = aStart64 + sizeof(T1) * aUtalizedLength - 1; // -1 byte because this would otherwise be the first byte of the next buffer. 
        var bEnd64 = bStart64 + sizeof(T2) * bUtalizedLength - 1; //.
        
        var bIsInsideA = bStart64 >= aStart64 && bStart64 <= aEnd64;
        var aIsInsideB = aStart64 >= bStart64 && aEnd64 <= bEnd64;
        var aStartsWithinCacheLineOfB = aEnd64 / JobsUtility.CacheLineSize == bStart64 / JobsUtility.CacheLineSize;
        var bStartsWithinCacheLineOfA = bEnd64 / JobsUtility.CacheLineSize == aStart64  / JobsUtility.CacheLineSize;
        return bIsInsideA || aIsInsideB || aStartsWithinCacheLineOfB || bStartsWithinCacheLineOfA;
    }

    [BurstCompile]
    [NoAlias]
    struct ArbitraryReadWriteJob : IJob
    {
        [NoAlias]
        public NativeArray<byte> Buffer;

        [NoAlias]
        [ReadOnly]
        public int NumIterations;
         
        [NoAlias]
        [ReadOnly]
        public int Length;
        
        public void Execute()
        {
            for (int count = 0; count < NumIterations; count++)
            for (int i = 0; i < Length; i++)
                Buffer[i] = (byte)Squirrel3.NextRand(Buffer[i], (uint)i); // Arbitrary work.
        }
    }
}
