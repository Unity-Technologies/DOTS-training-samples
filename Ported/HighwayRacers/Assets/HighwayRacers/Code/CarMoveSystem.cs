using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(CarSpawnSystem))]
//[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct CarMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //Debug.Log("Did spawn system create!");
        //state.RequireForUpdate<ExecuteCarSpawn>();
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        float3 moveAmount = new float3(Time.deltaTime, 0, 0);
        // For every entity having a LocalTransform and Player component, a read-write reference to
        // the LocalTransform is assigned to 'playerTransform'.
        foreach (var playerTransform in
                 SystemAPI.Query<RefRW<LocalTransform>>()
                     .WithAll<Car>())
        {
            var newPos = playerTransform.ValueRO.Position + moveAmount;
            playerTransform.ValueRW.Position = newPos;
        }
    }
}