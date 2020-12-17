using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntBoardEdgeInvertSteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        /*Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        Board board = EntityManager.GetComponentData<Board>(pheromoneEntity);

        Entities
            .WithAll<Ant>()
            .ForEach((ref Heading heading, ref Translation translation) =>
            {
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

            }).Schedule();*/
        
    }
}
