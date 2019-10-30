#define ENABLE_TEST

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[AlwaysUpdateSystem]
public class CarSystem : JobComponentSystem
{
    [BurstCompile]
    private struct BuildSortIndicesMapping : IJob
    {
        public int CarCount;
        [ReadOnly] public NativeArray<CarEntityAndState> Lanes;
        [ReadOnly] public NativeArray<int> LaneCounts;

        public NativeArray<int2> SortIndices;

        public void Execute()
        {
            for (int lane = 0; lane < 4; ++lane)
            {
                var laneArray = Lanes.Slice(lane * CarCount, LaneCounts[lane]);
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

#if ENABLE_TEST
    bool firstTime = true;
    Random rnd = new Random();
#endif

    EntityQuery CarEntityQuery;

    protected override void OnCreate()
    {
        CarEntityQuery = GetEntityQuery(ComponentType.ReadOnly<CarBasicState>());
    }

    private static CarBasicState GetCarInFront(int carEntityIndex, float carLane, ref NativeArray<int2> sortIndices,
        ref NativeArray<CarEntityAndState> lane0,
        ref NativeArray<CarEntityAndState> lane1,
        ref NativeArray<CarEntityAndState> lane2,
        ref NativeArray<CarEntityAndState> lane3)
    {
        int floor = (int)math.floor(carLane);
        int lane = carLane - floor > 0.5f ? floor + 1 : floor;

        UnityEngine.Debug.Assert(lane >= 0 && lane < 4);
        var sortedIndex = sortIndices[carEntityIndex].x;
        switch (lane)
        {
            default:
            case 0:
                return lane0[sortedIndex != 0 ? sortedIndex - 1 : lane0.Length - 1].State;
            case 1:
                return lane1[sortedIndex != 0 ? sortedIndex - 1 : lane1.Length - 1].State;
            case 2:
                return lane2[sortedIndex != 0 ? sortedIndex - 1 : lane2.Length - 1].State;
            case 3:
                return lane3[sortedIndex != 0 ? sortedIndex - 1 : lane3.Length - 1].State;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        const int carCount = 1000;

#if ENABLE_TEST
        if (firstTime)
        {
            rnd.InitState(9999);
            // Spawn a bunch of Lane and Position components for testing purpose.
            for (int i = 0; i < carCount; ++i)
            {
                var e = EntityManager.CreateEntity();
                EntityManager.AddComponentData(e, new CarBasicState()
                {
                    Lane = rnd.NextFloat(0, 3),
                    Position = rnd.NextFloat(0, 500),
                    Speed = rnd.NextFloat(0, 5)
                });
            }
            firstTime = false;
        }
#endif

        var carEntitiesInLane = new NativeArray<CarEntityAndState>(carCount * 4, Allocator.TempJob);
        var laneCounts = new NativeArray<int>(4, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        // Index into the sorted arrays (two at most for floor(lane) and ceil(lane)).
        var carSortIndices = new NativeArray<int2>(carCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        inputDeps = new LaneSortJob()
        {
            CarCount = carCount,
            Lanes = carEntitiesInLane,
            LaneCounts = laneCounts,
        }.Schedule(this, inputDeps);

        inputDeps = new PositionSortJob()
        {
            CarCount = carCount,
            LaneCounts = laneCounts,
            Lanes = carEntitiesInLane,
        }.Schedule(4, 1, inputDeps);

        inputDeps = new BuildSortIndicesMapping()
        {
            CarCount = carCount,
            LaneCounts = laneCounts,
            Lanes = carEntitiesInLane,

            SortIndices = carSortIndices,
        }.Schedule(inputDeps);

        // 3. Car logic {Entity, index, velocity, state }
        // 4. Compose the matrices for each mesh instance for correct rendering.

        // TODO: dispose the arrays on completing jobs.
        inputDeps.Complete();

        carEntitiesInLane.Dispose();
        laneCounts.Dispose();
        carSortIndices.Dispose();

        return inputDeps;
    }
}
