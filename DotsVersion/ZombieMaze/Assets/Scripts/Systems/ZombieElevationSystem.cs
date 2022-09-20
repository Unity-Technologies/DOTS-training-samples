using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(ElevatingPosition))]
partial struct ZombieElevationJob : IJobEntity
{
    public float DeltaTime;
    void Execute(Entity entity, TransformAspect transform)
    {
        transform.Position += new float3(0,1,0) * DeltaTime;
    }
}
[BurstCompile]
[WithAll(typeof(ElevatingPosition))]
partial struct ZombieElevationCheckJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<ElevatingPosition> ElevatingPositionFromEntity;

    public float Position;

    void Execute(Entity entity, TransformAspect transform)
    {
        ElevatingPositionFromEntity.SetComponentEnabled(entity, transform.Position.y < Position);
    }
}

[BurstCompile]
public partial struct ZombieElevationSystem : ISystem
{
    ComponentLookup<ElevatingPosition> m_ElevatingPositionFromEntity;
    
    public void OnCreate(ref SystemState state)
    {
        m_ElevatingPositionFromEntity = state.GetComponentLookup<ElevatingPosition>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = Time.deltaTime;
 
         m_ElevatingPositionFromEntity.Update(ref state);
         var elevationCheckJob = new ZombieElevationCheckJob
         {
             ElevatingPositionFromEntity = m_ElevatingPositionFromEntity,
             Position = 0.3f
         };
        elevationCheckJob.ScheduleParallel();//JobHandle?
        
        var elevationJob = new ZombieElevationJob
        {
            DeltaTime = deltaTime
        };
        elevationJob.ScheduleParallel();
    }
}