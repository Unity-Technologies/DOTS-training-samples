using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor.Overlays;

partial struct WallAvoidanceSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var ant in SystemAPI.Query<TargetDirectionAspect, TransformAspect>().WithAll<Ant>())
        {
            foreach (var wall in SystemAPI.Query<TransformAspect>().WithAll<Obstacle>())
            {
                if (ant.Item2.WorldPosition.x - wall.WorldPosition.x <= 2f || ant.Item2.WorldPosition.z - wall.WorldPosition.z <= 2f)
                {
                    /* Original code 
                    
                    dx = ant.position.x - obstacle.position.x;
				    dy = ant.position.y - obstacle.position.y;
				    float sqrDist = dx * dx + dy * dy;
				    if (sqrDist<obstacleRadius*obstacleRadius) {
				    	dist = Mathf.Sqrt(sqrDist);
				    	dx /= dist;
				    	dy /= dist;
				    	ant.position.x = obstacle.position.x + dx * obstacleRadius;
				    	ant.position.y = obstacle.position.y + dy * obstacleRadius;

				    	vx -= dx * (dx * vx + dy * vy) * 1.5f;
				    	vy -= dy * (dx * vx + dy * vy) * 1.5f;
				    }
                    */

                    var dx = ant.Item2.WorldPosition.x - wall.WorldPosition.x;
                    var dy = ant.Item2.WorldPosition.z - wall.WorldPosition.z;
                    float sqrDist = dx * dx + dy * dy;
                    if(sqrDist < /*ObstacleRadius*/ 2f * 2f)
                    {
                        var dist = math.sqrt(sqrDist);
                        dx /= dist;
                        dy /= dist;
                        ant.Item1.Direction = math.atan2(wall.WorldPosition.x + dx * /*ObstacleRadius*/2f, wall.WorldPosition.z + dy * /*ObstacleRadius*/ 2f);
                    }
                }
            }
        }
    }
}
