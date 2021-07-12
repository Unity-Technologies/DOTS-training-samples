using System;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public class DeterministicNativeQueueTest : MonoBehaviour
{
    public int[] numIterations = {0, 1, 8, 100, 1000, 10000};
    public const int innerloopBatchCount = 16;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Time.frameCount == 2)
        {
            Benchmarker[] tests =
            {
                new Benchmarker("Sequential", (seed, writePerc, length) =>
                {
                    using var queue = new NativeQueue<float>(Allocator.TempJob);
                    var start = Time.realtimeSinceStartupAsDouble;
                    new SequentialJob
                    {
                        sequentialQ = queue,
                        length = length,
                        seed = seed,
                        writePercentage = writePerc,
                    }.Run(); 
                    //.Schedule().Complete();
                    var results = queue.ToArray(Allocator.Temp);
                    return (Time.realtimeSinceStartupAsDouble - start, results);
                }),   
                new Benchmarker("ParallelNoDeterminism", (seed, writePerc, length) =>
                {
                    using var queue = new NativeQueue<float>(Allocator.TempJob);
                    var start = Time.realtimeSinceStartupAsDouble;
                    new ParallelNoDeterminismJob
                    {
                        sequentialQ = queue.AsParallelWriter(),
                        seed = seed,
                        writePercentage = writePerc,
                    }.Run(length);
                    //.Schedule(length, innerloopBatchCount).Complete();
                    var results = queue.ToArray(Allocator.Temp);
                    return (Time.realtimeSinceStartupAsDouble - start, results);
                }),
                new Benchmarker("DeterministicViaInjectIndex", (seed, writePercentage, length) =>
                {
                    using var queue = new NativeQueue<(int, float)>(Allocator.TempJob);
                    var start = Time.realtimeSinceStartupAsDouble;
                    var queueParallelWriter = queue.AsParallelWriter();
                    var iiJob = new InjectIndexJob
                    {
                        injectIndexQ = queueParallelWriter,
                        seed = seed,
                        writePercentage = writePercentage,
                    };
                    iiJob.Run(length);//.Schedule(length, innerloopBatchCount).Complete();
                    using var temp = queue.ToArray(Allocator.Temp);
                    temp.Sort(iiJob);
                    var results = new NativeArray<float>(temp.Length, Allocator.Temp);
                    for (int i = 0; i < results.Length; i++)
                        results[i] = temp[i].Item2;

                    return (Time.realtimeSinceStartupAsDouble - start, results);
                }), 
                new Benchmarker("ViaFilterJob+IterateIndicesJob", (seed, writePerc, length) =>
                {
                    using var indices = new NativeList<int>(length, Allocator.TempJob);
                    var start = Time.realtimeSinceStartupAsDouble;
                    new ViaFilterJob
                    {
                        seed = seed,
                        writePercentage = writePerc,
                    }.ScheduleAppend(indices, length, innerloopBatchCount).Complete();
                    var results = new NativeArray<float>(indices.Length, Allocator.TempJob);
                    new IterateIndicesJob
                    {
                        indices = indices,
                        results = results,
                        seed = seed,
                    }.Run();//.Schedule().Complete();
                    return (Time.realtimeSinceStartupAsDouble - start, results);
                }),
            };

            Benchmarker.RunAndOutputTests(GetType().Name, numIterations, tests);
        }
    }

    [NoAlias]
    [BurstCompile]
    public struct SequentialJob : IJob
    {
        [NoAlias]
        public NativeQueue<float> sequentialQ;

        public double writePercentage;
        public uint seed;
        public int length;
        
        public void Execute()
        {
            for (var index = 0; index < length; index++)
            {
                var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
                if (rand < writePercentage)
                {
                    sequentialQ.Enqueue(rand);
                }
            }
        }
    }     
    [NoAlias]
    [BurstCompile]
    public struct ParallelNoDeterminismJob : IJobParallelFor
    {
        [NoAlias]
        public NativeQueue<float>.ParallelWriter sequentialQ;

        public double writePercentage;
        public uint seed;

        public void Execute(int index)
        {
            var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
            if (rand < writePercentage)
            {
                sequentialQ.Enqueue(rand);
            }
        }
    }   
    
    [NoAlias]
    [BurstCompile]
    public struct InjectIndexJob : IJobParallelFor, IComparer<(int, float)>
    {
        [NativeSetThreadIndex]
        public int threadId;
        
        [NoAlias]
        public NativeQueue<(int, float)>.ParallelWriter injectIndexQ;
        
        public double writePercentage;
        public uint seed;

        public void Execute(int index)
        {
            var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
            if (rand < writePercentage)
            {
                injectIndexQ.Enqueue((index, rand));
            }
        }

        public int Compare((int, float) x, (int, float) y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }  
    
    [NoAlias]
    [BurstCompile]
    public struct IterateIndicesJob : IJob
    {
        [NoAlias]
        public NativeArray<int> indices;
        [NoAlias]
        public NativeArray<float> results;
        public uint seed;

        public void Execute()
        {
            for (var i = 0; i < indices.Length; i++)
            {
                results[i] = (float)Squirrel3.NextDouble((uint)indices[i], seed, 0, 1);
            }
        }
    }  
    [NoAlias]
    [BurstCompile]
    public struct ViaFilterJob : IJobParallelForFilter
    {
        [NativeSetThreadIndex]
        public int threadId;
        
        public double writePercentage;
        public uint seed;

        public bool Execute(int index)
        {
            var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
            return rand < writePercentage;
        }
    } 
    
    // [NoAlias]
    // [BurstCompile]
    // public struct DirectToArrayViaRefJob : IJobParallelFor
    // {
    //     [NativeSetThreadIndex]
    //     public int threadId;
    //     
    //     [NoAlias]
    //     [NativeDisableParallelForRestriction]
    //     public NativeArray<float> resultsDirectToArray;
    //     
    //     public double writePercentage;
    //     public uint seed;
    //
    //     public unsafe void Execute(int index)
    //     {
    //         var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
    //         if (rand < writePercentage)
    //         {
    //             ref var arrayElementAsRef = ref UnsafeUtility.ArrayElementAsRef<float>(resultsDirectToArray.GetUnsafePtr(), index);
    //             arrayElementAsRef = rand;
    //         }
    //     }
    // }  
    //
    // [NoAlias]
    // [BurstCompile]
    // public struct DirectToArrayViaPointerJob : IJobParallelFor
    // {
    //     [NativeSetThreadIndex]
    //     public int threadId;
    //     
    //     [NoAlias]
    //     [NativeDisableParallelForRestriction]
    //     public NativeArray<float> resultsDirectToArray;
    //     
    //     public double writePercentage;
    //     public uint seed;
    //
    //     public unsafe void Execute(int index)
    //     {
    //         var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
    //         if (rand < writePercentage)
    //         {
    //             ((float*)resultsDirectToArray.GetUnsafePtr())[index] = rand;
    //         }
    //     }
    // }  
    //
    // [NoAlias]
    // [BurstCompile]
    // public struct DirectToArrayViaIndexer : IJobParallelFor
    // {
    //     [NativeSetThreadIndex]
    //     public int threadId;
    //     
    //     [NoAlias]
    //     [NativeDisableParallelForRestriction]
    //     public NativeArray<float> resultsDirectToArray;
    //     
    //     public double writePercentage;
    //     public uint seed;
    //
    //     public void Execute(int index)
    //     {
    //         var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
    //         if (rand < writePercentage)
    //         {
    //             resultsDirectToArray[index] = rand;
    //         }
    //     }
    // } 
    //
    // [NoAlias]
    // [BurstCompile]
    // public struct DirectToArrayMathSelectJob : IJobParallelFor
    // {
    //     [NativeSetThreadIndex]
    //     public int threadId;
    //     
    //     [NoAlias]
    //     [NativeDisableParallelForRestriction]
    //     public NativeArray<float> resultsDirectToArray;
    //     
    //     public double writePercentage;
    //     public uint seed;
    //
    //     public unsafe void Execute(int index)
    //     {
    //         var rand = (float)Squirrel3.NextDouble((uint)index, seed, 0, 1);
    //         ref var arrayElementAsRef = ref UnsafeUtility.ArrayElementAsRef<float>(resultsDirectToArray.GetUnsafePtr(), index);
    //         arrayElementAsRef = math.@select(arrayElementAsRef, rand, rand < writePercentage);
    //     }
    // }  
    //
    // [NoAlias]
    // [BurstCompile]
    // public struct PerThreadSameCacheLineJob : IJobParallelFor
    // {
    //     [NativeSetThreadIndex]
    //     public int threadId;
    //     [NoAlias]
    //     [NativeDisableParallelForRestriction]
    //     public NativeArray<long> counter;
    //     
    //     public double writePercentage;
    //     public uint seed;
    //
    //     public unsafe void Execute(int index)
    //     {
    //         if (Squirrel3.NextDouble((uint)index, seed, 0, 1) < writePercentage)
    //         { 
    //             counter[threadId]++;
    //         }
    //     }
    // }
}