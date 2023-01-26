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
        carQuery = SystemAPI.QueryBuilder().WithAll<Car>().Build();
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

        var testJob = new CarMovementJob()
        {
            AllCars = carQuery.ToComponentDataArray<Car>(Allocator.TempJob),
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
