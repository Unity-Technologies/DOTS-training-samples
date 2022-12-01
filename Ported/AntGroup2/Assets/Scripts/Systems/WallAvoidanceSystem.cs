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
        bool sort = true;
        if (sort)
        {
            walls.Sort(new WallSort());
        }

        var job = new WallAvoidanceJob { wallPositions = walls, ArrayIsSorted = sort};
        job.ScheduleParallel();
        
    }

    struct WallSort : IComparer<LocalTransform>
    {
        public int Compare(LocalTransform a, LocalTransform b)
        {
            return a.Position.x.CompareTo(b.Position.x);
        }
    }

    [WithAll(typeof(Ant))]
    [BurstCompile]
    partial struct WallAvoidanceJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<LocalTransform> wallPositions;

        public bool ArrayIsSorted;
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
        
        [BurstCompile]
        public void Execute(ref WallDirection wallDirection, in CurrentDirection currentDirection, in LocalTransform transformAspect)
        {
            if(wallPositions.Length == 0)
                return;
            //bool lineOfSight = true;
            int wallBounce = 0;
            
            //Find wall range
            float range = 3.0f;
            int index = wallPositions.Length / 2;
            int prevIndex = wallPositions.Length;
            float currentX = wallPositions[index].Position.x;
            int leftIndex = 0;
            int rightIndex = wallPositions.Length;

            if (ArrayIsSorted)
            {
                int x = 0;
                while (!(currentX > transformAspect.Position.x + range &&
                         currentX < transformAspect.Position.x - range) && x < 6)
                {
                    if (currentX > transformAspect.Position.x)
                    {
                        int prev = prevIndex;
                        prevIndex = index;
                        index -= math.abs(prev - index) / 2;
                    }
                    else
                    {
                        int prev = prevIndex;
                        prevIndex = index;
                        index += math.abs(prev - index) / 2;
                    }

                    ++x;
                    currentX = wallPositions[index].Position.x;
                }

                leftIndex = index;
                while (currentX > transformAspect.Position.x - range && leftIndex != 0)
                {
                    leftIndex -= 5;
                    leftIndex = math.max(leftIndex, 0);
                    currentX = wallPositions[leftIndex].Position.x;
                }

                rightIndex = index;
                currentX = wallPositions[index].Position.x;
                while (currentX < transformAspect.Position.x + range && rightIndex != wallPositions.Length - 1)
                {
                    rightIndex += 5;
                    rightIndex = math.min(rightIndex, wallPositions.Length - 1);
                    currentX = wallPositions[rightIndex].Position.x;
                }

                //Debug.Log(rightIndex - leftIndex + "  " + leftIndex + "  " + index + "  " + rightIndex);
            }

            for(int i = leftIndex; i < rightIndex; ++i)
            {
                var wall = wallPositions[i];
                //check if wall in nearby line of sight

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
