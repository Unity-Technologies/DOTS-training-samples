using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;

public class ThrowerSystem : SystemBase
{
    public NativeArray<int2> m_Throwers;
    protected NativeArray<int2> m_NeighborOffsets;

    [BurstCompile]
    struct FindFireLineJob : IJobParallelForBatch
    {
        [ReadOnly]
        public NativeArray<float> fireSimBoard;
        [ReadOnly]
        public NativeArray<int2> neighborOffsets;
        [ReadOnly]
        public int xDim;
        [ReadOnly]
        public int yDim;
        [ReadOnly]
        public float fireThreshold;
        public NativeList<int>.ParallelWriter fireLine;
#if BB_DEBUG_FLAGS
        public NativeArray<uint> debugFlags;
#endif

        public void Execute(int startIndex, int count)
        {
            for (int i=startIndex,n=startIndex+count; i<n; ++i)
            {
                float heatValue = fireSimBoard[i];
                if (heatValue < fireThreshold)
                {
                    int2 linearCoord = new int2(i%xDim, i/xDim);

                    bool found = false;
                    for (int j=0; !found && j<neighborOffsets.Length; ++j)
                    {
                        int2 neighborCoord = linearCoord + neighborOffsets[j];
                        if (math.all(neighborCoord < new int2(xDim, yDim)) && math.all(neighborCoord >= int2.zero))
                        {
                            int neighborIndex = neighborCoord.y * xDim + neighborCoord.x;
                            found = fireSimBoard[neighborIndex] >= fireThreshold;
                        }
                    }

                    if (found)
                    {
                        fireLine.AddNoResize(i);
#if BB_DEBUG_FLAGS
                        debugFlags[i] = 1;
#endif
                    }
                }
            }
        }
    };

    [BurstCompile]
    struct FindNewTargetJob : IJob
    {
        [ReadOnly]
        public NativeArray<int>.ReadOnly fireLine;
        public NativeArray<int2> newThrowerCoords;

        public void Execute()
        {
        }
    };


    [BurstCompile]
    struct CopyNewTargetJob : IJob
    {
        [ReadOnly]
        public NativeArray<int2> sourceThrowerCoords;
        public NativeArray<int2> destThrowerCoords;

        public void Execute()
        {
        }
    };

    protected override void OnDestroy()
    {
        m_Throwers.Dispose();
        m_NeighborOffsets.Dispose();
    }

    protected override void OnCreate()
    {
        m_Throwers = new NativeArray<int2>(FireSimConfig.maxTeams, Allocator.Persistent);

        m_NeighborOffsets = new NativeArray<int2>(new [] {new int2(+0, -1),
                new int2(+1, -1),
                new int2(+1, +0),
                new int2(+1, +1),
                new int2(+0, +1),
                new int2(-1, +1),
                new int2(-1, +0),
                new int2(-1, -1)}, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        NativeArray<int2> throwerCoords = m_Throwers;
        NativeArray<int2> newThrowerCoords = new NativeArray<int2>(throwerCoords.Length, Allocator.TempJob);

        Entities.WithoutBurst().ForEach((ref DynamicBuffer<BoardDebugElement> boardDebugFlags, in DynamicBuffer<BoardElement> board) =>
        {
            NativeList<int> fireLine = new NativeList<int>(board.Length, Allocator.TempJob);

            FindFireLineJob findFireLineJob = new FindFireLineJob
            {
                fireSimBoard = board.Reinterpret<float>().AsNativeArray(),
                fireLine = fireLine.AsParallelWriter(),
                neighborOffsets = m_NeighborOffsets,
                xDim = FireSimConfig.xDim,
                yDim = FireSimConfig.yDim,
                fireThreshold = FireSimConfig.fireThreshold,
#if BB_DEBUG_FLAGS
                debugFlags = boardDebugFlags.Reinterpret<uint>().AsNativeArray()
#endif
            };

            JobHandle fireLineHandle = findFireLineJob.ScheduleBatch(board.Length, board.Length/SystemInfo.processorCount-1, Dependency);

            FindNewTargetJob findNewTargetJob = new FindNewTargetJob
            {
                fireLine = fireLine.AsParallelReader(),
                newThrowerCoords = newThrowerCoords
            };

            CopyNewTargetJob copyNewTargetJob = new CopyNewTargetJob
            {
                sourceThrowerCoords = newThrowerCoords,
                destThrowerCoords = throwerCoords
            };

            JobHandle copyNewTargetJobHandle = copyNewTargetJob.Schedule(findNewTargetJob.Schedule(fireLineHandle));
            Dependency = JobHandle.CombineDependencies(Dependency, copyNewTargetJobHandle);

            fireLine.Dispose(Dependency);
        }).Run();

        // update render (and cached) coordinates
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, in Thrower thrower) =>
        {
            throwerCoords[entityInQueryIndex] = thrower.Coord;
            translation.Value = new float3(thrower.Coord.x, 1.0f, thrower.Coord.y);
        }).Schedule();

        newThrowerCoords.Dispose(Dependency);
    }
}
