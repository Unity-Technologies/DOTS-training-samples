using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class VelocityRotationSystem : SystemBase
{

    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity entity,ref Rotation Rotation, in Translation translation, in Velocity velocity, in Bee bee) =>
            {
                //float3 lookDirection = math.normalize(translation.Value + velocity.Value);
                Quaternion targetRotation=Quaternion.LookRotation(math.normalize(velocity.Value),float3.zero );

                Rotation.Value = targetRotation;

            })
            .ScheduleParallel();
    }
}
