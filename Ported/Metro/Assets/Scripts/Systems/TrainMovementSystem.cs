using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TrainMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        TrainMovementJob job = new TrainMovementJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        job.ScheduleParallel();
    }
    
}

[BurstCompile]
[WithAll(typeof(Train))]
public partial struct TrainMovementJob : IJobEntity
{
    public float deltaTime;

    private void Execute(RefRO<Train> train, RefRO<TargetDestination> targetPos, RefRW<LocalTransform> transform)
    {
        if (train.ValueRO.State != TrainState.TrainMovement)
        {
            return;
        }

        float3 direction = math.normalize(targetPos.ValueRO.TargetPosition - transform.ValueRO.Position);
        float3 move = train.ValueRO.Speed * direction * deltaTime;

        float distToGoSq = math.distancesq(targetPos.ValueRO.TargetPosition, transform.ValueRO.Position);
        if (distToGoSq < math.lengthsq(move))
        {
            move = distToGoSq * direction;
        }
        
        transform.ValueRW.Position += move;
    }
}
