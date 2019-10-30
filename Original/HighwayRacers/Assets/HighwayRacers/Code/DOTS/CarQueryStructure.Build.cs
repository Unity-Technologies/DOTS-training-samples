using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

partial struct CarQueryStructure
{
    [BurstCompile]
    private struct LaneSortJob : IJobForEachWithEntity<CarBasicState>
    {
        public int CarCount;
        [NativeDisableContainerSafetyRestriction] public NativeArray<CarEntityAndState> CarLanes;
        public NativeArray<int> LaneCarCounts;

        public unsafe void Execute(Entity entity, int index, [ReadOnly] ref CarBasicState carState)
        {
            UnityEngine.Debug.Assert(carState.Lane >= 0 && carState.Lane <= 3);
            var lane1 = (int)math.floor(carState.Lane);
            var lane2 = (int)math.ceil(carState.Lane);

            int* laneCarCountsRaw = (int*)LaneCarCounts.GetUnsafePtr();

            CarLanes[lane1 * CarCount + Interlocked.Increment(ref laneCarCountsRaw[lane1]) - 1] = new CarEntityAndState()
            {
                EntityIndex = index,
                State = carState
            };
            if (lane2 != lane1)
            {
                CarLanes[lane2 * CarCount + Interlocked.Increment(ref laneCarCountsRaw[lane2]) - 1] = new CarEntityAndState()
                {
                    EntityIndex = index,
                    State = carState
                };
            }
        }
    }

    [BurstCompile]
    private struct PositionSortJob : IJobParallelFor
    {
        public int CarCount;
        [ReadOnly] public NativeArray<int> LaneCarCounts;

        [NativeDisableParallelForRestriction]
        public NativeArray<CarEntityAndState> CarLanes;

        private struct PositionSort : IComparer<CarEntityAndState>
        {
            public int Compare(CarEntityAndState x, CarEntityAndState y)
                => x.State.Position.CompareTo(y.State.Position);
        }

        public void Execute(int index)
        {
            CarLanes.Slice(index * CarCount, LaneCarCounts[index]).Sort(new PositionSort());
        }
    }

    [BurstCompile]
    private struct BuildSortIndicesMapping : IJob
    {
        public int CarCount;
        [ReadOnly] public NativeArray<CarEntityAndState> CarLanes;
        [ReadOnly] public NativeArray<int> LaneCarCounts;

        public NativeArray<int2> SortIndices;

        public void Execute()
        {
            for (int lane = 0; lane < 4; ++lane)
            {
                var laneArray = CarLanes.Slice(lane * CarCount, LaneCarCounts[lane]);
                for (int j = 0; j < laneArray.Length; ++j)
                {
                    var carIndex = laneArray[j].EntityIndex;
                    var carLane = laneArray[j].State.Lane;
                    UnityEngine.Debug.Assert(math.abs(carLane - lane) < 1);
                    var e = SortIndices[carIndex];
                    e[math.abs(lane - carLane) <= 0.5f ? 0 : 1] = j;
                    SortIndices[carIndex] = e;
                }
            }
        }
    }

    public JobHandle Build(ComponentSystemBase componentSystem, JobHandle dependency)
    {
        dependency = new LaneSortJob()
        {
            CarCount = CarCount,
            CarLanes = CarLanes,
            LaneCarCounts = LaneCarCounts,
        }.Schedule(componentSystem, dependency);

        dependency = new PositionSortJob()
        {
            CarCount = CarCount,
            LaneCarCounts = LaneCarCounts,
            CarLanes = CarLanes,
        }.Schedule(4, 1, dependency);

        return new BuildSortIndicesMapping()
        {
            CarCount = CarCount,
            LaneCarCounts = LaneCarCounts,
            CarLanes = CarLanes,

            SortIndices = OrderedCarLaneIndices,
        }.Schedule(dependency);
    }
}
