using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FarmerTargetController : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        FindTargetJob targetJob = new FindTargetJob();
        targetJob.ScheduleParallel();
    }
    
    
}

public  partial struct FindTargetJob: IJobEntity
{
    public void Execute(TransformAspect transformAspect, TargetPosition targetPosition, ref Velocity velocity,in FarmerSpeed speed)
    {
        float3 delta = targetPosition.Target - transformAspect.Position; 
        Debug.Log(delta);
        float3 targetVelocity = 0.0f;
        if (math.lengthsq(delta) > 0.0001f)
        {
            float3 direction = math.normalize(delta);
            float distance = math.length(delta);
            float finalSpeed = speed.MovementSpeed* math.clamp(distance, 0f, 1f);
            targetVelocity = direction * finalSpeed;

        }
        velocity.value = math.lerp(velocity.value, targetVelocity, 0.5f);
    }
}
