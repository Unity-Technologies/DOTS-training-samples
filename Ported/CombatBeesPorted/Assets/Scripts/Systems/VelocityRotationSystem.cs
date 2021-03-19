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
            .WithAny<VelocityScale>() // Hijacking VelocityScale because we probably won't want rotation on anything else
            .WithNone<Grounded>()
            .ForEach((Entity entity,ref Rotation Rotation, in Velocity velocity) =>
            {
                
                if (velocity.Value.Equals(float3.zero))
                    return;
                float3 forward = math.normalize(velocity.Value);
                float3 right = math.normalize(math.cross(new float3(0,1,0), forward));
                float3 up = math.cross(forward, right);
                
                // rotation looks a pretty bad when velocity is changing rapidly (idling). consider softer idling or storing last rotation.
                // swap up and forward if model is rotated to face X or Z
                Quaternion targetRotation = quaternion.LookRotation(up,forward);

                Rotation.Value = targetRotation;

            })
            .Schedule();
    }
}
