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
        state.RequireForUpdate<PlatformTrainStatusBuffer>();
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
        var platformTrainStatus = SystemAPI.GetSingletonBuffer<PlatformTrainStatusBuffer>();

        foreach (var (transform, speed, waypoint, time, trainInfo, locationInfo) in SystemAPI.Query<TransformAspect, Speed, RefRW<WaypointID>, RefRW<IdleTime>, TrainInfo, LocationInfo>())
        {
            if (time.ValueRO.Value > 0)
            {
                time.ValueRW.Value -= Time.deltaTime;
                continue;
            }

            float nextWaypointZ = -Globals.RailSize * 0.5f + (Globals.RailSize / (config.NumberOfStations + 1)) * (waypoint.ValueRO.Value + 1) - Globals.PlatformSize*0.5f;

            if (transform.LocalPosition.z > nextWaypointZ && waypoint.ValueRO.Value < config.NumberOfStations)
            {
                time.ValueRW.Value = Globals.TrainWaitTime;
                waypoint.ValueRW.Value++;
                platformTrainStatus[locationInfo.CurrentPlatform] = new PlatformTrainStatusBuffer() { TrainID = trainInfo.Id };
                Debug.Log(locationInfo.CurrentPlatform + " status: " + platformTrainStatus[locationInfo.CurrentPlatform].TrainID);
                continue;
            }

            if (platformTrainStatus[locationInfo.CurrentPlatform].TrainID == trainInfo.Id) // if we are moving and platform we are at has us marked, de-mark us
            {
                platformTrainStatus[locationInfo.CurrentPlatform] = new PlatformTrainStatusBuffer() { TrainID = -1 };
                Debug.Log("left: "+locationInfo.CurrentPlatform + " status: " + platformTrainStatus[locationInfo.CurrentPlatform].TrainID);
            }

            transform.LocalPosition += new float3(0, 0, speed.Value * Time.deltaTime);

            if (transform.LocalPosition.z > Globals.RailSize * 0.5f)
            {
                waypoint.ValueRW.Value = 0;
                transform.LocalPosition = new float3(transform.LocalPosition.x, transform.LocalPosition.y, -Globals.RailSize * 0.5f);
            }
            trainPositions[trainInfo.Id] = new TrainPositionsBuffer() { positionZ = transform.LocalPosition.z };
        }
    }
}