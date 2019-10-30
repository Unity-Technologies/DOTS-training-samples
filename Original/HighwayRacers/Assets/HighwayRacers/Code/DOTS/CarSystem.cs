#define ENABLE_TEST

using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[AlwaysUpdateSystem]
public class CarSystem : JobComponentSystem
{
    private struct CarEntityAndState
    {
        public int EntityIndex;
        public CarBasicState State;
    }

    // Sort cars into a list of each lane. Cars might be in two lanes if it's changing lanes.
    // TODO: for now all the work needs to be done in one thread because car instances write to the lists concurrently
    // which is not supported. Could preallocate the array to its maximum (= total entity count which we know beforehand)
    // and do atomic increment on the write position.
    [BurstCompile]
    private struct LaneSortJob : IJob
    {
        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<Entity> Entities;
        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<CarBasicState> CarStates;

        public NativeList<CarEntityAndState> Lane0;
        public NativeList<CarEntityAndState> Lane1;
        public NativeList<CarEntityAndState> Lane2;
        public NativeList<CarEntityAndState> Lane3;

        private static ref NativeList<CarEntityAndState> GetLaneList(ref LaneSortJob job, int index)
        {
            if (index == 0)
                return ref job.Lane0;
            else if (index == 1)
                return ref job.Lane1;
            else if (index == 2)
                return ref job.Lane2;
            else
                return ref job.Lane3;
        }

        public void Execute()
        {
            UnityEngine.Debug.Assert(Entities.Length == CarStates.Length);
            for (int i = 0; i < Entities.Length; ++i)
            {
                var carState = CarStates[i];
                int lane1 = (int)math.floor(carState.Lane);
                int lane2 = (int)math.ceil(carState.Lane);
                GetLaneList(ref this, lane1).Add(new CarEntityAndState() { EntityIndex = i, State = carState });
                if (lane2 != lane1)
                    GetLaneList(ref this, lane2).Add(new CarEntityAndState() { EntityIndex = i, State = carState });
            }
        }
    }

    [BurstCompile]
    private struct PositionSortJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<CarEntityAndState> Lane0;

        [NativeDisableParallelForRestriction]
        public NativeArray<CarEntityAndState> Lane1;

        [NativeDisableParallelForRestriction]
        public NativeArray<CarEntityAndState> Lane2;

        [NativeDisableParallelForRestriction]
        public NativeArray<CarEntityAndState> Lane3;

        private static ref NativeArray<CarEntityAndState> GetLaneArray(ref PositionSortJob job, int index)
        {
            if (index == 0)
                return ref job.Lane0;
            else if (index == 1)
                return ref job.Lane1;
            else if (index == 2)
                return ref job.Lane2;
            else
                return ref job.Lane3;
        }

        private struct PositionSort : IComparer<CarEntityAndState>
        {
            public int Compare(CarEntityAndState x, CarEntityAndState y)
                => x.State.Position.CompareTo(y.State.Position);
        }

        public void Execute(int index)
        {
            GetLaneArray(ref this, index).Sort(new PositionSort());
        }
    }

    [BurstCompile]
    private struct BuildSortIndicesMapping : IJob
    {
        [ReadOnly] public NativeArray<CarEntityAndState> Lane0;
        [ReadOnly] public NativeArray<CarEntityAndState> Lane1;
        [ReadOnly] public NativeArray<CarEntityAndState> Lane2;
        [ReadOnly] public NativeArray<CarEntityAndState> Lane3;

        public NativeArray<int2> SortIndices;

        private static ref NativeArray<CarEntityAndState> GetLaneArray(ref BuildSortIndicesMapping job, int index)
        {
            if (index == 0)
                return ref job.Lane0;
            else if (index == 1)
                return ref job.Lane1;
            else if (index == 2)
                return ref job.Lane2;
            else
                return ref job.Lane3;
        }

        public void Execute()
        {
            for (int lane = 0; lane < 4; ++lane)
            {
                ref var laneArray = ref GetLaneArray(ref this, lane);
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

        var carEntitiesLane0 = new NativeList<CarEntityAndState>(Allocator.TempJob);
        var carEntitiesLane1 = new NativeList<CarEntityAndState>(Allocator.TempJob);
        var carEntitiesLane2 = new NativeList<CarEntityAndState>(Allocator.TempJob);
        var carEntitiesLane3 = new NativeList<CarEntityAndState>(Allocator.TempJob);

        // Index into the sorted arrays (two at most for floor(lane) and ceil(lane)).
        var carSortIndices = new NativeArray<int2>(carCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        inputDeps = new LaneSortJob()
        {
            Entities = CarEntityQuery.ToEntityArray(Allocator.TempJob, out var entityJob),
            CarStates = CarEntityQuery.ToComponentDataArray<CarBasicState>(Allocator.TempJob, out var carStateJob),

            Lane0 = carEntitiesLane0,
            Lane1 = carEntitiesLane1,
            Lane2 = carEntitiesLane2,
            Lane3 = carEntitiesLane3,
        }.Schedule(JobHandle.CombineDependencies(inputDeps, entityJob, carStateJob));

        inputDeps = new PositionSortJob()
        {
            Lane0 = carEntitiesLane0.AsDeferredJobArray(),
            Lane1 = carEntitiesLane1.AsDeferredJobArray(),
            Lane2 = carEntitiesLane2.AsDeferredJobArray(),
            Lane3 = carEntitiesLane3.AsDeferredJobArray(),
        }.Schedule(4, 1, inputDeps);

        inputDeps = new BuildSortIndicesMapping()
        {
            Lane0 = carEntitiesLane0.AsDeferredJobArray(),
            Lane1 = carEntitiesLane1.AsDeferredJobArray(),
            Lane2 = carEntitiesLane2.AsDeferredJobArray(),
            Lane3 = carEntitiesLane3.AsDeferredJobArray(),

            SortIndices = carSortIndices,
        }.Schedule(inputDeps);

        // 3. Car logic {Entity, index, velocity, state }
        // 4. Compose the matrices for each mesh instance for correct rendering.

        // TODO: dispose the arrays on completing jobs.
        inputDeps.Complete();
        carEntitiesLane0.Dispose();
        carEntitiesLane1.Dispose();
        carEntitiesLane2.Dispose();
        carEntitiesLane3.Dispose();
        carSortIndices.Dispose();

        return inputDeps;
    }
}
