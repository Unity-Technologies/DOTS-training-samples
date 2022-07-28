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
    public void Execute(TargetPosition targetPosition, ref Velocity velocity, in TransformAspect farmerPosition)
    {
        //if(farmerPosition.Position==targetPosition.Target)
       // velocity.value = targetPosition.Target;
    }
}
