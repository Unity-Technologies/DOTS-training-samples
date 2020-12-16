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

        var antEntities = GetEntityQuery(typeof(Ant)).ToEntityArray(Allocator.Temp);
        
        Entities
            .WithAll<Collider>()
            .ForEach((in Translation translation, in Radius radius) =>
            {
                for (int i = 0; i < antEntities.Length; ++i)
                {
                    var antTranslation = GetComponent<Translation>(antEntities[i]);
                    var antHeading = GetComponent<Heading>(antEntities[i]);

                    var antToCollider = translation.Value - antTranslation.Value;
                    var sqrMag = Vector3.SqrMagnitude(antToCollider);

                    if (sqrMag <= (radius.radius * radius.radius))
                    {
                        // Collided! Reverse the heading
                        // antHeading.heading = new Heading{ heading = new float2(antHeading.heading.x * -1, antHeading.heading.y * -1)};
                    }
                    
                    
                }
                
                float2 repulseDirection = new float();
                // heading.heading = math.normalize((heading.heading * originalDirectionWeight) + (repulseDirection * repulseSteerWeight)); 
            }).ScheduleParallel();
    }
}
