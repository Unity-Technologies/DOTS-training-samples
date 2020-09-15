using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(YawToRotationSystem))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float moveSpeed = 1.0f; // temp
        Entities.WithAll<AntTag>().ForEach((ref Translation translation, in Rotation rotation) =>
        {
            float3 forward = new float3(0.0f, 0.0f, 1.0f);
            translation.Value += math.mul(rotation.Value, forward) * deltaTime * moveSpeed;
        }).Run();
    }
}
