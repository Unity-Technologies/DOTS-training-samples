using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Rendering;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct ResourceDetection : IJobEntity
{
    public float mapSize;
    public float obstacleSize;
    
    public int bucketResolution;
    public float2 resourcePosition;
    public float2 homePosition;
    
    [NativeDisableUnsafePtrRestriction]
    public RefRW<Stats> stats;
    
    [ReadOnly]
    public NativeArray<Bucket> buckets;

    public void Execute(ref Ant ant, in Position position, in Direction direction)
    {
        float2 targetPosition = ant.hasResource ? homePosition : resourcePosition;

        float dx = targetPosition.x - position.position.x;
        float dy = targetPosition.y - position.position.y;
        float dist = math.sqrt(dx * dx + dy * dy);


        // we are at the target
        if (dist < 4f)
        {
            if (ant.hasResource)
                stats.ValueRW.foodCount++;
            
            ant.hasResource = !ant.hasResource;
            ant.resourceSteering = 180f;
            return;
        }


        int stepCount = (int)math.ceil(dist * .5f);
        bool blocked = false;
        for (int i = 0; i < stepCount; ++i)
        {
            float t = (float)i / stepCount;
            float _, __;
            if (ObstacleDetection.DetectPositionInBuckets(position.position.x + dx * t, position.position.y + dy * t,
                buckets, obstacleSize, mapSize, bucketResolution, out _, out __))
            {
                blocked = true;
                break;
            }
        }


        if (blocked)
        {
            ant.resourceSteering = 0;
        }
        else
        {
            float directionInRad = math.radians(direction.direction);
            float targetAngle = math.atan2(targetPosition.y - position.position.y, targetPosition.x - position.position.x);
            var dPositiveAngle = targetAngle - directionInRad;
            while (dPositiveAngle < 0)
                dPositiveAngle += math.PI * 2;
            
            if (dPositiveAngle > 0 && dPositiveAngle < math.PI)
                ant.resourceSteering = 5f;
            else
                ant.resourceSteering = -5f;
        }
    }
}