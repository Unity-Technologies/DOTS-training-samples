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
        var random = new Random(6541);

        const float randomSteerWeight = 0.005f;
        const float originalDirectionWeight = 1.0f - randomSteerWeight;
        
        var minRange = new float2(-1,-1);
        var maxRange = new float2(1,1);

        Entities
            .WithAll<Ant>()
            .ForEach((ref Heading heading) =>
            {
                var randomDirection = math.normalize(random.NextFloat2(minRange, maxRange));
                heading.heading = (math.normalize(heading.heading) * originalDirectionWeight) + (randomDirection * randomSteerWeight); 
            }).ScheduleParallel();
    }
}
