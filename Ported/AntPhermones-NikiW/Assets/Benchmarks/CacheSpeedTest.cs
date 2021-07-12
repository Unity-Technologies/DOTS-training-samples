using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

public class CacheSpeedTest : MonoBehaviour
{
    public long numInnerLoopIterations = 100;
    public int numTestIterations = 10;
    public int innerloopBatchCount = 1;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Time.frameCount == 2)
        {
            var testStart = Time.realtimeSinceStartupAsDouble;
            var resultString = "Write%, atomic via Interlocked.Increment, per-thread counter, unique Per cache-line, per-thread counter same cache-line, Delta";
            for (int writePercentageInt = -10; writePercentageInt <= 100; writePercentageInt++)
            {
                double writePercentage = writePercentageInt * 0.01f; 
                // NW: First 10 tests are "warmup" and should be disregarded.
                bool isWarmup = writePercentage < -0.005f; // NW: Note that writePercentage will never be exactly 0.
                var (perThreadUniqueElapsed, perThreadSameElapsed, atomicElapsed) = Test(innerloopBatchCount, numInnerLoopIterations, writePercentage, numTestIterations);
                
                if(!isWarmup)
                    resultString += $"\n{writePercentage:0.00}, {atomicElapsed:000.000000}, {perThreadUniqueElapsed:000.000000}, {perThreadSameElapsed:000.000000}";
            }
            
            var testDuration = (Time.realtimeSinceStartupAsDouble - testStart);
            var fullResultString = $"RESULTS (test duration: {testDuration:0.00}s, iterating over: {numInnerLoopIterations} {numTestIterations} times, job worker batch count of {innerloopBatchCount}, max job workers: {JobsUtility.MaxJobThreadCount}):\n{resultString}";
            
            Debug.Log(fullResultString);
            var filePath = Application.persistentDataPath + $"/{GetType().Name}_{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.csv";
            Debug.Log(filePath);
            File.WriteAllText(filePath, fullResultString);
        }
    }

    /// <summary>
    ///     Returns the ms time cost of counting values.
    ///     Only useful to compare atomic vs counter.
    ///     Do not use to test performance of the job itself, as we perform arbitrary work.
    /// </summary>
    static (double perThreadUniqueCachelineElapsed, double perThreadSameCachelineElapsed, double atomicElapsed) Test(int innerLoopBatchCount, long numIterations, double writePerc, int numTestIterations)
    {
        Assert.AreEqual(JobsUtility.CacheLineSize, 8 * sizeof(long), "Cache line does not match 8*long!");
        
        var counterAtomic = new NativeArray<long>(1, Allocator.TempJob);
        var counterPerThreadUniqueCacheLine = new NativeArray<long>(8 * JobsUtility.MaxJobThreadCount, Allocator.TempJob);
        var counterPerThreadSameCacheLine = new NativeArray<long>(JobsUtility.MaxJobThreadCount, Allocator.TempJob);

        var atomicCountJob = new AtomicJob
        {
            counter = counterAtomic,
        };
        var perThreadUniqueCacheLineJob = new PerThreadUniqueCacheLineJob
        {
            counter = counterPerThreadUniqueCacheLine,
        }; 
        var perThreadSameCacheLineCountJob  = new PerThreadSameCacheLineJob
        {
            counter = counterPerThreadSameCacheLine,
        };
        
        // Run test:
        double atomicElapsed = 0, perThreadUniqueElapsed = 0, perThreadSameElapsed = 0;
        for (var testId = 0; testId < numTestIterations; testId++)
        {
            // Reset:
            for (var index = 0; index < counterAtomic.Length; index++) counterAtomic[index] = 0;
            for (var index = 0; index < counterPerThreadSameCacheLine.Length; index++) counterPerThreadSameCacheLine[index] = 0;
            for (var index = 0; index < counterPerThreadUniqueCacheLine.Length; index++) counterPerThreadUniqueCacheLine[index] = 0;
            var newSeed = (uint)(Random.value * ushort.MaxValue);
            
            atomicCountJob.writePercentage = perThreadUniqueCacheLineJob.writePercentage = perThreadSameCacheLineCountJob.writePercentage = writePerc;
            atomicCountJob.seed = perThreadUniqueCacheLineJob.seed = perThreadSameCacheLineCountJob.seed = newSeed;

            // Get the correct count:
            long correctCount = 0;
            for (var i = 0; i < numIterations; i++)
                if (Squirrel3.NextDouble((uint)i, newSeed, 0, 1) < writePerc)
                    correctCount++;

            long perThreadUniqueFinalCount = 0, perThreadSameFinalCount = 0, atomicFinalCount = 0;
            
            // Atomic:
            void Atomic()
            {
                var start = Time.realtimeSinceStartupAsDouble;
                atomicCountJob.Schedule((int)numIterations, innerLoopBatchCount).Complete();
                atomicElapsed += Time.realtimeSinceStartupAsDouble - start;
                atomicFinalCount = counterAtomic[0];
            }

            // Per-Thread:
            void PerThreadUnique()
            {
                var start = Time.realtimeSinceStartupAsDouble;
                perThreadUniqueCacheLineJob.Schedule((int)numIterations, innerLoopBatchCount).Complete();
                for (int thread = 0; thread < JobsUtility.MaxJobThreadCount; thread++)
                {
                    perThreadUniqueFinalCount += counterPerThreadUniqueCacheLine[thread * 8];
                }
                perThreadUniqueElapsed += Time.realtimeSinceStartupAsDouble - start;
            }  
            void PerThreadSame()
            {
                var start = Time.realtimeSinceStartupAsDouble;
                perThreadSameCacheLineCountJob.Schedule((int)numIterations, innerLoopBatchCount).Complete();
                for (int thread = 0; thread < JobsUtility.MaxJobThreadCount; thread++)
                {
                    perThreadSameFinalCount += counterPerThreadSameCacheLine[thread];
                }
                perThreadSameElapsed += Time.realtimeSinceStartupAsDouble - start;
            }

            switch (Random.Range(0, 6))
            {
                //  NW: Randomize to remove "who goes first" bias:
                case 0: Atomic();          PerThreadUnique(); PerThreadSame();   break;
                case 1: Atomic();          PerThreadSame();   PerThreadUnique(); break;
                case 2: PerThreadUnique(); Atomic();          PerThreadSame();   break;
                case 3: PerThreadUnique(); PerThreadSame();   Atomic();          break;
                case 4: PerThreadSame();   Atomic();          PerThreadUnique(); break;
                case 5: PerThreadSame();   PerThreadUnique(); Atomic();          break;
                default: throw new InvalidOperationException();
            }

            var assertMsg = $"Correct count is: {correctCount}!\n'Per-thread unique cache-line counter ({perThreadUniqueFinalCount})'\n'per-thread but same cache line counter ({perThreadSameFinalCount})'\n'atomic via Interlocked.Increment counter ({atomicFinalCount})'";
            Assert.AreEqual(correctCount, atomicFinalCount, assertMsg);
            Assert.AreEqual(correctCount, perThreadSameFinalCount, assertMsg);
            Assert.AreEqual(correctCount, perThreadUniqueFinalCount, assertMsg);
        }
        
        counterAtomic.Dispose();
        counterPerThreadSameCacheLine.Dispose();
        counterPerThreadUniqueCacheLine.Dispose();

        return (perThreadUniqueElapsed * 1000, perThreadSameElapsed * 1000, atomicElapsed * 1000);
    }

    [NoAlias]
    [BurstCompile]
    public struct AtomicJob : IJobParallelFor
    {
        [NoAlias]
        [NativeDisableParallelForRestriction]
        public NativeArray<long> counter;

        public double writePercentage;
        public uint seed;
        
        public unsafe void Execute(int index)
        {
            if (Squirrel3.NextDouble((uint)index, seed, 0, 1) < writePercentage)
            {
                Interlocked.Increment(ref ((long*)counter.GetUnsafePtr())[0]);
            }
        }
    }   
    
    [NoAlias]
    [BurstCompile]
    public struct PerThreadUniqueCacheLineJob : IJobParallelFor
    {
        [NativeSetThreadIndex]
        public int threadId;
        
        [NoAlias]
        [NativeDisableParallelForRestriction]
        public NativeArray<long> counter;
        
        public double writePercentage;
        public uint seed;

        public void Execute(int index)
        {
            if (Squirrel3.NextDouble((uint)index, seed, 0, 1) < writePercentage)
            {
                counter[8 * threadId]++;
            }
        }
    }  
    
    [NoAlias]
    [BurstCompile]
    public struct PerThreadSameCacheLineJob : IJobParallelFor
    {
        [NativeSetThreadIndex]
        public int threadId;
        [NoAlias]
        [NativeDisableParallelForRestriction]
        public NativeArray<long> counter;
        
        public double writePercentage;
        public uint seed;

        public unsafe void Execute(int index)
        {
            if (Squirrel3.NextDouble((uint)index, seed, 0, 1) < writePercentage)
            { 
                counter[threadId]++;
            }
        }
    }
}