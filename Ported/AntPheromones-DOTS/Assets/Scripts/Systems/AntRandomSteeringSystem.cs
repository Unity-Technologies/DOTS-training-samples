using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AntRandomSteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;
        
        var random = new Random(6541);
        
        float randomnessScale = 0.2f;
        
        float2 minRange = new float2(-1,-1);
        float2 maxRange = new float2(1,1);

        Entities
            .WithAll<Ant>()
            .ForEach((ref Heading heading) =>
            {
                heading.heading = math.normalize(heading.heading + (random.NextFloat2(minRange, maxRange) * randomnessScale)) ;
            }).ScheduleParallel();
    }
}
