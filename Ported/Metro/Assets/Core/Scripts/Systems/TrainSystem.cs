using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct TrainSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TrainPositionsBuffer>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var trainPositions = SystemAPI.GetSingletonBuffer<TrainPositionsBuffer>();

        foreach (var (transform, speed, waypoint, time, trainInfo) in SystemAPI.Query<TransformAspect, Speed, RefRW<Waypoint>, RefRW<IdleTime>, TrainInfo>())
        {
            if (time.ValueRO.Value > 0)
            {
                time.ValueRW.Value -= Time.deltaTime;
                continue;
            }

            float nextWaypointZ = -Globals.RailSize * 0.5f + (Globals.RailSize / (config.NumberOfStations + 1)) * (waypoint.ValueRO.WaypointID + 1) - Globals.PlatformSize*0.5f;

            if (transform.LocalPosition.z > nextWaypointZ && waypoint.ValueRO.WaypointID < config.NumberOfStations)
            {
                time.ValueRW.Value = Globals.TrainWaitTime;
                waypoint.ValueRW.WaypointID++;
                continue;
            }

            transform.LocalPosition += new float3(0, 0, speed.Value * Time.deltaTime);

            if (transform.LocalPosition.z > Globals.RailSize * 0.5f)
            {
                waypoint.ValueRW.WaypointID = 0;
                transform.LocalPosition = new float3(transform.LocalPosition.x, transform.LocalPosition.y, -Globals.RailSize * 0.5f);
            }
            trainPositions[trainInfo.Id] = new TrainPositionsBuffer() { positionZ = transform.LocalPosition.z };
        }
    }
}