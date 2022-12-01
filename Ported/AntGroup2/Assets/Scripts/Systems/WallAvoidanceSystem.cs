using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct WallAvoidanceSystem : ISystem
{
    private EntityQuery wallQuery;
    
    public void OnCreate(ref SystemState state)
    {
        wallQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacle>().Build();
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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var walls = wallQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
        var job = new WallAvoidanceJob { wallPositions = walls };
        job.ScheduleParallel();
        
    }
 
    [BurstCompile]
    [WithAll(typeof(Ant))]
    partial struct WallAvoidanceJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<LocalTransform> wallPositions;
        
        private bool Intersect(float2 p1, float2 p2, float2 center, float radius, out float distance)
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
                distance = -1f;
                return false;
            }

            float t1 = (-b + math.sqrt(bb4ac)) / (2 * a);
            float t2 = (-b - math.sqrt(bb4ac)) / (2 * a);
            distance = (t1 > 0.0f && t1 < 1.0f) ? t1 : t2;
            return (t1 > 0.0f && t1 < 1.0f) || (t2 > 0.0f && t2 < 1.0f);
        }

        public void Execute(ref WallDirection wallDirection, in CurrentDirection currentDirection, in LocalTransform transformAspect)
        {
            //bool lineOfSight = true;
            int wallBounce = 0;
            for(int i = 0; i < wallPositions.Length; ++i)
            {
                var wall = wallPositions[i];
                //check if wall in nearby line of sight
                float range = 3.0f;
               

                for (float checkDir = -0.3f; checkDir <= 0.3f; checkDir += 0.6f)
                {
                    float3 antStep = transformAspect.Position + new float3(math.sin(currentDirection.Angle + checkDir),0, math.cos(currentDirection.Angle + checkDir)) * range;
                    float distance;
                    
                    if (Intersect(transformAspect.Position.xz, antStep.xz, wall.Position.xz, 1.0f, out distance))
                    {

                        float dir = -1.0f;
                        if (checkDir < 0)
                            dir = 1.0f;

                        float newAngle = (math.PI / 4.0f) * math.pow(distance,5.0f) * dir;
                        if (math.abs(newAngle) > math.abs(wallDirection.Angle))
                            wallDirection.Angle = newAngle;
                        
                        if (wallDirection.Angle > math.PI)
                            wallDirection.Angle -= (float)(math.PI * 2.0f);
                        if (wallDirection.Angle < -math.PI)
                            wallDirection.Angle += (float)(math.PI * 2.0f);
                    }
                }


                if (wallDirection.WallBounceDirection != 0)
                {
                    wallBounce = 0;
                }
                else if (transformAspect.Position.x - wall.Position.x <= 2f || transformAspect.Position.z - wall.Position.z <= 2f)
                {
                    var dx = transformAspect.Position.x - wall.Position.x;
                    var dy = transformAspect.Position.z - wall.Position.z;
                    float sqrDist = dx * dx + dy * dy;
                    if(sqrDist < /*ObstacleRadius*/ 1.8f * 1.8f)
                    {
                        var dist = math.sqrt(sqrDist);
                        dx /= dist;
                        dy /= dist;

                        // Determine if teh ant hit from inside (-1) or outside (1) of the wall circle 
                        float2 wallPos = wall.Position.zx;
                        float2 antPos = transformAspect.Position.xz;
                        var wallSqDist = math.lengthsq(wallPos);
                        var antSqDist = math.lengthsq(antPos);
                        wallBounce = wallSqDist > antSqDist ? -1 : 1;
                    }
                }

                wallDirection.WallBounceDirection = wallBounce;
            }
        }
    }
    
}
