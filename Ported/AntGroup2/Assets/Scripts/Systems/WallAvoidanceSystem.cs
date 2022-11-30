using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial struct WallAvoidanceSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }
    
    private bool Intersect(float2 p1, float2 p2, float2 center, float radius)
    {
        //  get the distance between X and Z on the segment
        float2 dp = p2 - p1;

        float a = math.dot(dp, dp);
        float b = 2 * math.dot(dp, p1 - center);
        float c = math.dot(center, center) - 2 * math.dot(center, p1) + math.dot(p1, p1) - radius * radius;
        float bb4ac = b * b - 4 * a * c;
        if (math.abs(a) < float.Epsilon || bb4ac < 0)
        {
            //  line does not intersect
            return false;
        }

        float t1 = (-b + math.sqrt(bb4ac)) / (2 * a);
        float t2 = (-b - math.sqrt(bb4ac)) / (2 * a);

        return (t1 > 0.0f && t1 < 1.0f) || (t2 > 0.0f && t2 < 1.0f);
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var ant in SystemAPI.Query<DirectionAspect, TransformAspect>().WithAll<Ant>())
        {
            //bool lineOfSight = true;
            int wallBounce = 0;
            foreach (var wall in SystemAPI.Query<TransformAspect>().WithAll<Obstacle>())
            {
                //check if wall in nearby line of sight
                float range = 3.0f;
                float3 antStep = ant.Item2.WorldPosition + new float3(math.sin(ant.Item1.CurrentDirection),0, math.cos(ant.Item1.CurrentDirection)) * range;
                if (Intersect(ant.Item2.WorldPosition.xz, antStep.xz, wall.LocalPosition.xz, 1.0f))
                {
                    var dx = antStep.x - wall.WorldPosition.x;
                    var dy = antStep.z - wall.WorldPosition.z;
                    float sqrDist = dx * dx + dy * dy;

                    float dir = 1.0f;
                    float2 reflect = math.reflect(new float2(ant.Item2.Forward.x, ant.Item2.Forward.z), new float2(ant.Item2.WorldPosition.x, ant.Item2.WorldPosition.z));
                    float target = math.atan2(reflect.x, reflect.y);

                    target -= ant.Item1.CurrentDirection;
                    if (target < 0)
                        dir = -1.0f;
                    
                    ant.Item1.WallDirection = (math.PI / 4.0f) * (math.pow((range*range - sqrDist)/(range*range ), 1.0f)) * dir;
                    if (ant.Item1.WallDirection > math.PI)
                        ant.Item1.WallDirection -= (float)(math.PI * 2.0f);
                    if (ant.Item1.WallDirection < -math.PI)
                        ant.Item1.WallDirection += (float)(math.PI * 2.0f);
                }
                
                //check if inside wall
                if (ant.Item2.WorldPosition.x - wall.WorldPosition.x <= 2f || ant.Item2.WorldPosition.z - wall.WorldPosition.z <= 2f)
                {
                    var dx = ant.Item2.WorldPosition.x - wall.WorldPosition.x;
                    var dy = ant.Item2.WorldPosition.z - wall.WorldPosition.z;
                    float sqrDist = dx * dx + dy * dy;
                    if(sqrDist < /*ObstacleRadius*/ 1.8f * 1.8f)
                    {
                        var dist = math.sqrt(sqrDist);
                        dx /= dist;
                        dy /= dist;

                        // Determine if teh ant hit from inside (-1) or outside (1) of the wall circle 
                        float2 wallPos = wall.WorldPosition.xz;
                        float2 antPos = ant.Item2.WorldPosition.xz;
                        var wallSqDist = math.lengthsq(wallPos);
                        var antSqDist = math.lengthsq(antPos);
                        wallBounce = wallSqDist > antSqDist ? -1 : 1;
                    }
                }

                ant.Item1.WallBounceDirection = wallBounce;
            }
        }
    }
}
