using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        Board board = EntityManager.GetComponentData<Board>(pheromoneEntity);
        
        float time = Time.DeltaTime;
        float timeMultiplier = GetSingleton<TimeMultiplier>().SimulationSpeed;
        float scaledTime = time * timeMultiplier;
        
        Entities.WithAll<Ant>().ForEach((ref Translation translation, ref Rotation rotation, ref Heading heading) =>
        {
            rotation.Value = quaternion.RotateZ(math.atan2(heading.heading.y, heading.heading.x));
            
            translation.Value.x += heading.heading.x * scaledTime;
            translation.Value.y += heading.heading.y * scaledTime;
            
            if (translation.Value.x > board.BoardWidth - 1)
            {
                translation.Value.x = translation.Value.x - 1.1f;
                heading.heading = new float2(-heading.heading.x, heading.heading.y);
            }

            if (translation.Value.y > board.BoardHeight - 1)
            {
                translation.Value.y = translation.Value.y - 1.1f;
                heading.heading = new float2(heading.heading.x, -heading.heading.y);
            }

            if (translation.Value.x < 1)
            {
                translation.Value.x = translation.Value.x + 1.1f;
                heading.heading = new float2(-heading.heading.x, heading.heading.y);
            }

            if (translation.Value.y < 1)
            {
                translation.Value.y = translation.Value.y + 1.1f;
                heading.heading = new float2(heading.heading.x, -heading.heading.y);
            }
            
        }).ScheduleParallel();
    }
}
