using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

public class AntObstacleAvoidSystem : SystemBase
{
    //Query for getting all the world obstacles
    EntityQuery obstacleQuery;

    protected override void OnCreate()
    {
        //Cache our obstacle query and require it to return something for OnUpdate to run
        var obstacleQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Radius>() }
        };
        obstacleQuery = GetEntityQuery(obstacleQueryDesc);
        RequireForUpdate(obstacleQuery);
    }

    protected override void OnUpdate()
    {
        var obstRadiusArray = obstacleQuery.ToComponentDataArray<Radius>(Allocator.TempJob);
        var obstTranslateArray = obstacleQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
       
        //Update all ant entities and check that we are not going to collide with
        //a obstacle
        Entities
            .WithNativeDisableParallelForRestriction(obstRadiusArray) //It's safe here because we are only reading from the array
            .WithAll<Speed, Direction>()
            .ForEach((ref Direction dir, ref Translation antTranslation) =>
            {

                //Check this entity for collisions with all other entites
                for(int i = 0; i < obstRadiusArray.Length; ++i)
                {
                    //Get difference in x and y, calculate the sqrd distance to the 
                    float dx = antTranslation.Value.x - obstTranslateArray[i].Value.x;
                    float dy = antTranslation.Value.z - obstTranslateArray[i].Value.y;
                    float sqrDist = (dx * dx) + (dy * dy);

                    //If we are less than the sqrd distance away from the obstacle then reflect the ant
                    if(sqrDist < (obstRadiusArray[i].Value * obstRadiusArray[i].Value))
                    {
                        //Reflect
                        dir.Value += Mathf.PI;
                        dir.Value = (dir.Value >= 2 * Mathf.PI) ? dir.Value - 2 * Mathf.PI : dir.Value;


                        /*
                        float dist = Mathf.Sqrt(sqrDist);
                        dx /= dist;
                        dy /= dist;

                        //Move ant out of collision
                        antTranslation.Value.x = currentObst.position.x + dx * currentObst.radius;
                        antTranslation.Value.y = currentObst.position.y + dy * currentObst.radius;
                        */
                    }

                }

            }).WithDisposeOnCompletion(obstRadiusArray)
            .ScheduleParallel();
    }

}
