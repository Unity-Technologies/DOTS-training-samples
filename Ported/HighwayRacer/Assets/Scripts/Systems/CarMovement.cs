using Authoring;
using Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct CarMovement : ISystem
{
    private EntityQuery laneQuery;
    EntityQuery carQuery;

    private NativeArray<NativeArray<Car>> CarsBySegmentNumber;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        laneQuery = SystemAPI.QueryBuilder().WithAll<Lane>().Build();
        state.RequireForUpdate(laneQuery);
        carQuery = SystemAPI.QueryBuilder().WithAll<Car>().WithAll<SegmentNumber>().Build();
        state.RequireForUpdate(carQuery);

        CarsBySegmentNumber = new NativeArray<NativeArray<Car>>(8, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        CarsBySegmentNumber.Dispose();

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var writer = ecb.AsParallelWriter();

        state.EntityManager.GetAllUniqueSharedComponents(out NativeList<SegmentNumber> segments, Allocator.TempJob);

        // Ensure our map has valid arrays in all indexes, as otherwise if the component filter does not return any results
        // for a given index we're passing garbage through to the job and blow up.
        // I'm sure there's a better way of doing this, but I'm not seeing it this late at night
        for (int i = 0; i < 8; i++)
        {
            CarsBySegmentNumber[i] = new NativeArray<Car>(0, Allocator.TempJob);
        }

        foreach (var segmentNumber in segments)
        {
            carQuery.SetSharedComponentFilter(segmentNumber);
            CarsBySegmentNumber[segmentNumber.SegmentId] = carQuery.ToComponentDataArray<Car>(Allocator.TempJob);
        }

        var testJob = new CarMovementJob
        {
            AllCarsInSegment0 = CarsBySegmentNumber[0],
            AllCarsInSegment1 = CarsBySegmentNumber[1],
            AllCarsInSegment2 = CarsBySegmentNumber[2],
            AllCarsInSegment3 = CarsBySegmentNumber[3],
            AllCarsInSegment4 = CarsBySegmentNumber[4],
            AllCarsInSegment5 = CarsBySegmentNumber[5],
            AllCarsInSegment6 = CarsBySegmentNumber[6],
            AllCarsInSegment7 = CarsBySegmentNumber[7],
            Lanes = laneQuery.ToComponentDataArray<Lane>(Allocator.TempJob),
            Config = config,
            EntityParallelWriter = writer,
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        var jobHandle = testJob.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;
        jobHandle.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
