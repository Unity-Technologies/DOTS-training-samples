using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class AntObstacleAvoidSystem : SystemBase
{

    protected override void OnUpdate()
    {
        //Get all of the obstacles
        var obstacleQuery = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Obstacle>() }
        };
        var obstacles = GetEntityQuery(obstacleQuery);

        //Check that we found some obstacles
        if (obstacles.IsEmpty)
        {
            return;
        }
        var obstacleArray = obstacles.ToComponentDataArray<Obstacle>(Allocator.TempJob);
       

        //Update all ant entities and check that we are not going to collide with
        //a obstacle
        Entities
            .WithNativeDisableParallelForRestriction(obstacleArray) //It's safe here because we are only reading from the array
            .WithAll<Direction>()
            .ForEach((ref Direction dir, ref Translation antTranslation) =>
            {
                //todo convert to job?
                //Check this entity for collisions with all other entites
                for(int i = 0; i < obstacleArray.Length; ++i)
                {
                    //Get difference in x and y, calculate the sqrd distance to the 
                    Obstacle currentObst = obstacleArray[i];
                    float dx = antTranslation.Value.x - currentObst.position.x;
                    float dy = antTranslation.Value.y - currentObst.position.y;
                    float sqrDist = (dx * dx) + (dy * dy);

                    //If we are less than the sqrd distance away from the obstacle then reflect the ant
                    if(sqrDist < (currentObst.radius * currentObst.radius))
                    {
                        //Reflect
                        dir.Value = -dir.Value;

                        //Move ant out of collision
                        antTranslation.Value.x = currentObst.position.x + dx * currentObst.radius;
                        antTranslation.Value.y = currentObst.position.y + dy * currentObst.radius;
                    }

                }

            }).WithDisposeOnCompletion(obstacleArray)
            .ScheduleParallel();
    }

}
