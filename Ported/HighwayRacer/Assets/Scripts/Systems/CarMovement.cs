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

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        laneQuery = SystemAPI.QueryBuilder().WithAll<Lane>().Build();
        state.RequireForUpdate(laneQuery);
        carQuery = SystemAPI.QueryBuilder().WithAll<Car>().WithAll<SegmentNumber>().Build();
        state.RequireForUpdate(carQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var writer = ecb.AsParallelWriter();

        state.EntityManager.GetAllUniqueSharedComponents(out NativeList<SegmentNumber> segments, Allocator.TempJob);

        var carsBySegmentNumber = new NativeArray<NativeArray<Car>>(8, Allocator.Temp);

        // Ensure our map has valid arrays in all indexes, as otherwise if the component filter does not return any results
        // for a given index we're passing garbage through to the job and blow up.
        // I'm sure there's a better way of doing this, but I'm not seeing it this late at night
        for (int i = 0; i < 8; i++)
        {
            carsBySegmentNumber[i] = new NativeArray<Car>(0, Allocator.TempJob);
        }

        foreach (var segmentNumber in segments)
        {
            carQuery.SetSharedComponentFilter(segmentNumber);
            carsBySegmentNumber[segmentNumber.SegmentId] = carQuery.ToComponentDataArray<Car>(Allocator.TempJob);
        }

        var testJob = new CarMovementJob
        {
            AllCarsInSegment0 = carsBySegmentNumber[0],
            AllCarsInSegment1 = carsBySegmentNumber[1],
            AllCarsInSegment2 = carsBySegmentNumber[2],
            AllCarsInSegment3 = carsBySegmentNumber[3],
            AllCarsInSegment4 = carsBySegmentNumber[4],
            AllCarsInSegment5 = carsBySegmentNumber[5],
            AllCarsInSegment6 = carsBySegmentNumber[6],
            AllCarsInSegment7 = carsBySegmentNumber[7],
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
