using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct ResourceDetection : IJobEntity
{
    public float distance;
    public float mapSize;
    public float obstacleSize;
    public float steeringStrength;
    public int bucketResolution;
    public float2 resourcePosition;
    public float2 homePosition;
    [ReadOnly]
    public NativeArray<UnsafeList<float2>> buckets;

    public void Execute(ref Ant ant, in Position position, in Direction direction)
    {
        float2 targetPosition = ant.hasResource ? homePosition : resourcePosition;

        float dx = targetPosition.x - position.position.x;
        float dy = targetPosition.y - position.position.y;
        float dist = math.sqrt(dx * dx + dy * dy);


        // we are at the target
        if (dist < 0.5f)
        {
            ant.hasResource = !ant.hasResource;
            ant.resourceSteering = 180f;
        }


        int stepCount = (int)math.ceil(dist * .5f);
        bool blocked = false;
        for (int i = 0; i < stepCount; ++i)
        {
            float t = (float)i / stepCount;
            if (ObstacleDetection.DetectPositionInBuckets(position.position.x + dx * t, position.position.y + dy * t,
                buckets, obstacleSize, mapSize, bucketResolution))
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
            if (targetAngle - directionInRad > math.PI)
            {
                ant.resourceSteering = 90f;
            }
            else if (targetAngle - directionInRad < -math.PI)
            {
                ant.resourceSteering = -90f;
            }
            else
            {
                if (math.abs(targetAngle - directionInRad) < math.PI * .5f)
                    ant.resourceSteering = math.degrees((targetAngle - directionInRad) * steeringStrength);
            }
        }
    }
}