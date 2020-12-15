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
        var random = new Random(244);
        
        Entities.WithAll<Ant>().ForEach((ref Translation translation, ref Rotation rotation, in Heading heading) =>
        {
            translation.Value.x += heading.heading.x * time;
            translation.Value.y += heading.heading.y * time;
        }).ScheduleParallel();
    }
}
