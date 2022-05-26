using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct StartPositionJob : IJobEntity
{
    public float LaneLength;
    public float LaneWidth;
    public int LaneCount;

    void Execute([ChunkIndexInQuery] int chunkIndex, ref Translation carPosition, ref CarTraveledDistance traveledDistance, in CarAspect carAspect)
    {
        //carPosition.Value = CarMovementSystem.GetCarPosition(traveledDistance.Value, LaneLength, LaneWidth, carAspect.Lane, LaneCount);
    }
}

public partial struct CarStartPositionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.Enabled = false;
        state.RequireForUpdate<HighwayConfig>();
        state.RequireForUpdate<CarConfigComponent>();
        state.RequireForUpdate<CarId>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var highwayConfig = SystemAPI.GetSingleton<HighwayConfig>();

        var startPosJob = new StartPositionJob
        {
            LaneLength = highwayConfig.InsideLaneLength,
            LaneWidth = highwayConfig.InsideLaneWidth,
            LaneCount = highwayConfig.LaneCount
        };

        startPosJob.ScheduleParallel(state.Dependency);
        state.Enabled = false;
    }
}
