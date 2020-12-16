using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AntCollisionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        const float repulseSteerWeight = 0.05f;
        const float originalDirectionWeight = 1.0f - repulseSteerWeight;

        var obstacleEntities = GetEntityQuery(typeof(Obstacle)).ToEntityArray(Allocator.Temp);
        
        Entities
            .WithAll<Ant>()
            .ForEach((ref Heading heading, in Translation translation) =>
            {
                // for (int i = 0; i < obstacleEntities.Length; ++i)
                // {
                //     var obstacleTranslation = GetComponent<Translation>(obstacleEntities[i]);
                //     var obstacleRadius = GetComponent<Radius>(obstacleEntities[i]);
                //
                //     var antToCollider = translation.Value - obstacleTranslation.Value;
                //     var sqrMag = antToCollider.x * antToCollider.x + antToCollider.y * antToCollider.y;
                //
                //     if (sqrMag <= (obstacleRadius.radius * obstacleRadius.radius))
                //     {
                //         // Collided! Reverse the heading
                //         heading.heading = new float2(heading.heading.x * -1, heading.heading.y * -1);
                //     }
                // }
                
                // float2 repulseDirection = new float();
                // heading.heading = math.normalize((heading.heading * originalDirectionWeight) + (repulseDirection * repulseSteerWeight)); 
            }).Run();
    }
}
