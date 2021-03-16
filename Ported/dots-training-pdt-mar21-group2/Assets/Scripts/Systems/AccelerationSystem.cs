using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class AccelerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<Falling>()
            .ForEach((ref Velocity velocity) =>
            {
                velocity.Value.y -= 9.8f * deltaTime;
            }).ScheduleParallel();
    }
}
