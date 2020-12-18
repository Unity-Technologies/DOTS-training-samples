using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

[UpdateBefore(typeof(FireSystem))]
public class ThrowerSystem : SystemBase
{
    static public NativeArray<int2> m_Throwers;
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
        [NativeDisableContainerSafetyRestriction]
        public NativeList<int2>.ParallelWriter fireLine;
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

                    for (int j=0; j<neighborOffsets.Length; ++j)
                    {
                        int2 neighborCoord = linearCoord + neighborOffsets[j];
                        if (math.all(neighborCoord < new int2(xDim, yDim)) && math.all(neighborCoord >= int2.zero))
                        {
                            int neighborIndex = neighborCoord.y * xDim + neighborCoord.x;
                            if (fireSimBoard[neighborIndex] >= fireThreshold)
                            {
                                fireLine.AddNoResize(linearCoord);
#if BB_DEBUG_FLAGS
                                debugFlags[i] = 1;
#endif
                                break;
                            }
                        }
                    }
                }
            }
        }
    };

    [BurstCompile]
    struct FindNewTargetJob : IJob
    {
        [ReadOnly]
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<int2> fireLine;
        [ReadOnly]
        public NativeArray<int2> currentThrowerCoords;
        [ReadOnly]
        public int xDim;
        [ReadOnly]
        public int yDim;

        public NativeArray<int2> newThrowerCoords;

        private int lengthSq(int2 a, int2 b)
        {
            int2 t0 = a - b;
            return math.dot(t0, t0);
        }

        public void Execute()
        {
            if (fireLine.Length == 0)
                return;

            for (int i=0; i<currentThrowerCoords.Length; ++i)
            {
                int2 pos = currentThrowerCoords[i];
                int2 minTarget = fireLine[0];
                int minDistSqr = lengthSq(fireLine[0], pos);
                for (int j=1; j<fireLine.Length; ++j)
                {
                    int distSqr = lengthSq(fireLine[j], pos);
                    if (distSqr < minDistSqr)
                    {
                        minDistSqr = distSqr;
                        minTarget = fireLine[j];
                    }
                }

                newThrowerCoords[i] = minTarget;
            }
        }
    };

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

    protected override void OnDestroy()
    {
        m_Throwers.Dispose();
        m_NeighborOffsets.Dispose();
    }

    protected override void OnUpdate()
    {
        NativeArray<int2> throwerCoords = m_Throwers;
        NativeArray<int2> newThrowerCoords = new NativeArray<int2>(throwerCoords.Length, Allocator.TempJob);

#if BB_DEBUG_FLAGS
        Entities.WithoutBurst().ForEach((ref DynamicBuffer<BoardDebugElement> boardDebugFlags, in DynamicBuffer<BoardElement> board) =>
#else
        Entities.WithoutBurst().ForEach((in DynamicBuffer<BoardElement> board) =>
#endif
        {
            NativeList<int2> fireLine = new NativeList<int2>(board.Length, Allocator.TempJob);

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

            JobHandle findFireLineHandle = findFireLineJob.ScheduleBatch(board.Length, board.Length/SystemInfo.processorCount-1, Dependency);

            FindNewTargetJob findNewTargetJob = new FindNewTargetJob
            {
                fireLine = fireLine.AsDeferredJobArray(),
                currentThrowerCoords = throwerCoords,
                xDim = FireSimConfig.xDim,
                yDim = FireSimConfig.yDim,
                newThrowerCoords = newThrowerCoords
            };
            JobHandle findNewTargetJobHandle = findNewTargetJob.Schedule(findFireLineHandle);

            Dependency = JobHandle.CombineDependencies(Dependency, findNewTargetJobHandle);

            fireLine.Dispose(Dependency);
        }).Run();

		float currentDeltaTime = Time.DeltaTime;
        float throwerSpeed = FireSimConfig.throwerSpeed;

        // update render (and cached) coordinates
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref LocalToWorld localToWorld, ref Thrower thrower) =>
        {
            // take a step toward target
            int2 dir0   = thrower.TargetCoord - thrower.Coord;
            float2 dir1 = new float2(dir0);
            if (math.lengthsq(dir1) > math.EPSILON)
            {
                float2 dir2 = math.normalize(dir1);
                float2 step = dir2 * currentDeltaTime;
                thrower.GridPosition += step * throwerSpeed;
                thrower.Coord = new int2(thrower.GridPosition);
            }

            // copy to render data
            localToWorld.Value.c3 = new float4(thrower.GridPosition.x, 1.0f, thrower.GridPosition.y, localToWorld.Value.c3.w);

            // update new target
            thrower.TargetCoord = newThrowerCoords[entityInQueryIndex];

            // save current position for other folks
            throwerCoords[entityInQueryIndex] = thrower.Coord;

        }).Schedule();

        newThrowerCoords.Dispose(Dependency);
    }
}
