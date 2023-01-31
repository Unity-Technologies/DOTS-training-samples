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
        state.RequireForUpdate<Train>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new TrainMovementJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
    
}

[BurstCompile]
[WithAll(typeof(Train))]
public partial struct TrainMovementJob : IJobEntity
{
    public float deltaTime;

    private void Execute(RefRO<Train> train, DestinationAspect destinationAspect)
    {
        if (train.ValueRO.State != TrainState.TrainMovement)
        {
            return;
        }

        float3 direction = math.normalize(destinationAspect.target.ValueRO.TargetPosition - destinationAspect.transform.WorldPosition);
        float3 move = train.ValueRO.Speed * direction * deltaTime;

        float distToGoSq = math.distancesq(destinationAspect.target.ValueRO.TargetPosition, destinationAspect.transform.WorldPosition);
        if (distToGoSq < math.lengthsq(move))
        {
            move = distToGoSq * direction;
        }
        
        destinationAspect.transform.WorldPosition += move;
    }
}
