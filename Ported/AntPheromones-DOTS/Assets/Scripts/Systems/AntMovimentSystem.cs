using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class AntMovimentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;

        float timeMultiplier = GetSingleton<TimeMultiplier>().SimulationSpeed;

        float scaledTime = time * timeMultiplier;
        
        Entities.WithAll<Ant>().ForEach((ref Translation translation, ref Rotation rotation, in Heading heading) =>
        {
            rotation.Value = quaternion.RotateZ(math.atan2(heading.heading.y, heading.heading.x));
            
            translation.Value.x += heading.heading.x * scaledTime;
            translation.Value.y += heading.heading.y * scaledTime;
        }).ScheduleParallel();
    }
}
