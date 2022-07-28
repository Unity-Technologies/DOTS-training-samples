using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FarmerController : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        FarmerJob job = new FarmerJob{deltaTime = state.Time.DeltaTime};
        job.ScheduleParallel();
    }
}

public partial struct FarmerJob : IJobEntity
{
    public float deltaTime;
    public void Execute(TransformAspect transform,in Velocity velocity)
    {
        transform.Position += velocity.value * deltaTime;
    }
}
