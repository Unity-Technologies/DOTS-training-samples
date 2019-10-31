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
        [NativeDisableContainerSafetyRestriction] public NativeArray<CarIndexAndState> LaneCars;
        public NativeArray<int> LaneCarCounts;

        public unsafe void Execute(Entity entity, int index, ref CarBasicState carState)
        {
            UnityEngine.Debug.Assert(carState.Lane >= 0 && carState.Lane <= 3);
            var lane1 = (int)math.floor(carState.Lane);
            var lane2 = (int)math.ceil(carState.Lane);

            int* laneCarCountsRaw = (int*)LaneCarCounts.GetUnsafePtr();

            LaneCars[lane1 * CarCount + Interlocked.Increment(ref laneCarCountsRaw[lane1]) - 1] = new CarIndexAndState()
            {
                EntityArrayIndex = index,
                State = carState
            };
            if (lane2 != lane1)
            {
                LaneCars[lane2 * CarCount + Interlocked.Increment(ref laneCarCountsRaw[lane2]) - 1] = new CarIndexAndState()
                {
                    EntityArrayIndex = index,
                    State = carState
                };
            }
        }
    }

    [BurstCompile]
    private struct PositionSortJob : IJobParallelFor
    {
        public float HighwayLen;
        public int CarCount;
        [ReadOnly] public NativeArray<int> LaneCarCounts;

        [NativeDisableParallelForRestriction]
        public NativeArray<CarIndexAndState> LaneCars;

        private struct PositionSort : IComparer<CarIndexAndState>
        {
            public int Lane;
            public float HighwayLen;
            public int Compare(CarIndexAndState x, CarIndexAndState y)
            {
                float posX = Utilities.ConvertPositionToLane(x.State.Position, x.State.Lane, Lane, HighwayLen);
                float posY = Utilities.ConvertPositionToLane(y.State.Position, y.State.Lane, Lane, HighwayLen);
                return posX.CompareTo(posY);
            }
        }

        public void Execute(int index)
        {
            LaneCars.Slice(index * CarCount, LaneCarCounts[index]).Sort(new PositionSort()
            {
                Lane = index,
                HighwayLen = HighwayLen
            });
        }
    }

    [BurstCompile]
    private struct BuildSortIndicesMapping : IJob
    {
        public int CarCount;
        [ReadOnly] public NativeArray<CarIndexAndState> LaneCars;
        [ReadOnly] public NativeArray<int> LaneCarCounts;

        public NativeArray<int2> SortIndices;

        public void Execute()
        {
            for (int lane = 0; lane < 4; ++lane)
            {
                var laneArray = LaneCars.Slice(lane * CarCount, LaneCarCounts[lane]);
                for (int i = 0; i < laneArray.Length; ++i)
                {
                    var arrayIndex = laneArray[i].EntityArrayIndex;
                    var carLane = laneArray[i].State.Lane;
                    UnityEngine.Debug.Assert(math.abs(carLane - lane) < 1);
                    var e = SortIndices[arrayIndex];
                    // TODO: different lanes always writes to the different float2 component. Potentially jobifiable.
                    SortIndices[arrayIndex] = math.int2(lane <= carLane ? i : e.x, lane >= carLane ? i : e.y);
                }
            }
        }
    }

    public JobHandle RebuildOvertakingCarEntityArrayIndex(ComponentSystemBase componentSystem, JobHandle dependency)
    {
        // TODO:
        throw new System.NotImplementedException();
    }

    public JobHandle Build(ComponentSystemBase componentSystem, JobHandle dependency)
    {
        dependency = new LaneSortJob()
        {
            CarCount = CarCount,
            LaneCars = LaneCars,
            LaneCarCounts = LaneCarCounts,
        }.Schedule(componentSystem, dependency);

        dependency = new PositionSortJob()
        {
            HighwayLen = HighwayLen,
            CarCount = CarCount,
            LaneCarCounts = LaneCarCounts,
            LaneCars = LaneCars,
        }.Schedule(4, 1, dependency);

        return new BuildSortIndicesMapping()
        {
            CarCount = CarCount,
            LaneCarCounts = LaneCarCounts,
            LaneCars = LaneCars,

            SortIndices = OrderedCarLaneIndices,
        }.Schedule(dependency);
    }
}
