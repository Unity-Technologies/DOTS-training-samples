using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Jobs;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public partial struct CarMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
     
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var testJob = new CarMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        var jobHandle = testJob.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;
        jobHandle.Complete();
        
        /*float3 moveAmount = new float3(Time.deltaTime, 0, 0);
       
        foreach (var playerTransform in
                 SystemAPI.Query<RefRW<LocalTransform>>()
                     .WithAll<Car>())
        {
            var newPos = playerTransform.ValueRO.Position + moveAmount;
            playerTransform.ValueRW.Position = newPos;
        }*/
    }
}